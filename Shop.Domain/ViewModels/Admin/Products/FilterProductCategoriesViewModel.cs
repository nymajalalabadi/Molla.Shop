using Shop.Domain.Models.ProductEntities;
using Shop.Domain.ViewModels.Pigging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Admin.Products
{
    public class FilterProductCategoriesViewModel : BasePaging
    {
        #region properties

        public string Title { get; set; }

        public List<ProductCategory> ProductCategories { get; set; }

        #endregion

        #region methods

        public FilterProductCategoriesViewModel SetProductCategories(List<ProductCategory> productCategories)
        {
            this.ProductCategories = productCategories;
            return this;
        }

        public FilterProductCategoriesViewModel SetPaging(BasePaging paging)
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
}
