using Shop.Domain.Models.Account;
using Shop.Domain.ViewModels.Admin.Account;

namespace Shop.Domain.Interfaces
{
    public interface IUserRepository
    {
        #region account

        Task<bool> IsUserExistPhoneNumber(string phoneNumber);

        Task CreateUser(User user);

        Task<User> GetUserByPhoneNumber(string phoneNumber);

        Task SaveChanges();

        void UpdateUser(User user);

        Task<User> GetUserById(long UserId);

        bool CheckPermission(long permissionId, string phoneNumber);

        #endregion

        #region admin

        Task<FilterUserViewModel> filterUsers(FilterUserViewModel filterUser);

        Task AddRoleToUser(List<long> selectedRole, long userId);

        Task<EditUserFromAdmin> GetEditUserFromAdmin(long userId);

        Task<CreateOrEditRoleViewModel> GetEditRoleById(long roleId);

        Task CreateRole(Role role);

        void UpdateRole(Role role);

        Task<Role> GetRoleById(long roleId);

        Task<FilterRolesViewModel> filterRoles(FilterRolesViewModel filterRoles);

        Task<List<Permission>> GetAllActivePermission();

        Task RomveAllPermissionSelectedRole(long roleId);

        Task AddPermissionToRole(List<long> selectedPermission, long roleId);

        Task<List<Role>> GetAllActiveRoles();

        Task RemoveAllUserSelectedRole(long userId);

        Task AddUserToRole(List<long> selectedRole, long userId);

        #endregion
    }
}
