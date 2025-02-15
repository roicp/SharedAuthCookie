using System.Web.Mvc;

namespace Mvc4FormAuthentication.Controllers
{
    public class InternalController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}