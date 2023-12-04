﻿using Microsoft.AspNetCore.Http;
using Shop.Application.Extentions;
using Shop.Application.Interfaces;
using Shop.Application.Utils;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.ProductEntities;
using Shop.Domain.ViewModels.Admin.Products;
using SixLabors.ImageSharp;
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

        public async Task<List<ProductCategory>> GetAllProductCategories()
        {
            return await _productRepository.GetAllProductCategories();
        }
        #endregion

        #region product

        public async Task<FilterProductsViewModel> FilterProducts(FilterProductsViewModel filter)
        {
            return await _productRepository.FilterProducts(filter);
        }

        public async Task<CreateProductResult> CreateProduct(CreateProductViewModel createProduct, IFormFile imageProduct)
        {
            var newProduct = new Product()
            {
                Name = createProduct.Name,
                Price = createProduct.Price,
                Description = createProduct.Description,
                ShortDescription = createProduct.ShortDescription,
                IsActive = createProduct.IsActive
            };

            if (imageProduct != null && imageProduct.IsImage())
            {
                var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(imageProduct.FileName);

                imageProduct.AddImageToServer(imageName, PathExtensions.ProductOrginServer, 255, 273, PathExtensions.ProductThumbServer);

                newProduct.ProductImageName = imageName;
            }
            else
            {
                return CreateProductResult.NotImage;
            }

           await _productRepository.AddProduct(newProduct);
           await _productRepository.SaveChanges();

           await _productRepository.AddProductSelectedCategories(createProduct.ProductSelectedCategory, newProduct.Id);

            return CreateProductResult.Success;
        }

        public async Task<EditProductViewModel> GetEditProduct(long productId)
        {
            var product = await _productRepository.GetProductById(productId);

            if(product != null)
            {
                return new EditProductViewModel
                {
                    Description = product.Description,
                    IsActive = product.IsActive,
                    Name = product.Name,
                    Price = product.Price,
                    ShortDescription = product.ShortDescription,
                    ProductImageName = product.ProductImageName,
                    ProductSelectedCategory = await _productRepository.GetAllProductCategoriesId(productId)
                };
            }

            return null;
        }

        public async Task<EditProductResult> EditProduct(EditProductViewModel editProduct, IFormFile ProductImage)
        {
            var product = await _productRepository.GetProductById(editProduct.ProductId);

            if (product == null) return EditProductResult.NotFound;

            if (editProduct.ProductSelectedCategory == null) return EditProductResult.NotProductSelectedCategoryHasNull;

            #region edit product

            product.ShortDescription = editProduct.ShortDescription;
            product.Description = editProduct.Description;
            product.IsActive = editProduct.IsActive;
            product.Price = editProduct.Price;
            product.Name = editProduct.Name;

            if (ProductImage != null && ProductImage.IsImage())
            {
                var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(ProductImage.FileName);

                ProductImage.AddImageToServer(imageName, PathExtensions.ProductOrginServer, 255, 273, PathExtensions.ProductThumbServer, product.ProductImageName);

                product.ProductImageName = imageName;
            }

            _productRepository.UpdateProduct(product);
            await _productRepository.SaveChanges();

            #endregion

            await _productRepository.RemoveProductSelectedCategories(editProduct.ProductId);
            await _productRepository.AddProductSelectedCategories(editProduct.ProductSelectedCategory, editProduct.ProductId);

            return EditProductResult.Success;
        }

        #endregion
    }
}
