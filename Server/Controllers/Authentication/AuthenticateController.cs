using MemeIT.Server.Database;
using MemeIT.Shared.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace MemeIT.Server.Controllers.Authentication
{
    [Authorize]
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AuthenticateController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            ApplicationUser? userExists = await userManager.FindByNameAsync(model.UserName);

            if (userExists is not null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponse { Message = "User already exists!" });
            }

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };

            IdentityResult result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponse { Message = "User creation failed! Please check user details and try again." });
            }

            return Ok(new AuthResponse { Message = "User created successfully!" });
        }
    }
}
