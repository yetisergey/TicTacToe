namespace TicTacToe.Controllers
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    [Route("api/[controller]"), ApiController]
    public class LoginController : Controller
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            if (User.Identity.IsAuthenticated && int.TryParse(User.Identity.Name, out var userId))
            {
                var userName = await _userService.GetUserName(userId);
                return Ok(userName);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]string email)
        {
            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                return BadRequest();
            }

            if (!User.Identity.IsAuthenticated)
            {
                var userId = await _userService.AddUser(email);
                await Authenticate(userId);
            }

            return Ok();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async Task Authenticate(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        private async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}