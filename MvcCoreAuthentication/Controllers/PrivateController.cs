using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcCoreAuthentication.Controllers
{
    public class PrivateController : Controller
    {
        private readonly ILogger<PrivateController> _logger;

        public PrivateController(ILogger<PrivateController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
