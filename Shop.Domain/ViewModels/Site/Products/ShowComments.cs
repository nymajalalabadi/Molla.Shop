using Shop.Domain.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Site.Products
{
    public class ShowComments
    {
        [Display(Name = "متن")]
        [MaxLength(1000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Text { get; set; }

        public DateTime CreateDate { get; set; }

        public User User { get; set; }
    }
}
