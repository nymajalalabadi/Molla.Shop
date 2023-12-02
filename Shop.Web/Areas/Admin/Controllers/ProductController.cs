using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Application.Services;
using Shop.Domain.ViewModels.Admin.Products;

namespace Shop.Web.Areas.Admin.Controllers
{
    public class ProductController : AdminBaseController
    {
        #region constractor

        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        #endregion

        #region filter products

        [HttpGet]
        public async Task<IActionResult> Index(FilterProductsViewModel filter)
        {
            filter.ProductState = ProductState.All;

            return View(await _productService.FilterProducts(filter));
        }


        [HttpGet("createproduct")]
        public async Task<IActionResult> CreateProduct()
        {
            ViewData["Categories"] = await _productService.GetAllProductCategories();
            return View();
        }

        [HttpPost("createproduct"),ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(CreateProductViewModel createProduct, IFormFile productImage)
        {
            ViewData["Categories"] = await _productService.GetAllProductCategories();

            if (ModelState.IsValid)
            {
                var result = await _productService.CreateProduct(createProduct, productImage);

                switch (result)
                {
                    case CreateProductResult.NotImage:
                        TempData[WarningMessage] = "لطفا برای محصول یک تصویر انتخاب کنید";
                        break;

                    case CreateProductResult.Success:
                        TempData[SuccessMessage] = "عملیات ثبت محصول با موفقیت انجام شد";

                        return RedirectToAction("Index");
                }
            }

            return View(createProduct);
        }

        #endregion

        #region Filter Categories

        [HttpGet]
        public async Task<IActionResult> FilterProductCategories(FilterProductCategoriesViewModel filterProduct)
        {
            return View(await _productService.FilterProductCategories(filterProduct));
        }

        #endregion


        #region create category

        [HttpGet("createcategoty")]
        public async Task<IActionResult> CreateProductCategory()
        {
            return View();
        }

        [HttpPost("createcategoty"),ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProductCategory(CreateProductCategoryViewModel createCategory, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.CreatePrdouctCategory(createCategory, image);

                switch (result)
                {
                    case CreateProductCategoryResult.IsExistUrlName:
                        TempData[WarningMessage] = "اسم Url تکراری است";
                        break;
                    case CreateProductCategoryResult.Success:
                        TempData[SuccessMessage] = "دسته بندی با موفقیت ثبت شد";

                        return RedirectToAction("FilterProductCategories");
                }
            }
            return View(createCategory);
        }

        #endregion

        #region edit category

        [HttpGet("editecategory")]
        public async Task<IActionResult> EditProductCategory(long categoryId)
        {
            var data = await _productService.GetEditPrdouctCategory(categoryId);

            if (data == null) return NotFound();

            return View(data);
        }

        [HttpPost("editecategory"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProductCategory(EditProductCategoryViewModel editProductCategory, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.EditPrdouctCategory(editProductCategory, image);

                switch (result)
                {
                    case EditProductCategoryResult.IsExistUrlName:
                        TempData[WarningMessage] = "اسم Url تکراری است";
                        break;
                    case EditProductCategoryResult.NotFound:
                        TempData[ErrorMessage] = "دسته بندی با مشخصات وارد شده یافت نشد";
                        break;
                    case EditProductCategoryResult.Success:
                        TempData[SuccessMessage] = "دسته بندی با موفقیت ویرایش شد";

                        return RedirectToAction("FilterProductCategories");
                }
            }
            return View(editProductCategory);
        }

        #endregion
    }
}
