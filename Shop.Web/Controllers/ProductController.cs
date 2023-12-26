using Microsoft.AspNetCore.Authorization;
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

        private readonly IOrderService _orderService;

        public ProductController(IProductService productService, IOrderService orderService)
        {
            _productService = productService;
            _orderService = orderService;
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

            TempData["RelatedProduct"] = await _productService.GetRelatedProduct(product.ProductCategory.UrlName, productId);

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

        #region buy-product

        [Authorize]
        public async Task<IActionResult> BuyProduct(long productId)
        {
            long orderId = await _orderService.AddOrder(User.GetUserId(), productId);
            return Redirect("/User/Basket" + orderId);
        }

        #endregion

    }
}
