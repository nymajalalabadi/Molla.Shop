﻿using Microsoft.EntityFrameworkCore;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Orders;
using Shop.Domain.ViewModels.Admin.Orders;
using Shop.Domain.ViewModels.Pigging;
using Shop.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public async Task<Order> GetBasketForUser(long userId)
        {
            return await _context.Orders.Include(c => c.OrderDetails).ThenInclude(c => c.Product).AsQueryable()
                .Where(c => c.UserId == userId && c.OrderState == OrderState.Processing && !c.IsFinaly && !c.IsDelete)
                .FirstOrDefaultAsync();
        }

        public async Task<OrderDetail> GetOrderDetailById(long detailId)
        {
            return await _context.OrderDetails.AsQueryable()
                .SingleOrDefaultAsync(d => d.Id == detailId);
        }

        public async Task<ResultOrderStateViewModel> GetResultOrder()
        {
            return new ResultOrderStateViewModel()
            {
                CancelCount = await _context.Orders.AsQueryable().Where(o => o.OrderState == OrderState.Cancel 
                && o.CreateDate.Day == DateTime.Now.Day).CountAsync(),

                RequestCount = await _context.Orders.AsQueryable().Where(o => o.OrderState == OrderState.Requested).CountAsync(),

                ProcessingCount = await _context.Orders.AsQueryable().Where(o => o.OrderState == OrderState.Processing).CountAsync(),

                SentCount = await _context.Orders.AsQueryable().Where(o => o.OrderState == OrderState.Sent).CountAsync()
            };
        }

        public async Task<FilterOrdersViewModel> filterOrders(FilterOrdersViewModel filter)
        {
            var query = _context.Orders.Include(o => o.OrderDetails).Include(o => o.User).AsQueryable();

            #region filter

            if (filter.UserId.HasValue && filter.UserId != 0)
            {
                query = query.Where(o => o.UserId == filter.UserId);
            }

            #endregion

            #region state

            switch (filter.OrderStateFilter)
            {
                case OrderStateFilter.All:
                    break;
                case OrderStateFilter.Requested:
                    query = query.Where(c => c.OrderState == OrderState.Requested);
                    break;
                case OrderStateFilter.Processing:
                    query = query.Where(c => c.OrderState == OrderState.Processing);
                    break;
                case OrderStateFilter.Sent:
                    query = query.Where(c => c.OrderState == OrderState.Sent);
                    break;
                case OrderStateFilter.Cancel:
                    query = query.Where(c => c.OrderState == OrderState.Cancel);
                    break;
            }

            #endregion


            #region set paging

            var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.CountForShowAfterAndBefore);

            var allData = await query.Paging(pager).OrderByDescending(w => w.CreateDate).ToListAsync();

            #endregion

            return filter.SetPaging(pager).SetOrders(allData);
        }

        public async Task<Order> GetOrderDetail(long orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
                .AsQueryable()
                .Where(o => o.Id == orderId)
                .FirstOrDefaultAsync();
        }
        #endregion
    }
}
