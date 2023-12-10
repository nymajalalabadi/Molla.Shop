using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Application.Services;
using Shop.Domain.ViewModels.Admin.Products;
using Shop.Web.Extentions;

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
            return View(await _productService.FilterProducts(filter));
        }

        #endregion

        #region create product

        [HttpGet("createproduct")]
        public async Task<IActionResult> CreateProduct()
        {
            ViewData["Categories"] = await _productService.GetAllProductCategories();
            return View();
        }

        [HttpPost("createproduct"), ValidateAntiForgeryToken]
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

        #region edit product

        [HttpGet("editproduct/{productId}")]
        public async Task<IActionResult> EditProduct(long productId)
        {
            ViewData["Categories"] = await _productService.GetAllProductCategories();

            var product = await _productService.GetEditProduct(productId);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost("editproduct/{productId}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(EditProductViewModel editProduct, IFormFile productImage)
        {
            ViewData["Categories"] = await _productService.GetAllProductCategories();

            if (ModelState.IsValid)
            {
                var result = await _productService.EditProduct(editProduct, productImage);

                switch (result)
                {
                    case EditProductResult.NotFound:
                        TempData[WarningMessage] = "محصولی با مشخصات وارد شده یافت نشد";
                        break;

                    case EditProductResult.NotProductSelectedCategoryHasNull:
                        TempData[WarningMessage] = "لطفا دسته بندی محصول را وارد کنید";
                        break;

                    case EditProductResult.Success:
                        TempData[SuccessMessage] = ".ویرایش محصول با موفقیت انجام شد";

                        return RedirectToAction("Index");
                }
            }
            return View(editProduct);
        }

        #endregion

        #region delete product

        [HttpGet("deleteproduct/{productId}")]
        public async Task<IActionResult> DeleteProduct(long productId)
        {
            var result = await _productService.DeleteProduct(productId);

            if (result)
            {
                TempData[SuccessMessage] = "محصول شما با موفقیت حذف شد.";
                return RedirectToAction("Index");
            }

            TempData[WarningMessage] = "محصول شما با موفقیت حذف نشد لطفا دوباره اقدام بفرمایید.";
            return RedirectToAction("Index");
        }

        #endregion

        #region recover product

        [HttpGet("recoverproduct/{productId}")]
        public async Task<IActionResult> RecoverProduct(long productId)
        {
            var result = await _productService.RecoverProduct(productId);

            if (result)
            {
                TempData[SuccessMessage] = "محصول شما با موفقیت بازگردانی شد.";
                return RedirectToAction("Index");
            }

            TempData[WarningMessage] = "محصول شما با موفقیت بازگردانی نشد لطفا دوباره اقدام بفرمایید.";
            return RedirectToAction("Index");
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

        [HttpPost("createcategoty"), ValidateAntiForgeryToken]
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

        [HttpGet("editecategory/{categoryId}")]
        public async Task<IActionResult> EditProductCategory(long categoryId)
        {
            var data = await _productService.GetEditPrdouctCategory(categoryId);

            if (data == null) return NotFound();

            return View(data);
        }

        [HttpPost("editecategory/{categoryId}"), ValidateAntiForgeryToken]
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

        #region product galleries - create gallery

        [HttpGet("galleryproduct/{productId}")]
        public IActionResult GalleryProduct(long productId)
        {
            ViewBag.productId = productId;

            return View();
        }


        public async Task<IActionResult> AddImageToProduct(List<IFormFile> images, long productId)
        {
            var result = await _productService.AddProductGallery(productId, images);

            if (result)
            {
                return JsonResponseStatus.Success();
            }

            return JsonResponseStatus.Error();
        }

        #endregion

        #region list product galleries

        [HttpGet("productgalleries/{productId}")]
        public async Task<IActionResult> ProductGalleries(long productId)
        {
            var galleries = new ShowAllProductGalleriesViewModel()
            {
                ProductGallery = await _productService.ShowAllProductGalleries(productId),
            };

            return View(galleries);
        }

        #endregion

        #region delete product gallery

        [HttpGet]
        public async Task<IActionResult> DeleteImage(long galleryId)
        {
            await _productService.DeleteImage(galleryId);

            return RedirectToAction("Index");
        }

        #endregion

        #region product Featuers

        [HttpGet("createproductfeatuers/{productId}")]
        public async Task<IActionResult> CreateProductFeatuers(long productId)
        {
            var model = new CreateProductFeatuersViewModel()
            {
                ProductId = productId
            };

            return View(model);
        }

        [HttpPost("createproductfeatuers/{productId}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProductFeatuers(CreateProductFeatuersViewModel productFeatuers)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.CreateProductFeatuers(productFeatuers);

                switch (result)
                {
                    case CreateProductFeatuersResult.Error:
                        TempData[ErrorMessage] = "در ثبت ویژگی خطایی رخ داده است";
                        break;

                    case CreateProductFeatuersResult.Success:
                        TempData[SuccessMessage] = "ویژگی با موفقیت ثبت شد";

                        return RedirectToAction("Index");
                }

            }
            return View(productFeatuers);
        }

        #region product featuers

        public async Task<IActionResult> ProductFeatuers(long productId)
        {
            var features = new ShowAllProductFeatuersViewModel()
            {
                ProductFeatuers = await _productService.ShowAllProductFeatuers(productId)
            };

            return View(features);
        }

        #endregion


        #endregion
    }
}
