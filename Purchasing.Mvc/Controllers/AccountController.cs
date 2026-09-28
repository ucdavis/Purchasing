using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AspNetCore.Security.CAS;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using UCDArch.Core.PersistanceSupport;
using NHibernate.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Purchasing.Mvc.Models;

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// Controller for the Account class.
    /// </summary>
    public class AccountController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly bool _localLoginEnabled;

        public string Message
        {
            set { TempData["Message"] = value; }
        }

        public AccountController(IRepositoryWithTypedId<User,string> userRepository,
            IWebHostEnvironment environment, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _localLoginEnabled = environment.IsDevelopment() && configuration.GetValue<bool>("LocalLogin:Enabled");
        }
        

        [Route("LogOut")]
        public async Task<ActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet("LogOn")]
        public ActionResult LogOn(string returnUrl, bool useCas = false)
        {
            var redirectUrl = LocalReturnUrl(returnUrl);
            if (_localLoginEnabled && !useCas)
            {
                ModelState.Remove(nameof(LocalLoginModel.ReturnUrl));
                return View(new LocalLoginModel { ReturnUrl = redirectUrl });
            }

            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, CasDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpPost("LogOn/Local")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LocalLogOn(LocalLoginModel model)
        {
            if (!_localLoginEnabled) return NotFound();

            model.ReturnUrl = LocalReturnUrl(model.ReturnUrl);
            // Render the sanitized destination even when redisplaying a failed POST.
            ModelState.Remove(nameof(model.ReturnUrl));
            if (!ModelState.IsValid) return View("LogOn", model);

            var userId = model.UserId?.Trim().ToLowerInvariant();
            var user = string.IsNullOrEmpty(userId) ? null : _userRepository.GetNullableById(userId);
            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(nameof(model.UserId), "Enter the login ID of an existing, active user.");
                return View("LogOn", model);
            }

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
                new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return LocalRedirect(model.ReturnUrl);
        }

        private string LocalReturnUrl(string returnUrl) => Url.IsLocalUrl(returnUrl) ? returnUrl : "/Home/Landing";

        /// <summary>
        /// Emulate a specific user, for Emulation Users only
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Policy = Role.Codes.EmulationUser)]
        public async Task<ActionResult> Emulate(string id /* Login ID*/)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var user = await _userRepository.Queryable.SingleOrDefaultAsync(x => x.Id == id);
                if (user == null)
                {
                    var identity2 = new ClaimsIdentity(new[]
{
                    new Claim(ClaimTypes.NameIdentifier, id),
                    new Claim(ClaimTypes.Name, id),
                    new Claim(ClaimTypes.GivenName, "Fake"),
                    new Claim(ClaimTypes.Surname, "FakeLn"),
                    new Claim(ClaimTypes.Email, "Fake@fake.com")
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                    // kill old login
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    // create new login
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity2));
                    return RedirectToAction("Index", "Home");
                }

                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Id),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName),
                    new Claim(ClaimTypes.Email, user.Email)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                // kill old login
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // create new login
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                Message = "Login ID not provided.  Use /Emulate/login";
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Just a signout, without the hassle of signing out of CAS.  Ends emulated credentials.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> EndEmulate()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}