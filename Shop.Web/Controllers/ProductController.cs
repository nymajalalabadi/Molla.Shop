using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Domain.ViewModels.Admin.Products;
using Shop.Domain.ViewModels.Site.Products;
using Shop.Web.Extentions;

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
            ViewData["Categories"] = await _productService.GetAllProductCategories();

            filterProducts.TakeEntity = 12;
            filterProducts.ProductBox = ProductBox.ItemBoxInSite;

            return View(await _productService.FilterProducts(filterProducts));
        }

        #endregion

        #region Product-Detail

        [HttpGet("ProductDetail/{productId}")]
        public async Task<IActionResult> ProductDetail(long productId)
        {
            var product = await _productService.ShowProductDetail(productId);

            if (product == null)
            {
                return NotFound();
            }

            TempData["RelatedProduct"] = await _productService.GetRelatedProduct(product.ProductCategory.UrlName);

            return View(product);
        }

        #endregion

        #region create-product-comment

        [HttpPost("add-comment"),ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProductComment(CreateProductCommentViewModel productComment)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.CreateProductComment(productComment, User.GetUserId());

                switch (result)
                {
                    case CreateProductCommentResult.CheckUser:
                        TempData[WarningMessage] = "کاربر مورد نظر یافت نشد";
                        break;

                    case CreateProductCommentResult.CheckProduct:
                        TempData[WarningMessage] = "محصولی یافت نشد";
                        break;

                    case CreateProductCommentResult.Success:
                        TempData[SuccessMessage] = "نظر شما با موفقیت ثبت شد";

                        return RedirectToAction("ProductDetail", new { productId = productComment.ProductId });
                }
            }

            TempData[ErrorMessage] = "لطفا نظر خود را وارد نمایید";
            return RedirectToAction("ProductDetail", new { productId = productComment.ProductId });
        }

        #endregion

    }
}
