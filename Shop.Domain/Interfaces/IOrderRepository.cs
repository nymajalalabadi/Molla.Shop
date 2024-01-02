using Shop.Domain.Models.Orders;
using Shop.Domain.ViewModels.Admin.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CheckUserOrder(long userId);

        Task<OrderDetail> CheckOrderDetail(long orderId, long productId);

        Task<Order> GetOrderById(long OrderId);

        Task<Order> GetOrderById(long OrderId, long userId);

        Task<int> OrderSum(long OrderId);

        Task AddOrder(Order order);

        Task AddOrderDetail(OrderDetail orderDetail);

        Task SaveChanges();

        void UpdateOrder(Order order);

        void UpdateOrderDetail(OrderDetail orderDetail);

        Task<Order> GetBasketForUser(long orderId, long userId);

        Task<Order> GetBasketForUser(long userId);
        
        Task<OrderDetail> GetOrderDetailById(long detailId);

        Task<ResultOrderStateViewModel> GetResultOrder();

        Task<FilterOrdersViewModel> filterOrders(FilterOrdersViewModel filterOrdersViewModel);

        Task<Order> GetOrderDetail(long orderId);
        
    }
}
