using Shop.Application.Interfaces;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Account;
using Shop.Domain.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Services
{
    public class UserService : IUserService
    {
        #region constractor

        private readonly IUserRepository _userRepository;

        private readonly IPasswordHelper _passwordHelper;

        public UserService(IUserRepository userRepository, IPasswordHelper passwordHelper)
        {
            _userRepository = userRepository;
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
                return RegisterUserResult.success;
            }
            return RegisterUserResult.MobileExists;
        }

        #endregion


    }
}
