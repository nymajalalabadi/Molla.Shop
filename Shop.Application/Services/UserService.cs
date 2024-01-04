using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
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
                    FirstName = register.FirstName,
                    LastName = register.LastName,
                    UserGender = UserGender.Unknown,
                    PhoneNumber = register.PhoneNumber,
                    Password = _passwordHelper.EncodePasswordMd5(register.Password),
                    Avatar = "default.png",
                    IsMobileActive = false,
                    MobileActiveCode = new Random().Next(10000, 999999).ToString(),
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

            if (user.Password != _passwordHelper.EncodePasswordMd5(login.Password)) return LoginUserResult.NotFound;

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

            if (user != null)
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

        public bool CheckPermission(long permissionId, string phoneNumber)
        {
            return _userRepository.CheckPermission(permissionId, phoneNumber);
        }

        public async Task<bool> AddProductToFavorite(long userId, long productId)
        {
            if (!await _userRepository.IsExistProductFavorite(productId, userId))
            {
                //add
                var newUserFavorite = new UserFavorite()
                {
                    ProductId = productId,
                    UserId = userId
                };
                await _userRepository.AddUserFavorite(newUserFavorite);
                await _userRepository.SaveChanges();

                return true;
            }
            return false;
        }

        public async Task<bool> AddProductToCompare(long userId, long productId)
        {
            if (!await _userRepository.IsExistProductCompare(productId, userId))
            {
                //add
                var newUserCompare = new UserCompare()
                {
                    ProductId = productId,
                    UserId = userId
                };
                await _userRepository.AddUserComapre(newUserCompare);
                await _userRepository.SaveChanges();

                return true;
            }
            return false;
        }

        public async Task<List<UserCompare>> GetUserCompares(long userId)
        {
            return await _userRepository.GetUserCompares(userId);
        }

        public async Task<int> UserFavoriteCount(long userId)
        {
            return await _userRepository.UserFavoriteCount(userId);
        }

        public async Task<List<UserFavorite>> GetUserFavorite(long userId)
        {
            return await _userRepository.GetUserFavorite(userId);
        }

        public async Task<bool> RemoveAllUserComapre(long userId)
        {
            var allUserCompare = await _userRepository.GetUserCompares(userId);

            if (allUserCompare != null && allUserCompare.Any())
            {
                foreach (var item in allUserCompare)
                {
                    item.IsDelete = true;

                    _userRepository.UpdateUserComapre(item);
                }

                await _userRepository.SaveChanges();

                return true;
            }
            return false;
        }


        public async Task<bool> RemoveUserComapre(long userId, long productId)
        {
            var userCompare = await _userRepository.GetUserCompare(userId, productId);

            if (userCompare != null)
            {
                userCompare.IsDelete = true;

                _userRepository.UpdateUserComapre(userCompare);
                await _userRepository.SaveChanges();

                return true;
            }
            return false;
        }

        #endregion

        #region admin

        public async Task<FilterUserViewModel> filterUsers(FilterUserViewModel filterUser)
        {
            return await _userRepository.filterUsers(filterUser);
        }

        public async Task<CreateUserFromAdminResult> CreateUserForAdmin(CreateUserFromAdmin createUser)
        {
            if (!await _userRepository.IsUserExistPhoneNumber(createUser.PhoneNumber))
            {
                var user = new User()
                {
                    FirstName = createUser.FirstName,
                    LastName = createUser.LastName,
                    UserGender = createUser.UserGender,
                    PhoneNumber = createUser.PhoneNumber,
                    Password = _passwordHelper.EncodePasswordMd5(createUser.Password),
                    Avatar = "default.png",
                    IsMobileActive = true,
                    MobileActiveCode = new Random().Next(10000, 999999).ToString(),
                    IsBlocked = false,
                    IsDelete = false,
                };

                await _userRepository.CreateUser(user);
                await _userRepository.SaveChanges();

                await _userRepository.AddRoleToUser(createUser.RoleIds, user.Id);
                await _userRepository.SaveChanges();

                return CreateUserFromAdminResult.Success;
            }
            return CreateUserFromAdminResult.MobileExists;
        }


        public async Task<EditUserFromAdmin> GetEditUserFromAdmin(long userId)
        {
            return await _userRepository.GetEditUserFromAdmin(userId);
        }

        public async Task<EditUserFromAdminResult> EditUserFromAdmin(EditUserFromAdmin editUser)
        {
            var user = await _userRepository.GetUserById(editUser.Id);

            if (user == null)
            {
                return EditUserFromAdminResult.NotFound;
            }

            user.FirstName = editUser.FirstName;
            user.LastName = editUser.LastName;
            user.UserGender = editUser.UserGender;

            if (!string.IsNullOrEmpty(editUser.Password))
            {
                user.Password = _passwordHelper.EncodePasswordMd5(editUser.Password);
            }

            _userRepository.UpdateUser(user);
            await _userRepository.SaveChanges();

            await _userRepository.RemoveAllUserSelectedRole(editUser.Id);
            await _userRepository.AddUserToRole(editUser.RoleIds, editUser.Id);

            return EditUserFromAdminResult.Success;
        }

        public async Task<CreateOrEditRoleViewModel> GetEditRoleById(long roleId)
        {
            return await _userRepository.GetEditRoleById(roleId);
        }

        public async Task<CreateOrEditRoleResult> CreateOrEditRole(CreateOrEditRoleViewModel createOrEdit)
        {
            if (createOrEdit.Id != 0)
            {
                //Edit

                var role = await _userRepository.GetRoleById(createOrEdit.Id);

                if (role == null)
                    return CreateOrEditRoleResult.NotFound;

                role.RoleTitle = createOrEdit.RoleTitle;

                _userRepository.UpdateRole(role);

                await _userRepository.RomveAllPermissionSelectedRole(createOrEdit.Id);

                if (createOrEdit.SelectedPermission == null)
                    return CreateOrEditRoleResult.NotExistPermission;

                await _userRepository.AddPermissionToRole(createOrEdit.SelectedPermission, createOrEdit.Id);

                await _userRepository.SaveChanges();

                return CreateOrEditRoleResult.Success;
            }
            else
            {
                //Create

                var newRole = new Role()
                {
                    RoleTitle = createOrEdit.RoleTitle,
                };

                await _userRepository.CreateRole(newRole);

                await _userRepository.SaveChanges();

                if (createOrEdit.SelectedPermission == null)
                    return CreateOrEditRoleResult.NotExistPermission;

                await _userRepository.AddPermissionToRole(createOrEdit.SelectedPermission, newRole.Id);


                await _userRepository.SaveChanges();

                return CreateOrEditRoleResult.Success;
            }
        }

        public async Task<FilterRolesViewModel> filterRoles(FilterRolesViewModel filterRoles)
        {
            return await _userRepository.filterRoles(filterRoles);
        }

        public async Task<List<Permission>> GetAllActivePermission()
        {
            return await _userRepository.GetAllActivePermission();
        }
        public async Task<List<Role>> GetAllActiveRoles()
        {
            return await _userRepository.GetAllActiveRoles();
        }

       

        #endregion
    }
}
