using Shop.Domain.Models.Orders;
using Shop.Domain.ViewModels.Account;
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

        Task<FinallyOrderResult> FinallyOrder(FinallyOrderViewModel finallyOrder, long userId);

        Task<bool> RemoveOrderDetailFromOrder(long detailId);

    }
}
