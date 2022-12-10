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

namespace MemeIT.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api")]
    public class MemesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("memes")]
        public ActionResult<IEnumerable<MemeDTO>> GetMemes()
        {
            if(_context.Memes is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok(_context.Memes.ToList());
        }
    }
}
