using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Domain.Models.ProductEntities;
using Shop.Domain.ViewModels.Admin.Products;
using Shop.Domain.ViewModels.Site.Products;
using Shop.Domain.ViewModels.Site.Sliders;
using Shop.Web.Extentions;

namespace Shop.Web.ViewComponents
{
    #region siteHeader

    public class SiteHeaderViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        public SiteHeaderViewComponent(IUserService userService, IOrderService orderService)
        {
            _userService = userService;
            _orderService = orderService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.User = await _userService.GetUserByPhoneNumber(User.Identity.Name);

                ViewBag.Order = await _orderService.GetBasketForUser(User.GetUserId());
            }
            return View("SiteHeader");
        }
    }

    #endregion

    #region siteFooter

    public class SiteFooterViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("SiteFooter");
        }
    }

    #endregion


    #region slider - home

    public class SliderHomeViewComponent : ViewComponent
    {
        #region constractor

        private readonly ISiteSettingService _siteSettingService;

        public SliderHomeViewComponent(ISiteSettingService siteSettingService)
        {
            _siteSettingService = siteSettingService;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var filterSlidersViewModel = new FilterSlidersViewModel()
            {
                TakeEntity = 10
            };

            var data = await _siteSettingService.FilterSliders(filterSlidersViewModel);

            return View("SliderHome", data);
        }
    }

    #endregion

    #region popular category - home

    public class PopularCategoryViewComponent : ViewComponent
    {
        #region constractor

        private readonly IProductService _productService;

        public PopularCategoryViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var filterCategory = new FilterProductCategoriesViewModel()
            {
                TakeEntity = 6
            };

            var data = await _productService.FilterProductCategories(filterCategory);

            return View("PopularCategory", data);
        }
    }

    #endregion

    #region SideBar-Category

    public class SideBarCategoryViewComponent : ViewComponent
    {
        #region constractor

        private readonly IProductService _productService;

        public SideBarCategoryViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var filterCategory = new FilterProductCategoriesViewModel();
            
            var data = await _productService.FilterProductCategories(filterCategory);

            return View("SideBarCategory", data);
        }
    }


    #endregion


    #region All Product InSlider - home

    public class AllProductInSliderViewComponent : ViewComponent
    {
        #region constractor

        private readonly IProductService _productService;

        public AllProductInSliderViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var data = await _productService.ShowAllProductInSlider();

            return View("AllProductInSlider", data);
        }
    }

    #endregion

    #region All-productInCategoryPc - home

    public class AllInCategoryPcViewComponent : ViewComponent
    {
        #region constractor

        private readonly IProductService _productService;

        public AllInCategoryPcViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var data = await _productService.ShowAllProductInCategory("pc");

            return View("AllInCategoryPc", data);
        }
    }

    #endregion

    #region All-productInCategoryTv - home
    public class AllInCategoryTvViewComponent : ViewComponent
    {
        #region constractor
        private readonly IProductService _productService;
        public AllInCategoryTvViewComponent(IProductService productService)
        {
            _productService = productService;
        }
        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var data = await _productService.ShowAllProductInCategory("tv");

            return View("AllInCategoryTv", data);
        }
    }
    #endregion

    #region Show-Product-Comment
    public class ProductCommentsViewComponent : ViewComponent
    {
        #region constractor

        private readonly IProductService _productService;

        public ProductCommentsViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync(long productId)
        {
            var data = await _productService.AllProductCommentById(productId);

            return View("ProductComments", data);
        }
    }
    #endregion

}
