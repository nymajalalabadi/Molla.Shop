using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Web.Permission;

namespace Shop.Web.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [PermissionChecker(5)]
    public class AdminBaseController : Controller
    {
        protected string ErrorMessage = "ErrorMessage";
        protected string SuccessMessage = "SuccessMessage";
        protected string WarningMessage = "WarningMessage";
        protected string InfoMessage = "InfoMessage";
    }
}
