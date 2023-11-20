using Microsoft.AspNetCore.Mvc;

namespace Shop.Web.Areas.Admin.Controllers
{
    public class HomeController : AdminBaseController
    {
        #region dashboard

        public IActionResult Index()
        {
            return View();
        }

        #endregion

    }
}
