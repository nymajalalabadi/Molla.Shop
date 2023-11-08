using Shop.Application.Interfaces;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Account;
using Shop.Domain.ViewModels.Account;

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

        #endregion


    }
}
