using Shop.Application.Interfaces;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Orders;
using Shop.Domain.Models.Wallet;
using Shop.Domain.ViewModels.Account;
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

        private readonly IWalletRepository _walletRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IWalletRepository walletRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _walletRepository = walletRepository;

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

        public async Task<Order> GetBasketForUser(long orderId, long userId)
        {
            return await _orderRepository.GetBasketForUser(orderId, userId);
        }

        public async Task<FinallyOrderResult> FinallyOrder(FinallyOrderViewModel finallyOrder, long userId)
        {
            if (userId != finallyOrder.UserId)
            {
                return FinallyOrderResult.HasNotUser;
            }

            var order = await _orderRepository.GetOrderById(finallyOrder.OrderId, userId);
            
            if (order == null || order.IsFinaly == true)
            {
                return FinallyOrderResult.NotFound;
            }

            if (await _walletRepository.GetUserWalletAmount(userId) >= order.OrderSum)
            {
                order.IsFinaly = true;

                var wallet = new UserWallet()
                {
                    Amount = order.OrderSum,
                    Description = $"فاکتور شماره {order.Id}",
                    WalletType = WalletType.Bardasht,
                    UserId = userId
                };

                await _walletRepository.CreateWallet(wallet);

                _orderRepository.UpdateOrder(order);

                await _orderRepository.SaveChanges();

                return FinallyOrderResult.Suceess;
            }

            return FinallyOrderResult.Error;
        }

        #endregion
    }
}
