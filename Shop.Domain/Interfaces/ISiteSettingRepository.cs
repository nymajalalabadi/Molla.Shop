using Shop.Domain.Models.Site;
using Shop.Domain.ViewModels.Admin.Products;
using Shop.Domain.ViewModels.Site.Sliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.Interfaces
{
    public interface ISiteSettingRepository
    {
        #region slider

        Task SaveChanges();

        Task<FilterSlidersViewModel> FilterSliders(FilterSlidersViewModel filter);

        Task AddSlider(Slider slider);

        Task<Slider> GetSliderById(long sliderId);

        void UpdateSlider(Slider slider);
        
        #endregion
    }
}
