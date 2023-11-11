using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;

namespace Shop.Web.ViewComponents
{
    #region siteHeader

    public class SiteHeaderViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        public SiteHeaderViewComponent(IUserService userService)
        {
                _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.User = await _userService.GetUserByPhoneNumber(User.Identity.Name);
            }
            return View("SiteHeader");
        }
    }

    #endregion

    #region siteFooter

    public class SiteFooterViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("SiteFooter");
        }
    }

    #endregion
}
