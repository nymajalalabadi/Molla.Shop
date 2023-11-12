using Microsoft.EntityFrameworkCore;
using Shop.Domain.Models.Account;
using Shop.Domain.ViewModels.Account;

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
    }
}
