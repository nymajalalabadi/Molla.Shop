using Shop.Domain.Models.ProductEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Site.Products
{
    public class ProductDetailViewModel
    {
        #region properties

        public long ProductId { get; set; }

        [Display(Name = "نام محصول")]
        public string Name { get; set; }

        [Display(Name = "توضیحات کوتاه")]
        public string ShortDescription { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = "قیمت محصول")]
        public int Price { get; set; }

        [Display(Name = "تصویر محصول")]
        public string ProductImageName { get; set; }

        public int ProductComment { get; set; }

        public ProductCategory ProductCategory { get; set; }

        public List<string> ProductImages { get; set; }

        public List<ProductFeature> ProductFeatures { get; set; }

        #endregion
    }
}
