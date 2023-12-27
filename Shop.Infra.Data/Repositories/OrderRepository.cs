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
                .FirstOrDefaultAsync(o => o.UserId == userId);
        }

        public async Task<OrderDetail> CheckOrderDetail(long orderId, long productId)
        {
            return await _context.OrderDetails.AsQueryable()
                .Where(d => d.OrderId == orderId && d.ProductId == productId)
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
                .Where(d => d.Id == OrderId && !d.IsDelete)
                .SumAsync(d => d.Price * d.Count);
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
                .Where(o => o.Id == orderId && o.UserId == userId)
                .Select(o => new Order()
                {
                    UserId = o.UserId,
                    CreateDate = o.CreateDate,
                    Id = o.Id,
                    IsDelete = o.IsDelete,
                    IsFinaly = o.IsFinaly,
                    OrderSum = o.OrderSum,
                    OrderDetails = _context.OrderDetails.Include(c => c.Product).Where(d => !d.IsDelete && !d.Order.IsFinaly && d.Order.UserId == userId)
                    .ToList()
                })
                .FirstOrDefaultAsync();
        }


        #endregion
    }
}
