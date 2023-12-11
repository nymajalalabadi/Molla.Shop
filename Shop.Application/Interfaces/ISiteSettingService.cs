using Microsoft.AspNetCore.Http;
using Shop.Domain.ViewModels.Site.Sliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Interfaces
{
    public interface ISiteSettingService
    {
        #region slider

        Task<FilterSlidersViewModel> FilterSliders(FilterSlidersViewModel filter);

        Task<CreateSliderResult> CreateSlider(CreateSliderViewModel slider, IFormFile image);

        Task<EditSliderViewModel> GetEditSlider(long sliderId);

        Task<EditSliderResult> EditSlider(EditSliderViewModel editSlider, IFormFile image);

        #endregion
    }
}
