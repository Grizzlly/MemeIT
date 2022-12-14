using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MemeIT.Server.Database;
using Microsoft.AspNetCore.Authorization;
using MemeIT.Shared.Models;
using Mapster;
using System.Security.Claims;

namespace MemeIT.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class MemesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly TypeAdapterConfig config;

        public MemesController(ApplicationDbContext context)
        {
            _context = context;

            config = new();
            config.ForType<Meme, MemeDto>()
                .Map(dest => dest.CreatorUsername, src => src.Creator != null ? src.Creator!.UserName : null);
        }

        [HttpGet, AllowAnonymous]
        [Route("memes")]
        public ActionResult<IEnumerable<MemeDto>> GetMemes()
        {
            if(_context.Memes is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var memes = _context.Memes.Include(m => m.Creator).ToList();

            return Ok(memes.Select(x => x.Adapt<MemeDto>(config)));
        }

        [HttpGet, AllowAnonymous]
        [Route("memes/{memeid:guid}")]
        public ActionResult<MemeDto> GetMeme(Guid memeid)
        {
            if (_context.Memes is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var meme = _context.Memes.Where(m => m.MemeId == memeid).FirstOrDefault(defaultValue: null);

            if (meme is null) return NotFound();
            else return Ok(meme.Adapt<MemeDto>(config));
        }

        [HttpPost, Authorize]
        [Route("memes")]
        public IActionResult InsertMeme([FromBody] MemeDto? meme)
        {
            if (meme is null) return BadRequest();

            Meme newMeme = meme.Adapt<Meme>();

            newMeme.MemeId = new Guid();

            string claimid = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

            if(!Guid.TryParse(claimid, out Guid claim)) return StatusCode(StatusCodes.Status500InternalServerError);

            newMeme.CreatorId = claim;

            _context.Memes.Add(newMeme);

            return CreatedAtRoute("memes", newMeme.MemeId);
        }

        [HttpPut, Authorize]
        [Route("memes/{memeid:guid}")]
        public IActionResult PutMeme(Guid memeid, [FromBody] MemeDto? meme)
        {
            if (meme.HasValue is false) return BadRequest();

            var existingMeme = _context.Memes.Where(m => m.MemeId == memeid).FirstOrDefault(defaultValue: null);

            if (existingMeme == null) return NotFound();

            existingMeme.Description = meme.Value.Description ?? string.Empty;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete, Authorize]
        [Route("memes/{memeid:guid}")]
        public IActionResult DeleteMeme(Guid memeid)
        {
            var existingMeme = _context.Memes.Where(m => m.MemeId == memeid).FirstOrDefault(defaultValue: null);

            if (existingMeme is null) return BadRequest();

            string nameidentifier_claim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
            if (!Guid.TryParse(nameidentifier_claim, out Guid nameidentifier)) return StatusCode(StatusCodes.Status500InternalServerError);

            if(existingMeme?.CreatorId != nameidentifier) return Unauthorized();

            _context.Memes.Remove(existingMeme);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
