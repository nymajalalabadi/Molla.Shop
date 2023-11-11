using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;

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
            return View();
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
            return View();
        }
    }

    #endregion
}
