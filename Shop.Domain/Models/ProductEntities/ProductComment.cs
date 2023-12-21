using Shop.Domain.Models.Account;
using Shop.Domain.Models.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.Models.ProductEntities
{
    public class ProductComment : BaseEntity
    {
        public long ProductId { get; set; }

        public long UserId { get; set; }

        [Display(Name = "متن")]
        [MaxLength(1000, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Text { get; set; }


        #region relation

        public Product Product { get; set; }

        public User User { get; set; }

        #endregion
    }
}
