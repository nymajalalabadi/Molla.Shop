using Microsoft.AspNetCore.Mvc;
using Shop.Web.Models;
using System.Diagnostics;

namespace Shop.Web.Controllers
{
    public class HomeController : SiteBaseController
    {

        #region index
        public IActionResult Index()
        {
            TempData[ErrorMessage] = "123";
            return View();
        }

        #endregion

        public IActionResult Privacy()
        {
            return View();
        }

    }
}