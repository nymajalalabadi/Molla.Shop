using Shop.Domain.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Interfaces
{
    public interface IUserService
    {
        #region account

        Task<RegisterUserResult> RegisterUser(RegisterUserViewModel register);

        #endregion
    }
}
