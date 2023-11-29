using Shop.Domain.Models.ProductEntities;
using Shop.Domain.ViewModels.Admin.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.Interfaces
{
    public interface IProductRepository
    {
        #region product - admin

        Task SaveChanges();

        Task<bool> CheckUrlNameCategories(string urlName);

        Task<bool> CheckUrlNameCategories(string urlName, long CategoryId);

        Task AddProductCtaegory(ProductCategory productCategory);

        Task<ProductCategory> GetProductCategoryById(long id);

        void UpdateProductCtaegory(ProductCategory category);

        Task<FilterProductCategoriesViewModel> FilterProductCategories(FilterProductCategoriesViewModel filter);

        #endregion
    }
}
