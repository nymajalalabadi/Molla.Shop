using Microsoft.AspNetCore.Http;
using Shop.Application.Extentions;
using Shop.Application.Interfaces;
using Shop.Application.Utils;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Site;
using Shop.Domain.ViewModels.Site.Sliders;
using Shop.Infra.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Services
{
    public class SiteSettingService : ISiteSettingService
    {
        #region constractore

        private readonly ISiteSettingRepository _siteSettingRepository;

        public SiteSettingService(ISiteSettingRepository siteSettingRepository)
        {
            _siteSettingRepository = siteSettingRepository;
        }

        #endregion

        #region slider

        public async Task<FilterSlidersViewModel> FilterSliders(FilterSlidersViewModel filter)
        {
            return await _siteSettingRepository.FilterSliders(filter);
        }

        public async Task<CreateSliderResult> CreateSlider(CreateSliderViewModel slider, IFormFile image)
        {
            var newSlider = new Slider()
            {
                Href = slider.Href,
                Price = slider.Price,
                SliderText = slider.SliderText,
                SliderTitle = slider.SliderTitle,
                TextBtn = slider.TextBtn
            };

            if (image != null && image.IsImage())
            {
                var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(image.FileName);

                image.AddImageToServer(imageName, PathExtensions.SliderOrginServer, 255, 273, PathExtensions.SliderThumbServer);

                newSlider.SliderImage = imageName;
            }
            else
            {
                return CreateSliderResult.ImageNotFound;
            }

            await _siteSettingRepository.AddSlider(newSlider);
            await _siteSettingRepository.SaveChanges();

            return CreateSliderResult.Success;
        }

        public async Task<EditSliderViewModel> GetEditSlider(long sliderId)
        {
            var currentSlider = await _siteSettingRepository.GetSliderById(sliderId);

            if (currentSlider == null)
            {
                return null;
            }

            return new EditSliderViewModel()
            {
                SliderId = currentSlider.Id,
                TextBtn = currentSlider.TextBtn,
                SliderText = currentSlider.SliderText,
                SliderTitle = currentSlider.SliderTitle,
                Href = currentSlider.Href,
                ImageFile = currentSlider.SliderImage,
                Price = currentSlider.Price,
            };
        }

        public async Task<EditSliderResult> EditSlider(EditSliderViewModel editSlider, IFormFile image)
        {
            var slider = await _siteSettingRepository.GetSliderById(editSlider.SliderId);

            if (slider == null)
                return EditSliderResult.NotFound;

            slider.SliderText = editSlider.SliderText;
            slider.SliderTitle = editSlider.SliderTitle;
            slider.TextBtn = editSlider.TextBtn;
            slider.Price = editSlider.Price;
            slider.Href = editSlider.Href;

            if (image != null && image.IsImage())
            {
                var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(image.FileName);

                image.AddImageToServer(imageName, PathExtensions.SliderOrginServer, 255, 273, PathExtensions.SliderThumbServer, slider.SliderImage);

                slider.SliderImage = imageName;
            }

            _siteSettingRepository.UpdateSlider(slider);
            await _siteSettingRepository.SaveChanges();

            return EditSliderResult.Success;
        }

        #endregion
    }
}
