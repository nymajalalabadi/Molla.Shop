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

        [HttpGet]
        public async Task<IActionResult> Index(FilterUserViewModel filterUser)
        {
            filterUser.TakeEntity = 1;
            return View(await _userService.filterUsers(filterUser));
        }

        #endregion

        #region edit user 

        [HttpGet("edituser/{userId}")]
        public async Task<IActionResult> EditUser(long userId)
        {
            var data = await _userService.GetEditUserFromAdmin(userId);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        [HttpPost("edituser/{userId}")]
        public async Task<IActionResult> EditUser(EditUserFromAdmin editUser)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.EditUserFromAdmin(editUser);

                switch (result)
                {
                    case EditUserFromAdminResult.NotFound:
                        TempData[WarningMessage] = "کاربری با مشخصات وارد شده یافت نشد";
                        break;

                    case EditUserFromAdminResult.Success:
                        TempData[SuccessMessage] = "ویراش کاربر با موفقیت انجام شد";

                        return RedirectToAction("Index");
                }
            }

            return View(editUser);
        }

        #endregion
    }
}
