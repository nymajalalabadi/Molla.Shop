using Microsoft.EntityFrameworkCore;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.ProductEntities;
using Shop.Domain.ViewModels.Admin.Account;
using Shop.Domain.ViewModels.Admin.Products;
using Shop.Domain.ViewModels.Pigging;
using Shop.Domain.ViewModels.Site.Products;
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

            #region product item

            switch (filter.ProductBox)
            {
                case ProductBox.Default:

                    break;

                case ProductBox.ItemBoxInSite:
                    var pagerBox = Pager.Build(filter.PageId, await _context.ProductCategories.CountAsync(), filter.TakeEntity, filter.CountForShowAfterAndBefore);

                    var allDataBox = await query.Paging(pagerBox).Select(p => new ProductItemViewModel()
                    {
                        ProductCategory = p.ProductSelectedCategories.Select(c => c.ProductCategory).First(),
                        CommentCount = 0,
                        Price = p.Price,
                        ProductId = p.Id,
                        ProductImageName = p.ProductImageName,
                        ProductName = p.Name
                    }).ToListAsync();

                    return filter.SetPaging(pagerBox).SetProductItems(allDataBox);
            }

            #endregion

            #region set paging

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
                .Where(s => s.ProductId == productId).ToListAsync();

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

        public async Task AddProductGalleries(List<ProductGalleries> productGalleries)
        {
            await _context.ProductGalleries.AddRangeAsync(productGalleries);
            await SaveChanges();
        }

        public async Task<bool> CheckProduct(long productId)
        {
            return await _context.Products.AsQueryable()
                .AnyAsync(p => p.Id == productId);
        }

        public async Task<List<ProductGalleriesViewModel>> ShowAllProductGalleries(long productId)
        {
            List<ProductGalleriesViewModel> Galleries = await _context.ProductGalleries
                .Where(e => e.ProductId == productId && !e.IsDelete)
                 .Select(e => new ProductGalleriesViewModel()
                 {
                     Id = e.Id,
                     ProductId = e.ProductId,
                     ImageName = e.ImageName,
                 }).ToListAsync();

            return Galleries;
        }


        public async Task<List<ProductGalleries>> GetAllProductGalleries(long productId)
        {
            return await _context.ProductGalleries.AsQueryable().Where(g => g.ProductId == productId && !g.IsDelete).ToListAsync();
        }

        public async Task<ProductGalleries> GetProductGallery(long id)
        {
            return await _context.ProductGalleries.AsQueryable()
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public void UpdateProductGallery(ProductGalleries productGalleries)
        {
            _context.ProductGalleries.Update(productGalleries);
        }

        public async Task DeleteProductGallery(long id)
        {
            var currentGallery = await GetProductGallery(id);

            if (currentGallery != null)
            {
                currentGallery.IsDelete = true;

                UpdateProductGallery(currentGallery);

                await _context.SaveChangesAsync();
            }
        }

        public async Task AddProductFeatuers(ProductFeature feature)
        {
            await _context.ProductFeatures.AddAsync(feature);
            await SaveChanges();
        }

        public async Task<List<ProductFeatuersViewModel>> ShowAllProductFeatuers(long productId)
        {
            var Featuers = await _context.ProductFeatures.AsQueryable()
                .Where(f => f.ProductId == productId && !f.IsDelete)
                .Select(f => new ProductFeatuersViewModel()
                {
                    Id = f.Id,
                    Title = f.FeatuerTitle,
                    Value = f.FeatureValue
                }).ToListAsync();

            return Featuers;
        }

        public void UpdateProductFeature(ProductFeature feature)
        {
            _context.ProductFeatures.Update(feature);
        }

        public async Task DeleteFeatuers(long id)
        {
            var currentFeatuer = await _context.ProductFeatures.AsQueryable().Where(f => f.Id == id).FirstOrDefaultAsync();

            if (currentFeatuer != null)
            {
                currentFeatuer.IsDelete = true;

                UpdateProductFeature(currentFeatuer);
                await SaveChanges();
            }
        }

        public async Task<List<ProductItemViewModel>> ShowAllProductInSlider()
        {
            var allProduct = await _context.Products.Include(p => p.ProductSelectedCategories).ThenInclude(s => s.ProductCategory)
                .AsQueryable()
                .Select(c => new ProductItemViewModel
                {
                    ProductCategory = c.ProductSelectedCategories.Select(c => c.ProductCategory).First(),
                    CommentCount = 0,
                    Price = c.Price,
                    ProductId = c.Id,
                    ProductImageName = c.ProductImageName,
                    ProductName = c.Name
                }).ToListAsync();

            return allProduct;
        }

        public async Task<List<ProductItemViewModel>> ShowAllProductInCategory(string hrefName)
        {
            //var allProducts = await _context.ProductCategories
            //    .Include(c => c.ProductSelectedCategories).ThenInclude(s => s.Product)
            //    .Where(c => c.UrlName == hrefName)
            //    .Select(c => c.ProductSelectedCategories.Select(s => s.Product))
            //    .ToListAsync();

            var product = await _context.Products
                .Include(p => p.ProductSelectedCategories)
                .ThenInclude(s => s.ProductCategory)
                .Where(p => p.ProductSelectedCategories.Any(s => s.ProductCategory.UrlName == hrefName))
                .ToListAsync();

            var data = product.Select(c => new ProductItemViewModel
            {
                ProductCategory = c.ProductSelectedCategories.Select(c => c.ProductCategory).First(),
                CommentCount = 0,
                Price = c.Price,
                ProductId = c.Id,
                ProductImageName = c.ProductImageName,
                ProductName = c.Name
            }).ToList();

            return data;
        }

        public async Task<List<ProductItemViewModel>> LastProducts()
        {
            var lastProduct = await _context.Products
                .Include(p => p.ProductSelectedCategories)
                .ThenInclude(s => s.ProductCategory)
                .OrderByDescending(p => p.CreateDate)
                .Select(p => new ProductItemViewModel()
                {
                    ProductCategory = p.ProductSelectedCategories.Select(c => c.ProductCategory).First(),
                    CommentCount = 0,
                    Price = p.Price,
                    ProductId = p.Id,
                    ProductImageName = p.ProductImageName,
                    ProductName = p.Name
                })
                .Take(8)
                .ToListAsync();

            return lastProduct;
        }

        public async Task<ProductDetailViewModel> ShowProductDetail(long ProductId)
        {
            return await _context.Products
                .Include(p => p.ProductFeatures)
                .Include(p => p.ProductGalleries)
                .Include(p => p.ProductSelectedCategories).ThenInclude(s => s.ProductCategory)
                .Where(p => p.Id == ProductId)
                .Select(p => new ProductDetailViewModel()
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ShortDescription = p.ShortDescription,
                    Price = p.Price,
                    ProductImageName = p.ProductImageName,
                    ProductComment = 0,
                    ProductFeatures = p.ProductFeatures.Where(s => s.ProductId == ProductId).ToList(),
                    ProductImages = p.ProductGalleries.Where(s => s.ProductId == ProductId).Select(g => g.ImageName).ToList(),
                    ProductCategory = p.ProductSelectedCategories.Select(p => p.ProductCategory).First()
                }).FirstOrDefaultAsync();
        }

        #endregion
    }
}
