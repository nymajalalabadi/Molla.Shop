using System.ComponentModel.DataAnnotations;
using Shop.Domain.Models.Account;

namespace Shop.Domain.ViewModels.Admin.Account
{
    public class EditUserFromAdmin
    {
        public long Id { get; set; }

        [Display(Name = "نام")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string FirstName { get; set; }


        [Display(Name = "نام خانوادگی")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string LastName { get; set; }


        [Display(Name = "شماره تلفن همراه")]
        public string? PhoneNumber { get; set; }


        [Display(Name = "جنسیت")]
        public UserGender UserGender { get; set; }

        [Display(Name = "گذرواژه")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Password { get; set; }

        public List<long> RoleIds { get; set; }
    }

    public enum EditUserFromAdminResult
    {
        NotFound,
        Success
    }

}
