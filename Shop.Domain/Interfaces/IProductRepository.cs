﻿using Shop.Domain.Models.ProductEntities;
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

        Task AddProduct(Product product);
        Task RemoveProductSelectedCategories(long productId);

        Task AddProductSelectedCategories(List<long> productSelectedCategories, long productId);

        #endregion
    }
}
