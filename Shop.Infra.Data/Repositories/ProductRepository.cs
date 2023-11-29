using Microsoft.EntityFrameworkCore;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.ProductEntities;
using Shop.Domain.ViewModels.Admin.Products;
using Shop.Domain.ViewModels.Pigging;
using Shop.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Infra.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        #region constractore

        private readonly ShopDbContext _context;

        public ProductRepository(ShopDbContext context)
        {
            _context = context;
        }

        #endregion

        #region product - admin

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task AddProductCtaegory(ProductCategory productCategory)
        {
            await _context.ProductCategories.AddAsync(productCategory);
        }

        public void UpdateProductCtaegory(ProductCategory category)
        {
            _context.ProductCategories.Update(category);
        }

        public async Task<ProductCategory> GetProductCategoryById(long id)
        {
            return await _context.ProductCategories.AsQueryable().SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> CheckUrlNameCategories(string urlName)
        {
            return await _context.ProductCategories.AsQueryable()
                .AnyAsync(p => p.UrlName == urlName);
        }

        public async Task<bool> CheckUrlNameCategories(string urlName, long CategoryId)
        {
            return await _context.ProductCategories.AsQueryable()
                .AnyAsync(p => p.UrlName == urlName && p.Id !=  CategoryId);
        }

        public async Task<FilterProductCategoriesViewModel> FilterProductCategories(FilterProductCategoriesViewModel filter)
        {
            var query = _context.ProductCategories.AsQueryable();

            #region filter

            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(p => EF.Functions.Like(p.Title, $"%{filter.Title}%"));
            }

            #endregion

            #region paging

            var pager = Pager.Build(filter.PageId, await _context.ProductCategories.CountAsync(), filter.TakeEntity, filter.CountForShowAfterAndBefore);

            var allData = await query.Paging(pager).ToListAsync();

            #endregion

            return filter.SetPaging(pager).SetProductCategories(allData);
        }

        #endregion
    }
}
