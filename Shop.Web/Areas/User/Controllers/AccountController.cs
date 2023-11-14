using Microsoft.AspNetCore.Mvc;
using Shop.Application.Extentions;
using Shop.Application.Interfaces;
using Shop.Application.Utils;
using Shop.Domain.Models.Account;
using Shop.Domain.ViewModels.Account;
using Shop.Web.Extentions;

namespace Shop.Web.Areas.User.Controllers
{
    public class AccountController : UserBaseController
    {
        #region constractor

        private readonly IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        #endregion


        #region edit user profile

        [HttpGet("edit-user-profile")]
        public async Task<IActionResult> EditUserProfile()
        {
            var user = await _userService.GetEditUserProfile(User.GetUserId());
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost("edit-user-profile")]
        public async Task<IActionResult> EditUserProfile(EditUserProfileViewModel editUserProfile, IFormFile? userAvatar)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.EditProfile(User.GetUserId(), userAvatar, editUserProfile);
                switch (result)
                {
                    case EditUserProfileResult.NotFound:
                        TempData[WarningMessage] = "کاربری با مشخصات وارد شده یافت نشد";
                        break;
                    case EditUserProfileResult.Success:
                        TempData[SuccessMessage] = "عملیات ویرایش حساب کاربری با موفقیت انجام شد";

                        return RedirectToAction("Index" ,"Home");
                }
            }
            return View(editUserProfile);
        }


        #endregion
    }
}
