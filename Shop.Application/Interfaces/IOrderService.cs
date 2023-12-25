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
    }
}
