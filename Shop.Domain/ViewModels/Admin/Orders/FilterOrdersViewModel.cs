using Shop.Domain.Models.Orders;
using Shop.Domain.ViewModels.Pigging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Admin.Orders
{
    public class FilterOrdersViewModel : BasePaging
    {
        public long? UserId { get; set; }

        public OrderStateFilter? OrderStateFilter { get; set; }

        public List<Order> Orders { get; set; }


        #region methods

        public FilterOrdersViewModel SetOrders(List<Order> orders)
        {
            this.Orders = orders;
            return this;
        }

        public FilterOrdersViewModel SetPaging(BasePaging paging)
        {
            this.PageId = paging.PageId;
            this.AllEntityCount = paging.AllEntityCount;
            this.StartPage = paging.StartPage;
            this.EndPage = paging.EndPage;
            this.TakeEntity = paging.TakeEntity;
            this.CountForShowAfterAndBefore = paging.CountForShowAfterAndBefore;
            this.SkipEntity = paging.SkipEntity;
            this.PageCount = paging.PageCount;

            return this;
        }

        #endregion
    }

    public enum OrderStateFilter
    {
        [Display(Name = "همه")]
        All,
        [Display(Name = "درخواست شده")]
        Requested,
        [Display(Name = "در حال بررسی")]
        Processing,
        [Display(Name = "ارسال شده")]
        Sent,
        [Display(Name = "لغو شده")]
        Cancel
    }

}
