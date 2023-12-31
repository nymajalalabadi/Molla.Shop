﻿using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Domain.ViewModels.Admin.Orders;
using Shop.Web.Extentions;

namespace Shop.Web.Areas.Admin.Controllers
{
    public class OrderController : AdminBaseController
    {
        #region constractor

        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        #endregion

        #region filter-order

        [HttpGet]
        public async Task<IActionResult> FilterOrder(FilterOrdersViewModel filter)
        {
            filter.TakeEntity = 15;
            return View(await _orderService.filterOrders(filter));
        }

        #endregion

        #region Change-order-state

        [HttpGet("ChangeStateToSent/{orderId}")]
        public async Task<IActionResult> ChangeStateToSent(long orderId)
        {
            var result = await _orderService.ChangeStateToSent(orderId);

            if (result)
            {
                return JsonResponseStatus.Success();
            }

            return JsonResponseStatus.Error();
        }

        #endregion

        #region OrderDetail
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

    }
}
