using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Admin.Products
{
    public class ProductFeatuersViewModel
    {
        public long Id { get; set; }

        [Display(Name = "ویژگی")]
        public string Title { get; set; }

        [Display(Name = "مقدار ویژگی")]
        public string Value { get; set; }
    }
}
