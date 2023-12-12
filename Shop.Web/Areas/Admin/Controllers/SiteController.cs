using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Domain.ViewModels.Site.Sliders;

namespace Shop.Web.Areas.Admin.Controllers
{
    public class SiteController : AdminBaseController
    {
        #region constractor

        private readonly ISiteSettingService _siteSettingService;

        public SiteController(ISiteSettingService siteSettingService)
        {
            _siteSettingService = siteSettingService;
        }

        #endregion

        #region filter sliders

        public async Task<IActionResult> Index(FilterSlidersViewModel filterSliders)
        {
            return View(await _siteSettingService.FilterSliders(filterSliders));
        }

        #endregion

        #region create-slider

        [HttpGet("createslider")]
        public async Task<IActionResult> CreateSlider()
        {
            return View();
        }

        [HttpPost("createslider"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSlider(CreateSliderViewModel createSlider, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var result = await _siteSettingService.CreateSlider(createSlider, image);

                switch (result)
                {
                    case CreateSliderResult.ImageNotFound:
                        TempData[WarningMessage] = "لطفا یک عکس قرار دهید";
                        break;

                    case CreateSliderResult.Success:
                        TempData[SuccessMessage] = "ثبت عکس با موفقیت انجام شد";

                        return RedirectToAction("Index");
                }
            }

            return View(createSlider);
        }

        #endregion

        #region edit-slider

        [HttpGet("editslider/{sliderId}")]
        public async Task<IActionResult> EditSlider(long sliderId)
        {
            var slider = await _siteSettingService.GetEditSlider(sliderId);

            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        [HttpPost("editslider/{sliderId}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSlider(EditSliderViewModel editSlider, IFormFile? image)
        {
            if (ModelState.IsValid)
            {
                var result = await _siteSettingService.EditSlider(editSlider, image);
                switch (result)
                {
                    case EditSliderResult.NotFound:
                        TempData[WarningMessage] = "محصول مورد نظر پیدا نشد";
                        break;

                    case EditSliderResult.Success:
                        TempData[SuccessMessage] = "ویرایش محصول با موفیقت انجام شد";

                        return RedirectToAction("Index");
                }
            }

            return View(editSlider);
        }

        #endregion
    }
}
