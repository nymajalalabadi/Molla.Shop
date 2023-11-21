using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shop.Application.Extentions;
using Shop.Application.Interfaces;
using Shop.Application.Utils;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Account;
using Shop.Domain.ViewModels.Account;
using Shop.Domain.ViewModels.Admin.Account;

namespace Shop.Application.Services
{
    public class UserService : IUserService
    {
        #region constractor

        private readonly IUserRepository _userRepository;

        private readonly ISmsService _smsService;

        private readonly IPasswordHelper _passwordHelper;

        public UserService(IUserRepository userRepository, ISmsService smsService, IPasswordHelper passwordHelper)
        {
            _userRepository = userRepository;
            _smsService = smsService;
            _passwordHelper = passwordHelper;
        }

        #endregion


        #region account

        public async Task<RegisterUserResult> RegisterUser(RegisterUserViewModel register)
        {
            if (!await _userRepository.IsUserExistPhoneNumber(register.PhoneNumber))
            {
                var user = new User()
                {
                    FirstName =register.FirstName,
                    LastName =register.LastName,
                    UserGender = UserGender.Unknown,
                    PhoneNumber = register.PhoneNumber,
                    Password = _passwordHelper.EncodePasswordMd5(register.Password),
                    Avatar = "default.png",
                    IsMobileActive = false,
                    MobileActiveCode = new Random().Next(10000,999999).ToString(),
                    IsBlocked = false,
                    IsDelete = false,
                };

                await _userRepository.CreateUser(user);
                await _userRepository.SaveChanges();

                await _smsService.SendVerificationCode(user.PhoneNumber, user.MobileActiveCode);

                return RegisterUserResult.success;
            }
            return RegisterUserResult.MobileExists;
        }

        public async Task<LoginUserResult> LoginUser(LoginUserViewModel login)
        {
            var user = await _userRepository.GetUserByPhoneNumber(login.PhoneNumber);

            if (user == null) return LoginUserResult.NotFound;

            if (user.IsBlocked) return LoginUserResult.IsBlocked;

            if (!user.IsMobileActive) return LoginUserResult.NotActive;

            if(user.Password != _passwordHelper.EncodePasswordMd5(login.Password)) return LoginUserResult.NotFound;

            return LoginUserResult.Success;
        }

        public async Task<User> GetUserByPhoneNumber(string phoneNumber)
        {
            return await _userRepository.GetUserByPhoneNumber(phoneNumber);
        }

        public async Task<ActiveAccountResult> ActiveAccount(ActiveAccountViewModel activeAccount)
        {
            var user = await GetUserByPhoneNumber(activeAccount.PhoneNumber);

            if (user == null) return ActiveAccountResult.NotFound;

            if (user.MobileActiveCode == activeAccount.ActiveCode)
            {
                user.MobileActiveCode = new Random().Next(10000, 999999).ToString();
                user.IsMobileActive = true;

                _userRepository.UpdateUser(user);
                await _userRepository.SaveChanges();

                return ActiveAccountResult.Success;
            }
            return ActiveAccountResult.Error;
        }

        public async Task<User> GetUserById(long UserId)
        {
            return await _userRepository.GetUserById(UserId);
        }
        #endregion

        #region profile

        public async Task<EditUserProfileViewModel> GetEditUserProfile(long userId)
        {
            var user = await _userRepository.GetUserById(userId);

            if (user == null) return null;

            return new EditUserProfileViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                UserGender = user.UserGender
            };
        }

        public async Task<EditUserProfileResult> EditProfile(long userId, IFormFile userAvatar, EditUserProfileViewModel editUserProfile)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null) return EditUserProfileResult.NotFound;

            user.FirstName = editUserProfile.FirstName;
            user.LastName = editUserProfile.LastName;
            user.UserGender = editUserProfile.UserGender;

            if (userAvatar != null && userAvatar.IsImage())
            {
                var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(userAvatar.FileName);

                userAvatar.AddImageToServer(imageName, PathExtensions.UserAvatarOrginServer, 150, 150, PathExtensions.UserAvatarThumbServer);

                user.Avatar = imageName;
            }

            _userRepository.UpdateUser(user);
            await _userRepository.SaveChanges();

            return EditUserProfileResult.Success;

        }

        public async Task<ChangePasswordResult> ChangePassword(long userId, ChangePasswordViewModel changePassword)
        {
            var user = await _userRepository.GetUserById(userId);

            if(user != null)
            {
                var currentPassword = _passwordHelper.EncodePasswordMd5(changePassword.CurrentPassword);

                if (user.Password == currentPassword)
                {
                    var newPassword = _passwordHelper.EncodePasswordMd5(changePassword.NewPassword);

                    if (user.Password != newPassword)
                    {
                        user.Password = newPassword;
                        _userRepository.UpdateUser(user);
                        await _userRepository.SaveChanges();

                        return ChangePasswordResult.Success;
                    }

                    return ChangePasswordResult.PasswordEqual;
                }
                return ChangePasswordResult.Failed;
            }

            return ChangePasswordResult.NotFound;
        }

        #endregion

        #region admin

        public async Task<FilterUserViewModel> filterUsers(FilterUserViewModel filterUser)
        {
            return await _userRepository.filterUsers(filterUser);
        }

        #endregion
    }
}
