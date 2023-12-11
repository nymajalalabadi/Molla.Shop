using Microsoft.EntityFrameworkCore;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Site;
using Shop.Domain.ViewModels.Pigging;
using Shop.Domain.ViewModels.Site.Sliders;
using Shop.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Infra.Data.Repositories
{
    public class SiteSettingRepository : ISiteSettingRepository
    {
        #region constractore

        private readonly ShopDbContext _context;

        public SiteSettingRepository(ShopDbContext context)
        {
            _context = context;
        }

        #endregion

        #region slider

        public Task SaveChanges()
        {
            return _context.SaveChangesAsync();
        }


        public async Task<FilterSlidersViewModel> FilterSliders(FilterSlidersViewModel filter)
        {
            var query = _context.Sliders.AsQueryable();

            #region filter

            if (!string.IsNullOrEmpty(filter.SliderTitle))
            {
                query = query.Where(p => EF.Functions.Like(p.SliderTitle, $"%{filter.SliderTitle}%"));
            }

            #endregion

            #region paging

            var pager = Pager.Build(filter.PageId, await _context.Sliders.CountAsync(), filter.TakeEntity, filter.CountForShowAfterAndBefore);

            var allData = await query.Paging(pager).ToListAsync();

            #endregion

            return filter.SetPaging(pager).SetSilders(allData);
        }

        public async Task AddSlider(Slider slider)
        {
            await _context.Sliders.AddAsync(slider);
            await SaveChanges();
        }

        public async Task<Slider> GetSliderById(long sliderId)
        {
            return await _context.Sliders
                .SingleOrDefaultAsync(s => s.Id == sliderId);
        }

        public void UpdateSlider(Slider slider)
        {
             _context.Sliders.Update(slider);
        }

        #endregion

    }
}
