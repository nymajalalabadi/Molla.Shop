using Shop.Domain.Models.Orders;
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

        Task<int> OrderSum(long OrderId);

        Task AddOrder(Order order);

        Task AddOrderDetail(OrderDetail orderDetail);

        Task SaveChanges();

        void UpdateOrder(Order order);

        void UpdateOrderDetail(OrderDetail orderDetail);

        Task<Order> GetBasketForUser(long orderId, long userId);
    }
}
