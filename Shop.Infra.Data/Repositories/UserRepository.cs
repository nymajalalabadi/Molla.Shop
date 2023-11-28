using Microsoft.EntityFrameworkCore;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Account;
using Shop.Domain.ViewModels.Admin.Account;
using Shop.Domain.ViewModels.Pigging;
using Shop.Infra.Data.Context;
using System.Security;

namespace Shop.Infra.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        #region constractore

        private readonly ShopDbContext _context;

        public UserRepository(ShopDbContext context)
        {
            _context = context;
        }

        #endregion

        #region account

        public async Task<bool> IsUserExistPhoneNumber(string phoneNumber)
        {
            return await _context.Users.AsQueryable().AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task CreateUser(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User> GetUserByPhoneNumber(string phoneNumber)
        {
            var user = await _context.Users.AsQueryable()
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (user == null)
            {
                return null;
            }

            return user;
        }


        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }

        public async Task<User> GetUserById(long UserId)
        {
            return await _context.Users.AsQueryable().SingleOrDefaultAsync(u => u.Id == UserId);
        }

        #endregion

        #region admin

        public async Task<FilterUserViewModel> filterUsers(FilterUserViewModel filterUser)
        {
            var query = _context.Users.AsQueryable();

            #region filter

            if (!string.IsNullOrEmpty(filterUser.PhoneNumber))
            {
                query = query.Where(u => u.PhoneNumber == filterUser.PhoneNumber);
            }

            #endregion

            #region paging

            var pager = Pager.Build(filterUser.PageId, await _context.Users.CountAsync(), filterUser.TakeEntity, filterUser.CountForShowAfterAndBefore);

            var AllData = await query.Paging(pager).ToListAsync();

            #endregion

            return filterUser.SetPaging(pager).SetUsers(AllData);
        }


        public async Task<EditUserFromAdmin> GetEditUserFromAdmin(long userId)
        {
            return await _context.Users.AsQueryable()
                .Where(u => u.Id == userId)
                .Select(x => new EditUserFromAdmin
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    UserGender = x.UserGender,
                    Password = x.Password
                }).SingleOrDefaultAsync();
        }


        public async Task<CreateOrEditRoleViewModel> GetEditRoleById(long roleId)
        {
            return await _context.Roles.AsQueryable()
                .Include(r => r.RolePermissions)
                .Where(r => r.Id == roleId)
                .Select(x => new CreateOrEditRoleViewModel
                {
                    Id = x.Id,
                    RoleTitle = x.RoleTitle,
                    SelectedPermission = x.RolePermissions.Select(p => p.PermissionId).ToList()

                }).SingleOrDefaultAsync();
        }

        public async Task CreateRole(Role role)
        {
            await _context.Roles.AddAsync(role);
        }


        public void UpdateRole(Role role)
        {
            _context.Roles.Update(role);
        }

        public async Task<Role> GetRoleById(long roleId)
        {
            return await _context.Roles.SingleOrDefaultAsync(r => r.Id == roleId);
        }

        public async Task<FilterRolesViewModel> filterRoles(FilterRolesViewModel filterRoles)
        {
            var query = _context.Roles.AsQueryable();

            #region filter

            if (!string.IsNullOrEmpty(filterRoles.RoleName))
            {
                query = query.Where(c => EF.Functions.Like(c.RoleTitle, $"%{filterRoles.RoleName}%"));
            }

            #endregion

            #region paging

            var pager = Pager.Build(filterRoles.PageId, await _context.Roles.CountAsync(), filterRoles.TakeEntity, filterRoles.CountForShowAfterAndBefore);

            var allData = await query.Paging(pager).ToListAsync();

            #endregion

            return filterRoles.SetPaging(pager).SetRoles(allData);
        }

        public async Task<List<Permission>> GetAllActivePermission()
        {
            return await _context.Permissions.Where(p => !p.IsDelete).ToListAsync();
        }

        public async Task RomveAllPermissionSelectedRole(long roleId)
        {
            var AllRolePermission = await _context.RolePermissions.Where(r => r.RoleId == roleId).ToListAsync();

            if (AllRolePermission.Any())
            {
                _context.RemoveRange(AllRolePermission);
            }
        }

        public async Task AddPermissionToRole(List<long> selectedPermission, long roleId)
        {
            if (selectedPermission != null && selectedPermission.Any())
            {
                var rolePermission = new List<RolePermission>();

                foreach (var permissionId in selectedPermission)
                {
                    rolePermission.Add(new RolePermission
                    {
                        PermissionId = permissionId,
                        RoleId = roleId
                    });
                }

                await _context.RolePermissions.AddRangeAsync(rolePermission);

            }
        }

        public async Task<List<Role>> GetAllActiveRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task RemoveAllUserSelectedRole(long userId)
        {
            var AllRole = await _context.UserRoles.AsQueryable().Where(u => u.UserId == userId).ToListAsync();

            if (AllRole.Any())
            {
                _context.RemoveRange(AllRole);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddUserToRole(List<long> selectedRole, long userId)
        {
            if (selectedRole != null && selectedRole.Any())
            {
                var userRole = new List<UserRole>();

                foreach (var roleId in selectedRole)
                {
                    userRole.Add(new UserRole
                    {
                        RoleId = roleId,
                        UserId = userId
                    });
                }

                await _context.UserRoles.AddRangeAsync(userRole);
                await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
