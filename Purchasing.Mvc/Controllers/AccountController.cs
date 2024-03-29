﻿using Microsoft.AspNetCore.Authorization;
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

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// Controller for the Account class.
    /// </summary>
    public class AccountController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IRepositoryWithTypedId<User, string> _userRepository;

        public string Message
        {
            set { TempData["Message"] = value; }
        }

        public AccountController(IRepositoryWithTypedId<User,string> userRepository)
        {
            _userRepository = userRepository;
        }
        

        [Route("LogOut")]
        public async Task<ActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [Route("LogOn")]
        public async Task LogOn(string returnUrl)
        {
            var props = new AuthenticationProperties { RedirectUri = returnUrl };
            await HttpContext.ChallengeAsync(CasDefaults.AuthenticationScheme, props);
        }

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