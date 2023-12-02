using Microsoft.AspNetCore.Http;
using Shop.Domain.ViewModels.Admin.Products;
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

        #endregion

        #region product

        Task<FilterProductsViewModel> FilterProducts(FilterProductsViewModel filter);

        #endregion
    }
}
