using Shop.Domain.Models.Account;
using Shop.Domain.Models.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.Models.Orders
{
    public class Order : BaseEntity
    {
        #region properties

        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long UserId { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int OrderSum { get; set; }

        public bool IsFinaly { get; set; }

        public OrderState OrderState { get; set; }

        #endregion


        #region relations

        public User User { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }

        #endregion
    }

    public enum OrderState
    {
        Requested,
        Processing,
        Sent,
        Cancel
    }


}
