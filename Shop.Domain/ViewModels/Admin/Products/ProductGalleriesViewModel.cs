using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Admin.Products
{
    public class ProductGalleriesViewModel
    {
        public long Id { get; set; }

        public long ProductId { get; set; }

        [Display(Name = "تصویر محصول")]
        public string ImageName { get; set; }
    }
}
