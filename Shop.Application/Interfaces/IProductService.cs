using Microsoft.AspNetCore.Http;
using Shop.Domain.Models.ProductEntities;
using Shop.Domain.ViewModels.Admin.Products;
using Shop.Domain.ViewModels.Site.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Interfaces
{
    public interface IProductService
    {
        #region category - admin

        Task<CreateProductCategoryResult> CreatePrdouctCategory(CreateProductCategoryViewModel createProductCategory, IFormFile image);

        Task<EditProductCategoryViewModel> GetEditPrdouctCategory(long categoryId);

        Task<EditProductCategoryResult> EditPrdouctCategory(EditProductCategoryViewModel editProductCategoryViewModel, IFormFile image);

        Task<FilterProductCategoriesViewModel> FilterProductCategories(FilterProductCategoriesViewModel filterProductCategoriesViewModel);

        Task<List<ProductCategory>> GetAllProductCategories();

        #endregion

        #region product

        Task<FilterProductsViewModel> FilterProducts(FilterProductsViewModel filter);

        Task<CreateProductResult> CreateProduct(CreateProductViewModel createProduct, IFormFile imageProduct);

        Task<EditProductViewModel> GetEditProduct(long productId);

        Task<EditProductResult> EditProduct(EditProductViewModel editProduct , IFormFile ProductImage);

        Task<bool> DeleteProduct(long productId);

        Task<bool> RecoverProduct(long productId);

        Task<bool> AddProductGallery(long productId, List<IFormFile> images);

        Task<List<ProductGalleriesViewModel>> ShowAllProductGalleries(long productId);

        Task<List<ProductGalleries>> GetAllProductGalleries(long productId);

        Task DeleteImage(long galleryId);

        Task<CreateProductFeatuersResult> CreateProductFeatuers(CreateProductFeatuersViewModel createProductFetuers);

        Task<List<ProductFeatuersViewModel>> ShowAllProductFeatuers(long productId);

        Task DeleteFeatuers(long id);

        Task<List<ProductItemViewModel>> ShowAllProductInSlider();

        Task<List<ProductItemViewModel>> ShowAllProductInCategory(string hrefName);

        Task<List<ProductItemViewModel>> LastProducts();

        #endregion
    }
}
