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

        #region category - admin

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
                .AnyAsync(p => p.UrlName == urlName && p.Id != CategoryId);
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

        public async Task<List<ProductCategory>> GetAllProductCategories()
        {
            return await _context.ProductCategories.AsQueryable()
                 .Where(c => !c.IsDelete)
                 .ToListAsync();
        }

        #endregion

        #region product

        public async Task<FilterProductsViewModel> FilterProducts(FilterProductsViewModel filter)
        {
            var query = _context.Products
                .Include(c => c.ProductSelectedCategories)
                .ThenInclude(c => c.ProductCategory)
                .AsQueryable();

            #region filter

            if (!string.IsNullOrEmpty(filter.ProductName))
            {
                query = query.Where(p => EF.Functions.Like(p.Name, $"%{filter.ProductName}%"));
            }

            if (!string.IsNullOrEmpty(filter.FilterByCategory))
            {
                query = query.Where(p => p.ProductSelectedCategories.Any(c => c.ProductCategory.UrlName == filter.FilterByCategory));
            }

            #endregion

            #region state

            switch (filter.ProductState)
            {
                case ProductState.AllOfThem:
                    query = query.Where(p => !p.IsDelete);
                    break;
                case ProductState.IsActice:
                    query = query.Where(p => p.IsActive);
                    break;
                case ProductState.Delete:
                    query = query.Where(p => p.IsDelete);
                    break;
                default:
                    break;
            }

            #endregion

            #region order

            switch (filter.ProductOrder)
            {
                case ProductOrder.All:
                    break;

                case ProductOrder.ProductNewss:
                    query = query.Where(p => p.IsActive).OrderByDescending(p => p.CreateDate);
                    break;

                case ProductOrder.ProductExp:
                    query = query.Where(p => p.IsActive).OrderByDescending(p => p.Price);
                    break;

                case ProductOrder.ProductInExprnsive:
                    query = query.Where(p => p.IsActive).OrderBy(p => p.Price);
                    break;
            }

            #endregion

            #region paging

            var pager = Pager.Build(filter.PageId, await _context.ProductCategories.CountAsync(), filter.TakeEntity, filter.CountForShowAfterAndBefore);

            var allData = await query.Paging(pager).ToListAsync();

            #endregion

            return filter.SetPaging(pager).SetProducts(allData);
        }

        public async Task<Product> GetProductById(long productId)
        {
            return await _context.Products.AsQueryable().SingleOrDefaultAsync(p => p.Id == productId);
        }


        public async Task AddProduct(Product product)
        {
            _context.Products.Add(product);
        }

        public async Task RemoveProductSelectedCategories(long productId)
        {
            var allProductSelectedCategories = await _context.ProductSelectedCategories.AsQueryable()
                .Where(s => s.ProductId ==  productId).ToListAsync();

            if (allProductSelectedCategories.Any())
            {
                _context.ProductSelectedCategories.RemoveRange(allProductSelectedCategories);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddProductSelectedCategories(List<long> productSelectedCategories, long productId)
        {
            if (productSelectedCategories != null && productSelectedCategories.Any())
            {
                var newProductSelectedCategories = new List<ProductSelectedCategories>();

                foreach (var categoryId in productSelectedCategories)
                {
                    newProductSelectedCategories.Add(new ProductSelectedCategories
                    {
                        ProductId = productId,
                        ProductCategoryId = categoryId,
                    });
                }

                await _context.ProductSelectedCategories.AddRangeAsync(newProductSelectedCategories);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<long>> GetAllProductCategoriesId(long productId)
        {
            return await _context.ProductSelectedCategories.AsQueryable()
                .Where(s => s.ProductId == productId)
                .Select(s => s.ProductCategoryId).ToListAsync();
        }

        public async void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
        }

        public async Task<bool> DeleteProduct(long productId)
        {
            var product = await GetProductById(productId);

            if (product != null)
            {
                product.IsDelete = true;

                UpdateProduct(product);
                await SaveChanges();

                return true;
            }
            return false;
        }

        public async Task<bool> RecoverProduct(long productId)
        {
            var product = await GetProductById(productId);

            if (product != null)
            {
                product.IsDelete = false;

                UpdateProduct(product);
                await SaveChanges();

                return true;
            }
            return false;
        }

        #endregion
    }
}
