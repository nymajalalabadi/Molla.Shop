using Shop.Domain.Models.Orders;
using Shop.Domain.ViewModels.Account;
using Shop.Domain.ViewModels.Admin.Orders;
using Shop.Domain.ViewModels.Admin.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Interfaces
{
    public interface IOrderService
    {
        Task<long> AddOrder(long userId, long productId);

        Task UpdatePriceOrder(long orderId);

        Task<Order> GetBasketForUser(long orderId, long userId);

        Task<Order> GetBasketForUser(long userId);

        Task<Order> GetOrderById(long OrderId);

        Task<FinallyOrderResult> FinallyOrder(FinallyOrderViewModel finallyOrder, long userId);

        Task<bool> RemoveOrderDetailFromOrder(long detailId);

        Task ChangeIsFilnalyToOrder(long orderId);

        Task<ResultOrderStateViewModel> GetResultOrder();

        Task<FilterOrdersViewModel> filterOrders(FilterOrdersViewModel filterOrdersViewModel);

        Task<bool> ChangeStateToSent(long orderId);

        Task<Order> GetOrderDetail(long orderId);
    }
}
