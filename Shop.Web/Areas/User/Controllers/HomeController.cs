using Microsoft.AspNetCore.Mvc;

namespace Shop.Web.Areas.User.Controllers
{
    public class HomeController : UserBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
