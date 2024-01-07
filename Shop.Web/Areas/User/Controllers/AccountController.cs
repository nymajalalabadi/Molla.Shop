using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Domain.ViewModels.Account;
using Shop.Domain.ViewModels.Admin.Orders;
using Shop.Domain.ViewModels.Wallet;
using Shop.Web.Extentions;
using ZarinpalSandbox;

namespace Shop.Web.Areas.User.Controllers
{
    public class AccountController : UserBaseController
    {
        #region constractor

        private readonly IUserService _userService;
        private readonly IWalletService _walletService;
        private readonly IConfiguration _configuration;
        private readonly IOrderService _orderService;

        public AccountController(IUserService userService, IWalletService walletService, IConfiguration configuration, IOrderService orderService)
        {
            _userService = userService;
            _walletService = walletService;
            _configuration = configuration;
            _orderService = orderService;
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

        [HttpPost("edit-user-profile"),ValidateAntiForgeryToken]
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

        #region Change Password

        [HttpGet("change-password")]
        public async Task<IActionResult> ChangePassword() 
        {
            return View();
        }

        [HttpPost("change-password"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePassword)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ChangePassword(User.GetUserId(), changePassword);

                switch (result)
                {
                    case ChangePasswordResult.NotFound:
                        TempData[WarningMessage] = "کاربری با مشخصات وارد شده یافت نشد";
                        break;

                    case ChangePasswordResult.Failed:
                        TempData[ErrorMessage] = "لطفا رمز عبور قبلی خود را درست وارد بفرمایید";
                        break;

                    case ChangePasswordResult.PasswordEqual:
                        TempData[InfoMessage] = "لطفا از کلمه عبور جدیدی استفاده کنید";
                        ModelState.AddModelError("NewPassword", "لطفا از کلمه عبور جدیدی استفاده کنید");
                        break;

                    case ChangePasswordResult.Success:
                        TempData[SuccessMessage] = "کلمه ی عبور شما با موفقیت تغیر یافت";
                        TempData[InfoMessage] = "لطفا جهت تکمیل فراید تغیر کلمه ی عبور ،مجددا وارد سایت شود";

                        await HttpContext.SignOutAsync();

                        return RedirectToAction("Login", "Account", new { area = "" });
                }
            }
            return View(changePassword);
        }

        #endregion

        #region charge wallet

        [HttpGet("charge-wallet")]
        public async Task<IActionResult> ChargeWallet()
        {
            return View();
        }

