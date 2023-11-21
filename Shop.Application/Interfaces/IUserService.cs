using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Models.Account;
using Shop.Domain.ViewModels.Account;
using Shop.Domain.ViewModels.Admin.Account;

namespace Shop.Application.Interfaces
{
    public interface IUserService
    {
        #region account

        Task<RegisterUserResult> RegisterUser(RegisterUserViewModel register);

        Task<LoginUserResult> LoginUser(LoginUserViewModel login);

        Task<User> GetUserByPhoneNumber(string phoneNumber);

        Task<ActiveAccountResult> ActiveAccount(ActiveAccountViewModel activeAccount);

        Task<User> GetUserById(long UserId);

        #endregion

        #region profile

        Task<EditUserProfileViewModel> GetEditUserProfile(long userId);

        Task<EditUserProfileResult> EditProfile(long userId, IFormFile userAvatar, EditUserProfileViewModel editUserProfile);

        Task<ChangePasswordResult> ChangePassword(long userId, ChangePasswordViewModel changePassword);

        #endregion

        #region admin

        Task<FilterUserViewModel> filterUsers(FilterUserViewModel filterUser);

        #endregion
    }
}
