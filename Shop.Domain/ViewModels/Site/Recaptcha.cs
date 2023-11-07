using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Site
{
    public class Recaptcha
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Token { get; set; }
    }
}
