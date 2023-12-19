﻿using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Domain.ViewModels.Admin.Products;

namespace Shop.Web.Controllers
{
    public class ProductController : SiteBaseController
    {
        #region constractor

        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
               _productService = productService;
        }

        #endregion

        #region products

        [HttpGet("products")]
        public async Task<IActionResult> Products(FilterProductsViewModel filterProducts)
        {
            filterProducts.TakeEntity = 12;
            filterProducts.ProductBox = ProductBox.ItemBoxInSite;

            return View(await _productService.FilterProducts(filterProducts));
        }

        #endregion


    }
}