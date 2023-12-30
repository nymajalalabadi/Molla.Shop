using Microsoft.EntityFrameworkCore;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Orders;
using Shop.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Infra.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        #region constractore

        private readonly ShopDbContext _context;

        public OrderRepository(ShopDbContext context)
        {
            _context = context;
        }

        #endregion

        #region order

        public async Task<Order> CheckUserOrder(long userId)
        {
            return await _context.Orders.AsQueryable()
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.UserId == userId && !o.IsFinaly);
        }

        public async Task<OrderDetail> CheckOrderDetail(long orderId, long productId)
        {
            return await _context.OrderDetails.AsQueryable()
                .Where(c => c.OrderId == orderId && c.ProductId == productId && !c.IsDelete)
                .FirstOrDefaultAsync();
        }

        public async Task<Order> GetOrderById(long OrderId)
        {
            return await _context.Orders
                .AsQueryable()
                .SingleOrDefaultAsync(o => o.Id == OrderId);
        }

        public async Task<Order> GetOrderById(long OrderId, long userId)
        {
            return await _context.Orders
                .AsQueryable()
                .SingleOrDefaultAsync(o => o.Id == OrderId && o.UserId == userId);
        }

        public async Task<int> OrderSum(long OrderId)
        {
            return await _context.OrderDetails.AsQueryable()
                .Where(c => c.OrderId == OrderId && !c.IsDelete).SumAsync(c => c.Price * c.Count);
        }

        public async Task AddOrder(Order order)
        {
            await _context.Orders.AddAsync(order);  
        }

        public async Task AddOrderDetail(OrderDetail orderDetail)
        {
            await _context.OrderDetails.AddAsync(orderDetail);
        }


        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
        }


        public void UpdateOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
        }

        public async Task<Order> GetBasketForUser(long orderId, long userId)
        {
            return await _context.Orders.AsQueryable()
                .Include(o => o.User)
                .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
                .Where(o => o.Id == orderId && o.UserId == userId)
                .Select(o => new Order()
                {
                    UserId = o.UserId,
                    CreateDate = o.CreateDate,
                    Id = o.Id,
                    IsDelete = o.IsDelete,
                    IsFinaly = o.IsFinaly,
                    OrderSum = o.OrderSum,
                    OrderDetails = o.OrderDetails.Where(d => d.OrderId == orderId && !d.IsDelete && !d.Order.IsFinaly).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<OrderDetail> GetOrderDetailById(long detailId)
        {
            return await _context.OrderDetails.AsQueryable()
                .SingleOrDefaultAsync(d => d.Id == detailId);
        }

        #endregion
    }
}
