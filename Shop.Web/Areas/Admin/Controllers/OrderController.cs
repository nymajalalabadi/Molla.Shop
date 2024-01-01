using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Domain.ViewModels.Admin.Orders;

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

        public async Task<IActionResult> FilterOrder(FilterOrdersViewModel filter)
        {
            return View(await _orderService.filterOrders(filter));
        }

        #endregion
    }
}
