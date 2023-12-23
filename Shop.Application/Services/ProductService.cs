using Microsoft.AspNetCore.Http;
using Shop.Application.Extentions;
using Shop.Application.Interfaces;
using Shop.Application.Utils;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.ProductEntities;
using Shop.Domain.ViewModels.Admin.Products;
using Shop.Domain.ViewModels.Site.Products;

namespace Shop.Application.Services
{
    public class ProductService : IProductService
    {
        #region constractor

        private readonly IProductRepository _productRepository;

        private readonly IUserRepository _userRepository;

        public ProductService(IProductRepository productRepository, IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        #endregion

        #region category - admin

        public async Task<CreateProductCategoryResult> CreatePrdouctCategory(CreateProductCategoryViewModel createProductCategory, IFormFile image)
        {
            if (await _productRepository.CheckUrlNameCategories(createProductCategory.UrlName))
                return CreateProductCategoryResult.IsExistUrlName;

            var newProductCategory = new ProductCategory()
            {
                Title = createProductCategory.Title,
                UrlName = createProductCategory.UrlName,
                ParentId = null,
                IsDelete = false
            };

            if (image != null && image.IsImage())
            {
                var nameImage = Guid.NewGuid().ToString("N") + Path.GetExtension(image.FileName);

                image.AddImageToServer(nameImage, PathExtensions.CategoryOrginServer, 150, 150, PathExtensions.CategoryThumbServer);

                newProductCategory.ImageName = nameImage;
            }

            await _productRepository.AddProductCtaegory(newProductCategory);
            await _productRepository.SaveChanges();

            return CreateProductCategoryResult.Success;
        }

        public async Task<EditProductCategoryViewModel> GetEditPrdouctCategory(long categoryId)
        {
            var categoryProduct = await _productRepository.GetProductCategoryById(categoryId);

            if (categoryProduct == null)
            {
                return null;
            }

            return new EditProductCategoryViewModel()
            {
                ProductCategoryId = categoryProduct.Id,
                Title = categoryProduct.Title,
                ImageName = categoryProduct.ImageName,
                UrlName = categoryProduct.UrlName
            };
        }

        public async Task<EditProductCategoryResult> EditPrdouctCategory(EditProductCategoryViewModel editProductCategoryViewModel, IFormFile image)
        {
            var productCategory = await _productRepository.GetProductCategoryById(editProductCategoryViewModel.ProductCategoryId);

            if (productCategory == null)
                return EditProductCategoryResult.NotFound;

            if (await _productRepository.CheckUrlNameCategories(editProductCategoryViewModel.UrlName, editProductCategoryViewModel.ProductCategoryId))
                return EditProductCategoryResult.IsExistUrlName;

            productCategory.Title = editProductCategoryViewModel.Title;
            productCategory.UrlName = editProductCategoryViewModel.UrlName;

            if (image != null && image.IsImage())
            {
                var nameImage = Guid.NewGuid().ToString("N") + Path.GetExtension(image.FileName);

                image.AddImageToServer(nameImage, PathExtensions.CategoryOrginServer, 150, 150, PathExtensions.CategoryThumbServer, productCategory.ImageName);

                productCategory.ImageName = nameImage;
            }

            _productRepository.UpdateProductCtaegory(productCategory);
            await _productRepository.SaveChanges();

            return EditProductCategoryResult.Success;
        }

        public async Task<FilterProductCategoriesViewModel> FilterProductCategories(FilterProductCategoriesViewModel filterProductCategoriesViewModel)
        {
            return await _productRepository.FilterProductCategories(filterProductCategoriesViewModel);
        }

        public async Task<List<ProductCategory>> GetAllProductCategories()
        {
            return await _productRepository.GetAllProductCategories();
        }
        #endregion

        #region product

        public async Task<FilterProductsViewModel> FilterProducts(FilterProductsViewModel filter)
        {
            return await _productRepository.FilterProducts(filter);
        }

        public async Task<CreateProductResult> CreateProduct(CreateProductViewModel createProduct, IFormFile imageProduct)
        {
            var newProduct = new Product()
            {
                Name = createProduct.Name,
                Price = createProduct.Price,
                Description = createProduct.Description,
                ShortDescription = createProduct.ShortDescription,
                IsActive = createProduct.IsActive
            };

            if (imageProduct != null && imageProduct.IsImage())
            {
                var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(imageProduct.FileName);

                imageProduct.AddImageToServer(imageName, PathExtensions.ProductOrginServer, 255, 273, PathExtensions.ProductThumbServer);

                newProduct.ProductImageName = imageName;
            }
            else
            {
                return CreateProductResult.NotImage;
            }

            await _productRepository.AddProduct(newProduct);
            await _productRepository.SaveChanges();

            await _productRepository.AddProductSelectedCategories(createProduct.ProductSelectedCategory, newProduct.Id);

            return CreateProductResult.Success;
        }

        public async Task<EditProductViewModel> GetEditProduct(long productId)
        {
            var product = await _productRepository.GetProductById(productId);

            if (product != null)
            {
                return new EditProductViewModel
                {
                    Description = product.Description,
                    IsActive = product.IsActive,
                    Name = product.Name,
                    Price = product.Price,
                    ShortDescription = product.ShortDescription,
                    ProductImageName = product.ProductImageName,
                    ProductSelectedCategory = await _productRepository.GetAllProductCategoriesId(productId)
                };
            }

            return null;
        }

        public async Task<EditProductResult> EditProduct(EditProductViewModel editProduct, IFormFile ProductImage)
        {
            var product = await _productRepository.GetProductById(editProduct.ProductId);

            if (product == null) return EditProductResult.NotFound;

            if (editProduct.ProductSelectedCategory == null) return EditProductResult.NotProductSelectedCategoryHasNull;

            #region edit product

            product.ShortDescription = editProduct.ShortDescription;
            product.Description = editProduct.Description;
            product.IsActive = editProduct.IsActive;
            product.Price = editProduct.Price;
            product.Name = editProduct.Name;

            if (ProductImage != null && ProductImage.IsImage())
            {
                var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(ProductImage.FileName);

                ProductImage.AddImageToServer(imageName, PathExtensions.ProductOrginServer, 255, 273, PathExtensions.ProductThumbServer, product.ProductImageName);

                product.ProductImageName = imageName;
            }

            _productRepository.UpdateProduct(product);
            await _productRepository.SaveChanges();

            #endregion

            await _productRepository.RemoveProductSelectedCategories(editProduct.ProductId);
            await _productRepository.AddProductSelectedCategories(editProduct.ProductSelectedCategory, editProduct.ProductId);

            return EditProductResult.Success;
        }

        public async Task<bool> DeleteProduct(long productId)
        {
            return await _productRepository.DeleteProduct(productId);
        }

        public async Task<bool> RecoverProduct(long productId)
        {
            return await _productRepository.RecoverProduct(productId);

        }

        public async Task<bool> AddProductGallery(long productId, List<IFormFile> images)
        {
            if (!await _productRepository.CheckProduct(productId))
            {
                return false;
            }

            if (images != null && images.Any())
            {
                var productGallery = new List<ProductGalleries>();

                foreach (var image in images)
                {
                    if (image.IsImage())
                    {
                        var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(image.FileName);

                        image.AddImageToServer(imageName, PathExtensions.ProductOrginServer, 255, 273, PathExtensions.ProductThumbServer);

                        productGallery.Add(new ProductGalleries()
                        {
                            ImageName = imageName,
                            ProductId = productId
                        });
                    }
                }
                await _productRepository.AddProductGalleries(productGallery);
            }

            return true;
        }

        public async Task<List<ProductGalleriesViewModel>> ShowAllProductGalleries(long productId)
        {
            return await _productRepository.ShowAllProductGalleries(productId);
        }


        public async Task<List<ProductGalleries>> GetAllProductGalleries(long productId)
        {
            return await _productRepository.GetAllProductGalleries(productId);
        }

        public async Task DeleteImage(long galleryId)
        {
            var productGallery = await _productRepository.GetProductGallery(galleryId);

            if (productGallery != null)
            {
                UploadImageExtension.DeleteImage(productGallery.ImageName, PathExtensions.ProductOrginServer, PathExtensions.ProductThumbServer);

                await _productRepository.DeleteProductGallery(galleryId);
            }
        }

        public async Task<CreateProductFeatuersResult> CreateProductFeatuers(CreateProductFeatuersViewModel createProductFetuers)
        {
            if (!await _productRepository.CheckProduct(createProductFetuers.ProductId))
            {
                return CreateProductFeatuersResult.Error;
            }

            var newProductFeatuers = new ProductFeature()
            {
                FeatuerTitle = createProductFetuers.Title,
                FeatureValue = createProductFetuers.Value,
                ProductId = createProductFetuers.ProductId,
            };

            await _productRepository.AddProductFeatuers(newProductFeatuers);

            return CreateProductFeatuersResult.Success;
        }

        public async Task<List<ProductFeatuersViewModel>> ShowAllProductFeatuers(long productId)
        {
            return await _productRepository.ShowAllProductFeatuers(productId);
        }

        public async Task DeleteFeatuers(long id)
        {
            await _productRepository.DeleteFeatuers(id);
        }

        public async Task<List<ProductItemViewModel>> ShowAllProductInSlider()
        {
            return await _productRepository.ShowAllProductInSlider();
        }

        public async Task<List<ProductItemViewModel>> ShowAllProductInCategory(string hrefName)
        {
            return await _productRepository.ShowAllProductInCategory(hrefName);
        }

        public async Task<List<ProductItemViewModel>> LastProducts()
        {
            return await _productRepository.LastProducts();
        }

        public async Task<ProductDetailViewModel> ShowProductDetail(long ProductId)
        {
            return await _productRepository.ShowProductDetail(ProductId);
        }

        public async Task<CreateProductCommentResult> CreateProductComment(CreateProductCommentViewModel createProductComment, long userId)
        {
            var user = await _userRepository.GetUserById(userId);

            if (user == null)
            {
                return CreateProductCommentResult.CheckUser;
            }

            if (!await _productRepository.CheckProduct(createProductComment.ProductId))
            {
                return CreateProductCommentResult.CheckProduct;
            }

            var newComment = new ProductComment()
            {
                ProductId = createProductComment.ProductId,
                UserId = userId,
                Text = createProductComment.Text
            };

            await _productRepository.AddProductComment(newComment);
            await _productRepository.SaveChanges();

            return CreateProductCommentResult.Success;
        }

        public async Task<List<ProductComment>> AllProductCommentById(long ProductId)
        {
            return await _productRepository.AllProductCommentById(ProductId);
        }

        public async Task<List<ProductItemViewModel>> GetRelatedProduct(string categoryName, long productId)
        {
            return await _productRepository.GetRelatedProduct(categoryName, productId);
        }

        #endregion
    }
}
