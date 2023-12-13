﻿using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Domain.ViewModels.Admin.Products;
using Shop.Domain.ViewModels.Site.Sliders;

namespace Shop.Web.ViewComponents
{
    #region siteHeader

    public class SiteHeaderViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        public SiteHeaderViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.User = await _userService.GetUserByPhoneNumber(User.Identity.Name);
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
}
