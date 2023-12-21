using Shop.Domain.Models.BaseEntities;
using Shop.Domain.Models.ProductEntities;
using Shop.Domain.Models.Wallet;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.Models.Account
{
    public class User : BaseEntity
    {
        #region properties

        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string FirstName { get; set; }


        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string LastName { get; set; }


        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string PhoneNumber { get; set; }


        [Display(Name = "گذرواژه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Password { get; set; }


        [Display(Name = "آواتار")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Avatar { get; set; }


        [Display(Name = "مسدود شده / نشده")]
        public bool IsBlocked { get; set; }


        [Display(Name = "کد احرازهویت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string MobileActiveCode { get; set; }


        [Display(Name = "تایید شده / نشده")]
        public bool IsMobileActive { get; set; }


        [Display(Name = "جنسیت")]
        public UserGender UserGender { get; set; }

        #endregion


        #region relations

        public ICollection<UserWallet> UserWallets { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        public ICollection<ProductComment> ProductComments { get; set; }

        #endregion
    }

    public enum UserGender
    {
        [Display(Name = "آقا")]
        Mail,
        [Display(Name = "خانوم")]
        Femail,
        [Display(Name = "نامشخص")]
        Unknown
    }

}
