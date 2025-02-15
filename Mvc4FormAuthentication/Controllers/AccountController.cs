using Mvc4FormAuthentication.Models;
using System.Web.Mvc;
using System.Web.Security;

namespace Mvc4FormAuthentication.Controllers
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

            var isAuthenticated = IsAuthenticateUser(model.Username, model.Password);

            if (isAuthenticated)
            {
                // the AUTH cookie is set here
                FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);

                return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
            }

            ModelState.AddModelError("", "Invalid username or password.");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            // the AUTH cookie is removed here
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        private bool IsAuthenticateUser(string username, string password)
        {
            return true;
        }
    }
}