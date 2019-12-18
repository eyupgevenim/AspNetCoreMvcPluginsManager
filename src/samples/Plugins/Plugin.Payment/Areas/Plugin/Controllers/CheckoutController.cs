using Microsoft.AspNetCore.Mvc;

namespace Plugin.Payment.Controllers
{
    [Area("Plugin")]
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
