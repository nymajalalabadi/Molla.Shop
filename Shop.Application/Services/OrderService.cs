using Shop.Application.Interfaces;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Services
{
    public class OrderService : IOrderService
    {
        #region constractore

        private readonly IOrderRepository _orderRepository
            ;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;

        }
        #endregion

        #region order

        public async Task<long> AddOrder(long userId, long productId)
        {
            var product = await _productRepository.GetProductById(productId);

            var order = await _orderRepository.CheckUserOrder(userId);

            if (order == null)
            {
                //add order

                order = new Order()
                {
                    UserId = userId,
                    IsFinaly = false,
                    OrderSum = product.Price,
                    OrderState = OrderState.Processing,
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail()
                        {
                            ProductId = productId,
                            Price = product.Price,
                            Count = 1,
                        }
                    }
                };

                await _orderRepository.AddOrder(order);
                await _orderRepository.SaveChanges();
            }
            else
            {
                var orderDetail = await _orderRepository.CheckOrderDetail(order.Id, product.Id);

                if (orderDetail != null)
                {
                    orderDetail.Count += 1;

                    _orderRepository.UpdateOrderDetail(orderDetail);
                   //await _orderRepository.SaveChanges();
                }
                else
                {
                    orderDetail = new OrderDetail()
                    {
                        OrderId = order.Id,
                        ProductId = productId,
                        Price = product.Price,
                        Count = 1,
                    };

                    await _orderRepository.AddOrderDetail(orderDetail);
                    // _orderRepository.SaveChanges();
                }
                await _orderRepository.SaveChanges();
            }

            await UpdatePriceOrder(order.Id);

            return order.Id;
        }

        public async Task UpdatePriceOrder(long orderId)
        {
            var order = await _orderRepository.GetOrderById(orderId);

            order.OrderSum = await _orderRepository.OrderSum(orderId);

            _orderRepository.UpdateOrder(order);
            await _orderRepository.SaveChanges();
        }


        #endregion
    }
}
