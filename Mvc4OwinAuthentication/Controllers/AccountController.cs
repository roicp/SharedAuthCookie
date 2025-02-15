using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Mvc4OwinAuthentication.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Mvc4OwinAuthentication.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = AuthenticateUser(model.Username, model.Password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType);

                var authManager = HttpContext.GetOwinContext().Authentication;

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                };

                authManager.SignIn(authProperties, claimsIdentity);

                return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return RedirectToAction("Index", "Home");
        }

        private User AuthenticateUser(string username, string password)
        {
            return new User(username);
        }
    }
}