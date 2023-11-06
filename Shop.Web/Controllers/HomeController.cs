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
            return View();
        }

        #endregion

        public IActionResult Privacy()
        {
            return View();
        }

    }
}