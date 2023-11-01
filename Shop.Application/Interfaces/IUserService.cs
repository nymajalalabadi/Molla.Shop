using Shop.Domain.ViewModels.Account;

namespace Shop.Application.Interfaces
{
    public interface IUserService
    {
        #region account

        Task<RegisterUserResult> RegisterUser(RegisterUserViewModel register);

        #endregion
    }
}
