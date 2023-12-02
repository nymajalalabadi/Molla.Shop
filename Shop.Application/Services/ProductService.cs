using Microsoft.AspNetCore.Http;
using Shop.Application.Extentions;
using Shop.Application.Interfaces;
using Shop.Application.Utils;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.ProductEntities;
using Shop.Domain.ViewModels.Admin.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Services
{
    public class ProductService : IProductService
    {
        #region constractor

        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        #endregion

        #region category - admin

        public async Task<CreateProductCategoryResult> CreatePrdouctCategory(CreateProductCategoryViewModel createProductCategory, IFormFile image)
        {
            if (await _productRepository.CheckUrlNameCategories(createProductCategory.UrlName))
                return CreateProductCategoryResult.IsExistUrlName;

            var newProductCategory = new ProductCategory()
            {
                Title = createProductCategory.Title,
                UrlName = createProductCategory.UrlName,
                ParentId = null,
                IsDelete = false
            };

            if (image != null && image.IsImage())
            {
                var nameImage = Guid.NewGuid().ToString("N") + Path.GetExtension(image.FileName);

                image.AddImageToServer(nameImage, PathExtensions.CategoryOrginServer, 150, 150, PathExtensions.CategoryThumbServer);

                newProductCategory.ImageName = nameImage;
            }

            await _productRepository.AddProductCtaegory(newProductCategory);
            await _productRepository.SaveChanges();

            return CreateProductCategoryResult.Success;
        }

        public async Task<EditProductCategoryViewModel> GetEditPrdouctCategory(long categoryId)
        {
            var categoryProduct = await _productRepository.GetProductCategoryById(categoryId);

            if (categoryProduct == null)
            {
                return null;
            }

            return new EditProductCategoryViewModel()
            {
                ProductCategoryId = categoryProduct.Id,
                Title = categoryProduct.Title,
                ImageName = categoryProduct.ImageName,
                UrlName = categoryProduct.UrlName
            };
        }

        public async Task<EditProductCategoryResult> EditPrdouctCategory(EditProductCategoryViewModel editProductCategoryViewModel, IFormFile image)
        {
            var productCategory = await _productRepository.GetProductCategoryById(editProductCategoryViewModel.ProductCategoryId);

            if (productCategory == null)
                return EditProductCategoryResult.NotFound;

            if (await _productRepository.CheckUrlNameCategories(editProductCategoryViewModel.UrlName, editProductCategoryViewModel.ProductCategoryId))
                return EditProductCategoryResult.IsExistUrlName;

            productCategory.Title = editProductCategoryViewModel.Title;
            productCategory.UrlName = editProductCategoryViewModel.UrlName;

            if (image != null && image.IsImage())
            {
                var nameImage = Guid.NewGuid().ToString("N") + Path.GetExtension(image.FileName);

                image.AddImageToServer(nameImage, PathExtensions.CategoryOrginServer, 150, 150, PathExtensions.CategoryThumbServer, productCategory.ImageName);

                productCategory.ImageName = nameImage;
            }

            _productRepository.UpdateProductCtaegory(productCategory);
            await _productRepository.SaveChanges();

            return EditProductCategoryResult.Success;
        }

        public async Task<FilterProductCategoriesViewModel> FilterProductCategories(FilterProductCategoriesViewModel filterProductCategoriesViewModel)
        {
            return await _productRepository.FilterProductCategories(filterProductCategoriesViewModel);
        }

        #endregion

        #region product

        public async Task<FilterProductsViewModel> FilterProducts(FilterProductsViewModel filter)
        {
            return await _productRepository.FilterProducts(filter);
        }


        #endregion
    }
}
