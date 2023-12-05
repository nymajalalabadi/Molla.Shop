using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;

namespace Shop.Web.Permission
{
    public class PermissionCheckerAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        #region constractor

        private IUserService _userService;
        private long _permissionId = 0;

        public PermissionCheckerAttribute(long permissionId)
        {
            _permissionId = permissionId;
        }

        #endregion

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _userService = (IUserService)context.HttpContext.RequestServices.GetService(typeof(IUserService));

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var phoneNumber = context.HttpContext.User.Identity.Name;

                if (!_userService.CheckPermission(_permissionId, phoneNumber))
                {
                    context.Result = new RedirectResult("/Login");
                }
            }
            else
            {
                context.Result = new RedirectResult("/Login");
            }
        }
    }

}
