using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Domain.ViewModels.Admin.Account;

namespace Shop.Web.Areas.Admin.Controllers
{
    public class UserController : AdminBaseController
    {
        #region constractor

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
               _userService = userService;
        }

        #endregion

        #region filter users

        public async Task<IActionResult> Index(FilterUserViewModel filterUser)
        {
            return View(await _userService.filterUsers(filterUser));
        }

        #endregion
    }
}
