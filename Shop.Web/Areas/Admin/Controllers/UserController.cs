﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Shop.Application.Interfaces;
using Shop.Domain.ViewModels.Account;
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

        #region create user

        [HttpGet("createuser")]
        public async Task<IActionResult> CreateUser()
        {
            ViewData["Roles"] = await _userService.GetAllActiveRoles();
            return View();
        }

        [HttpPost("createuser")]
        public async Task<IActionResult> CreateUser(CreateUserFromAdmin createUser)
        {
            ViewData["Roles"] = await _userService.GetAllActiveRoles();

            if (ModelState.IsValid)
            {
                var result = await _userService.CreateUserForAdmin(createUser);

                switch (result)
                {
                    case CreateUserFromAdminResult.MobileExists:
                        TempData[ErrorMessage] = " شماره موبایل وارد شده قبلا در سایت ثبت شده است";
                        break;

                    case CreateUserFromAdminResult.Success:
                        TempData[SuccessMessage] = "ثبت نام شما به موفقیت انجام شد";

                        return RedirectToAction("Index");
                }
            }
            return View(createUser);
        }

        #endregion

        #region edit user 

        [HttpGet("edituser/{userId}")]
        public async Task<IActionResult> EditUser(long userId)
        {
            ViewData["Roles"] = await _userService.GetAllActiveRoles();

            var data = await _userService.GetEditUserFromAdmin(userId);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        [HttpPost("edituser/{userId}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserFromAdmin editUser)
        {
            ViewData["Roles"] = await _userService.GetAllActivePermission();

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

        #region filter roles

        [HttpGet]
        public async Task<IActionResult> FilterRoles(FilterRolesViewModel filterRoles)
        {
            return View(await _userService.filterRoles(filterRoles));
        }

        #endregion

        #region Create Role

        [HttpGet("createrole")]
        public async Task<IActionResult> CreateRole()
        {
            ViewData["Permissions"] = await _userService.GetAllActivePermission();
            return View();
        }

        [HttpPost("createrole"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateOrEditRoleViewModel createRole)
        {
            ViewData["Permissions"] = await _userService.GetAllActivePermission();

            if (ModelState.IsValid)
            {
                var result = await _userService.CreateOrEditRole(createRole);

                switch (result)
                {
                    case CreateOrEditRoleResult.NotFound:
                        break;

                    case CreateOrEditRoleResult.NotExistPermission:
                        TempData[WarningMessage] = "لطفا نقشی را انتخاب کنید";
                        break;

                    case CreateOrEditRoleResult.Success:
                        TempData[SuccessMessage] = "عملیات افزودن نقش با موفقیت انجام شد";

                        return RedirectToAction("FilterRoles");
                }

            }

            return View(createRole);
        }

        #endregion

        #region edit role

        [HttpGet("editrole/{roleId}")]
        public async Task<IActionResult> EditRole(long roleId)
        {
            ViewData["Permissions"] = await _userService.GetAllActivePermission();

            var role = await _userService.GetEditRoleById(roleId);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost("editrole/{roleId}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(CreateOrEditRoleViewModel editRole)
        {
            ViewData["Permissions"] = await _userService.GetAllActivePermission();

            if (ModelState.IsValid)
            {
                var result = await _userService.CreateOrEditRole(editRole);

                switch (result)
                {
                    case CreateOrEditRoleResult.NotFound:
                        TempData[WarningMessage] = "نقشی با مشخصات وارد شده یافت نشد";
                        break;

                    case CreateOrEditRoleResult.NotExistPermission:
                        TempData[WarningMessage] = "لطفا نقشی را انتخاب کنید";
                        break;

                    case CreateOrEditRoleResult.Success:
                        TempData[SuccessMessage] = "عملیات ویرایش نقش با موفقیت انجام شد";

                        return RedirectToAction("FilterRoles");
                }
            }

            return View(editRole);
        }

        #endregion
    }
}
