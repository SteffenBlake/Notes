using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Notes.Data.Models.Identity;
using Notes.Website.Models.Login;

namespace Notes.Website.Controllers
{
    /// <summary>
    /// Endpoints for Authentication 
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private SignInManager<NotesUser> SignInManager { get; }

        /// <summary>
        /// Endpoints for Authentication 
        /// </summary>
        public AuthenticationController(
            SignInManager<NotesUser> signInManager
        )
        {
            SignInManager = signInManager;
        }

        /// <summary>
        /// Logs in the current user via cookie based authorization
        /// </summary>
        [ProducesResponseType(typeof(LoginResponseModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm]LoginRequestModel request)
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

            var signInResult = await SignInManager.CheckPasswordSignInAsync(user, request.Password, true);
            if (!signInResult.Succeeded)
            {
                return Unauthorized();
            }

            var claims = new List<Claim>();
            claims.Add(new(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new(ClaimTypes.Name, user.UserName!));
            claims.Add(new(ClaimTypes.Email, user.Email!));

            var roles = await SignInManager.UserManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                principal, 
                new AuthenticationProperties
                {
                    IsPersistent = request.RememberMe
                }
            );

            return LocalRedirect(request.ReturnUrl ?? "/");
        }

        /// <summary>
        /// Logs out the currently signed in user
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(302)]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/login");
        }
    }
}
