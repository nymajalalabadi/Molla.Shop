using Shop.Domain.Models.ProductEntities;
using Shop.Domain.ViewModels.Pigging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Admin.Products
{
    public class FilterProductsViewModel : BasePaging
    {
        #region property

        public string ProductName { get; set; }

        public string FilterByCategory { get; set; }

        public List<Product> Products { get; set; }

        public ProductState ProductState { get; set; }

        public ProductOrder ProductOrder { get; set; }

        #endregion

        #region method

        public FilterProductsViewModel SetProducts(List<Product> products)
        {
            this.Products = products;
            return this;
        }

        public FilterProductsViewModel SetPaging(BasePaging paging)
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

    public enum ProductState
    {
        [Display(Name = "همه")]
        All,

        [Display(Name = "فعال")]
        IsActice,

        [Display(Name = "حذف شده")]
        Delete
    }
    public enum ProductOrder
    {
        [Display(Name = "همه")]
        All,

        [Display(Name = "جدیدترین ها")]
        ProductNewss,

        [Display(Name = "گران ترین ها")]
        ProductExp,

        [Display(Name = "ارزان ترین ها")]
        ProductInExprnsive
    }

}
