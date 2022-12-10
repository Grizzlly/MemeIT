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

namespace MemeIT.Server.Controllers
{
    [Authorize]
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

        [HttpGet]
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

        [HttpGet]
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
    }
}
