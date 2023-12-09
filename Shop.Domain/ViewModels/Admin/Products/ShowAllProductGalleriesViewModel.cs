using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Admin.Products
{
    public class ShowAllProductGalleriesViewModel
    {
        public List<ProductGalleriesViewModel> ProductGallery { get; set; }
    }
}
