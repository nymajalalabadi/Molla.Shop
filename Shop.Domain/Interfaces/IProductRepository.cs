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
        #region category - admin

        Task SaveChanges();

        Task<bool> CheckUrlNameCategories(string urlName);

        Task<bool> CheckUrlNameCategories(string urlName, long CategoryId);

        Task AddProductCtaegory(ProductCategory productCategory);

        Task<ProductCategory> GetProductCategoryById(long id);

        void UpdateProductCtaegory(ProductCategory category);

        Task<FilterProductCategoriesViewModel> FilterProductCategories(FilterProductCategoriesViewModel filter);

        Task<List<ProductCategory>> GetAllProductCategories();

        #endregion


        #region product
        
        Task<FilterProductsViewModel> FilterProducts(FilterProductsViewModel filter);

        Task<Product> GetProductById(long productId);

        Task AddProduct(Product product);

        Task RemoveProductSelectedCategories(long productId);

        Task AddProductSelectedCategories(List<long> productSelectedCategories, long productId);

        Task<List<long>> GetAllProductCategoriesId(long productId);

        void UpdateProduct(Product product);

        Task<bool> DeleteProduct(long productId);

        Task<bool> RecoverProduct(long productId);

        Task AddProductGalleries(List<ProductGalleries> productGalleries);

        Task<bool> CheckProduct(long productId);

        Task<List<ProductGalleriesViewModel>> ShowAllProductGalleries(long productId);

        Task<List<ProductGalleries>> GetAllProductGalleries(long productId);

        Task<ProductGalleries> GetProductGallery(long id);

        void UpdateProductGallery(ProductGalleries productGalleries);

        Task DeleteProductGallery(long id);

        Task AddProductFeatuers(ProductFeature feature);

        Task<List<ProductFeatuersViewModel>> ShowAllProductFeatuers(long productId);

        void UpdateProductFeature(ProductFeature feature);

        Task DeleteFeatuers(long id);

        #endregion
    }
}