        [HttpPost("charge-wallet"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ChargeWallet(ChargeWalletViewModel chargeWallet)
        {
            if (ModelState.IsValid)
            {
                var walletId = await _walletService.ChargeWallet(User.GetUserId(), chargeWallet, $"شارژ به مبلغ {chargeWallet.Amount}");

                #region payment

                var payment = new Payment(chargeWallet.Amount);

                var url = _configuration.GetSection("DefaultUrl")["Host"] + "/user/online-payment/" + walletId;

                var result = payment.PaymentRequest("شارژ کیف پول", url);

                if (result.Result.Status == 100)
                {
                    return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + result.Result.Authority);
                }
                else
                {
                    TempData[ErrorMessage] = "مشکلی در پرداخت به وجود آماده است،لطفا مجددا امتحان کنید";
                }

                #endregion
            }
            return View(chargeWallet);
        }

        #endregion

        #region online payment

        [HttpGet("online-payment/{id}")]
        public async Task<IActionResult> OnlinePayment(long id)
        {
            if (HttpContext.Request.Query["Status"] != "" && HttpContext.Request.Query["Status"].ToString().ToLower() == "ok" && HttpContext.Request.Query["Authority"] != "")
            {
                string authority = HttpContext.Request.Query["Authority"];

                var wallet = await _walletService.GetUserWalletById(id);

                if (wallet != null)
                {
                    var payment = new Payment(wallet.Amount);
                    var result = payment.Verification(authority).Result;

                    if (result.Status == 100)
                    {
                        ViewBag.RefId = result.RefId;
                        ViewBag.Success = true;

                        await _walletService.UpdateWalletForCharge(wallet);
                    }

                    return View();
                }

                return NotFound();
            }

            return View();
        }
        #endregion

        #region user wallet

        [HttpGet("user-wallet")]
        public async Task<IActionResult> UserWallet(FilterWalletViewModel filter)
        {
            filter.UserId = User.GetUserId();
            return View(await _walletService.FilterWallets(filter));
        }

        #endregion


        #region user-basket

        [HttpGet("Basket/{orderId}")]
        public async Task<IActionResult> UserBasket(long orderId)
        {
            var order = await _orderService.GetBasketForUser(orderId, User.GetUserId());
            if (order == null)
            {
                return NotFound();
            }
            ViewBag.UserWalletAmount = await _walletService.GetUserWalletAmount(User.GetUserId());

            return View(order);
        }

        [HttpPost("Basket/{orderId}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UserBasket(FinallyOrderViewModel finallyOrder)
        {
            if (finallyOrder.IsWallet)
            {
                var result = await _orderService.FinallyOrder(finallyOrder, User.GetUserId());

                switch (result)
                {
                    case FinallyOrderResult.HasNotUser:
                        TempData[ErrorMessage] = "سفارش شما یافت نشد";
                        break;

                    case FinallyOrderResult.NotFound:
                        TempData[ErrorMessage] = "سفارش شما یافت نشد";
                        break;

                    case FinallyOrderResult.Error:
                        TempData[ErrorMessage] = "موجودی کیف پول شما کافی نمیباشد";

                        return RedirectToAction("UserWallet");

                    case FinallyOrderResult.Suceess:
                        TempData[SuccessMessage] = "فاکتور شما با موفقیت پرداخت شد از خرید متشکریم";

                        return RedirectToAction("UserWallet");
                }
            }
            else
            {
                var order = await _orderService.GetOrderById(finallyOrder.OrderId);

                #region payment

                var payment = new Payment(order.OrderSum);

                var url = _configuration.GetSection("DefaultUrl")["Host"] + "/user/online-order/" + order.Id;

                var result = payment.PaymentRequest("شارژ کیف پول", url);

                if (result.Result.Status == 100)
                {
                    return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + result.Result.Authority);
                }
                else
                {
                    TempData[ErrorMessage] = "مشکلی در پرداخت به وجود آماده است،لطفا مجددا امتحان کنید";
                }

                #endregion
            }

            ViewBag.UserWalletAmount = await _walletService.GetUserWalletAmount(User.GetUserId());

            return View();
        }

        #endregion


        #region online order

        [HttpGet("online-order/{id}")]
        public async Task<IActionResult> OrderPeyment(long id)
        {
            if (HttpContext.Request.Query["Status"] != "" && HttpContext.Request.Query["Status"].ToString().ToLower() == "ok" && HttpContext.Request.Query["Authority"] != "")
            {
                string authority = HttpContext.Request.Query["Authority"];

                var order = await _orderService.GetOrderById(id);

                if (order != null)
                {
                    var payment = new Payment(order.OrderSum);
                    var result = payment.Verification(authority).Result;

                    if (result.Status == 100)
                    {
                        ViewBag.RefId = result.RefId;
                        ViewBag.Success = true;

                        await _orderService.ChangeIsFilnalyToOrder(order.Id);
                    }

                    return View();
                }

                return NotFound();
            }

            return View();
        }

        #endregion


        #region delete-order-detail

        [HttpGet("delete-orderDetail/{orderDetailId}")]
        public async Task<IActionResult> DeleteOrderDetail(long orderDetailId)
        {
            var result = await _orderService.RemoveOrderDetailFromOrder(orderDetailId);

            if (result)
            {
                return JsonResponseStatus.Success();
            }
            return JsonResponseStatus.Error();
        }

        #endregion

        #region reload-price

        [HttpGet("reload-price")]
        public async Task<IActionResult> ReloadOrderPrice(long id)
        {
            var order = await _orderService.GetBasketForUser(id, User.GetUserId());

            ViewBag.UserWalletAmount = await _walletService.GetUserWalletAmount(User.GetUserId());

            return PartialView("_OrderPrice", order);
        }

        #endregion

        #region user - orders

        [HttpGet("user-orders")]
        public async Task<IActionResult> UserOrders(FilterOrdersViewModel filter)
        {
            filter.UserId = User.GetUserId();

            var data = await _orderService.filterOrders(filter);

            return View(data);
        }

        #endregion

        #region OrderDetail

        [HttpGet("orderdetail/{orderId}")]
        public async Task<IActionResult> OrderDetail(long orderId)
        {
            var data = await _orderService.GetOrderDetail(orderId);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        #endregion

        #region user-favorite

        [HttpGet("add-favorit/{productId}")]
        public async Task<IActionResult> AddUserFavorit(long productId)
        {
            var result = await _userService.AddProductToFavorite(User.GetUserId(), productId);

            if (result)
            {
                TempData[SuccessMessage] = "محصول مورد نظر با موفقیت در قسمت علاقه مندی اضافه شد";
                return RedirectToAction("UserFavorits");
            }

            TempData[WarningMessage] = "محصول مورد نظر قبلا در علاقه مندی اضافه شده است";
            return RedirectToAction("UserFavorits");
        }

        #endregion

        #region user-compatre

        [HttpGet("add-compare/{productId}")]
        public async Task<IActionResult> AddUserCompare(long productId)
        {
            var result = await _userService.AddProductToCompare(User.GetUserId(), productId);

            if (result)
            {
                TempData[SuccessMessage] = "محصول مورد نظر با موفقیت در قسمت مقایسه اضافه شد";
                return RedirectToAction("UserCompares");
            }

            TempData[WarningMessage] = "محصول مورد نظر قبلا در مقایسه اضافه شده است";
            return RedirectToAction("UserCompares");
        }

        #endregion

        #region remove-user-favorit

        [HttpGet("removeuserfavorit/{productId}")]
        public async Task<IActionResult> RemoveUserFavorit(long productId)
        {
            var result = await _userService.RemoveUserFavorit(User.GetUserId(), productId);

            if (result)
            {
                TempData[SuccessMessage] = "محصول مورد نظر که در لیست علاقه مندی ها بود حذف شده";
                return RedirectToAction("UserFavorits");
            }

            TempData[WarningMessage] = "همچین محصولی در لیست علاقه مندی ها شما وجود ندارد";
            return RedirectToAction("UserFavorits");

        }

        #endregion

        #region all-remove-usercompare

        [HttpGet("removeAllUserCompare")]
        public async Task<IActionResult> RemoveAllUserCompare()
        {
            var result = await _userService.RemoveAllUserComapre(User.GetUserId());

            if (result)
            {
                TempData[SuccessMessage] = "تمامی محصولاتی که در لیست مقایسه بود حذف شده";
                return RedirectToAction("UserCompares");

            }

            TempData[WarningMessage] = "لیست مقایسه شما خالی میباشد";
            return RedirectToAction("UserCompares");
        }

        #endregion

        #region remove-userCompare

        [HttpGet("removeUserCompare/{productId}")]
        public async Task<IActionResult> RemoveUserCompare(long productId)
        {
            var result = await _userService.RemoveUserComapre(User.GetUserId(), productId);

            if (result)
            {
                TempData[SuccessMessage] = "محصول مورد نظر که در لیست مقایسه بود حذف شده";
                return RedirectToAction("UserCompares");
            }

            TempData[WarningMessage] = "همچین محصولی در لیست مقایسه ی شما وجود ندارد";
            return RedirectToAction("UserCompares");

        }

        #endregion

        #region list-user-favorits

        [HttpGet("user-favorits")]
        public async Task<IActionResult> UserFavorits(UserFavoritsViewModel filter)
        {
            return View(await _userService.UserFavorits(filter));
        }

        #endregion

        #region list-user-compares

        [HttpGet("user-compares")]
        public async Task<IActionResult> UserCompares(UserComparesViewModel filter)
        {
            return View(await _userService.UserCompares(filter));
        }

        #endregion

    }
}
