using Microsoft.AspNetCore.Mvc;

namespace Shop.Web.Areas.User.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
