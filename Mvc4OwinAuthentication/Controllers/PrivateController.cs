using System.Web.Mvc;

namespace Mvc4OwinAuthentication.Controllers
{
    public class PrivateController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}