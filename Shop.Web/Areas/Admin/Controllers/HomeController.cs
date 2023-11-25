using Microsoft.AspNetCore.Mvc;

namespace Shop.Web.Areas.Admin.Controllers
{
    public class HomeController : AdminBaseController
    {
        #region dashboard

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #endregion

    }
}
