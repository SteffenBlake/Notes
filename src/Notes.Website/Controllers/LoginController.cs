using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Notes.Business;
using Notes.Business.Configurations;
using Notes.Data.Models.Identity;
using Notes.Website.Models.Login;

namespace Notes.Website.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private NotesConfig Config { get; }
        private SignInManager<NotesUser> SignInManager { get; }
        private IUserClaimsPrincipalFactory<NotesUser> ClaimsPrincipal { get; }

        public LoginController(
            NotesConfig config, 
            SignInManager<NotesUser> signInManager,
            IUserClaimsPrincipalFactory<NotesUser> claimsPrincipal
        )
        {
            Config = config;
            SignInManager = signInManager;
            ClaimsPrincipal = claimsPrincipal;
        }

        [ProducesResponseType(typeof(LoginResponseModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [HttpPost, Route("login")]
        public async Task<IActionResult> Login(LoginRequestModel request)
        {
            if (
                string.IsNullOrEmpty(request.UserName) ||
                string.IsNullOrEmpty(request.Password)
            )
            {
                return BadRequest("Username and/or Password are required");
            }

            var user = await SignInManager.UserManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return Unauthorized();
            }
            if (!await SignInManager.CanSignInAsync(user))
            {
                return Unauthorized();
            }

            var result = await SignInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);

            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            var token = await CompileTokenAsync(user);

            return Ok(new LoginResponseModel
            {
                access_token = token,
                expires_in = Config.Login.Expiry.Ticks
            });
        }

        [Authorize]
        [HttpPost, Route("logout")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return Ok();
        }

        private async Task<string> CompileTokenAsync(NotesUser user)
        {
            var secretUTF = Encoding.UTF8.GetBytes(Config.Signing.JWTSecret);
            var secretKey = new SymmetricSecurityKey(secretUTF);
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var principal = await ClaimsPrincipal.CreateAsync(user);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: NotesConstants.JWT_ISSUER,
                audience: Config.Urls,
                claims: principal.Claims,
                expires: DateTime.Now + Config.Login.Expiry,
                signingCredentials: signinCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }
    }
}
