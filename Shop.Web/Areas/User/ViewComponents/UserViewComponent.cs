using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Web.Extentions;

namespace Shop.Web.Areas.User.ViewComponents
{
    #region UserSideBar

    public class UserSideBarViewComponent : ViewComponent
    {
        #region constrator

        private readonly IUserService _userService;

        public UserSideBarViewComponent(IUserService userService)
        {
               _userService = userService;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userService.GetUserById(User.GetUserId());

                return View("UserSideBar", user);
            }
            return View("UserSideBar");
        }
    }

    #endregion

    #region UserInformation

    public class UserInformationViewComponent : ViewComponent
    {
        #region constrator

        private readonly IUserService _userService;

        public UserInformationViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userService.GetUserById(User.GetUserId());

                return View("UserInformation", user);
            }

            return View("UserInformation");

        }
    }

    #endregion
}
