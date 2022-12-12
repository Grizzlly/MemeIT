using MemeIT.Server.Database;
using MemeIT.Shared.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MemeIT.Server.Controllers.Authentication
{
    [Authorize]
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticateController : ControllerBase
    {
        public static readonly SymmetricSecurityKey SecurityAppKey = new(Encoding.ASCII.GetBytes("ByYM000OLlMQG6VVVp1OH7Xzyr7gHuw1qvUC5dcGt3SNM"));

        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;

        public AuthenticateController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
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

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            ApplicationUser user = (await userManager.FindByNameAsync(loginModel.UserName))!;

            if ((user is not null) && await userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                Task<IList<string>> userRoles = userManager.GetRolesAsync(user);

                List<Claim> authClaims = new()
                {
                    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.AuthenticationMethod, "pwd")
                };

                foreach (string role in await userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                SymmetricSecurityKey authSigningKey = SecurityAppKey;

                JwtSecurityToken token = new(
                    issuer: configuration["JWT:ValidIssuer"],
                    audience: configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddDays(7),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            return Unauthorized();
        }
    }
}
