using Microsoft.EntityFrameworkCore;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Account;
using Shop.Domain.ViewModels.Account;
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

        public bool CheckPermission(long permissionId, string phoneNumber)
        {
            var userId = _context.Users.AsQueryable().Single(c => c.PhoneNumber == phoneNumber).Id;

            var userRole = _context.UserRoles.AsQueryable()
                .Where(c => c.UserId == userId).Select(r => r.RoleId).ToList();


            if (!userRole.Any())
                return false;


            var permissions = _context.RolePermissions.AsQueryable()
                .Where(c => c.PermissionId == permissionId).Select(c => c.RoleId).ToList();


            return permissions.Any(c => userRole.Contains(c));
        }

        public async Task<bool> IsExistProductFavorite(long productId, long userId)
        {
            return await _context.UserFavorites.AsQueryable()
                .Where(c => !c.IsDelete)
                .AnyAsync(c => c.ProductId == productId && c.UserId == userId);
        }

        public async Task AddUserFavorite(UserFavorite userFavorite)
        {
            await _context.UserFavorites.AddAsync(userFavorite);
        }

        public async Task<bool> IsExistProductCompare(long productId, long userId)
        {
            return await _context.UserCompares.AsQueryable()
                .Where(c => !c.IsDelete)
                .AnyAsync(c => c.ProductId == productId && c.UserId == userId); 
        }

        public async Task AddUserComapre(UserCompare userCompare)
        {
            await _context.UserCompares.AddAsync(userCompare);
        }

        public async Task<List<UserCompare>> GetUserCompares(long userId)
        {
            return await _context.UserCompares.Include(c => c.Product).AsQueryable()
                .Where(c => c.UserId == userId && !c.IsDelete).ToListAsync();
        }

        public async Task<int> UserFavoriteCount(long userId)
        {
            return await _context.UserFavorites.Where(f => f.UserId == userId && !f.IsDelete).CountAsync();
        }

        public async Task<List<UserFavorite>> GetUserFavorite(long userId)
        {
            return await _context.UserFavorites.Include(c => c.Product).AsQueryable()
                .Where(c => c.UserId == userId && !c.IsDelete).ToListAsync();
        }

        public void UpdateUserComapre(UserCompare userCompare)
        {
            _context.UserCompares.Update(userCompare);
        }

        public async Task<UserCompare> GetUserCompare(long userId, long productId)
        {
            return await _context.UserCompares.AsQueryable()
                .Where(c => c.UserId == userId && c.ProductId == productId).FirstOrDefaultAsync();
        }

        public async Task<UserComparesViewModel> UserCompares(UserComparesViewModel userCompares)
        {
            var query = _context.UserCompares.Include(c => c.Product).ThenInclude(p => p.ProductFeatures).AsQueryable();

            #region paging

            var pager = Pager.Build(userCompares.PageId, await query.CountAsync(), userCompares.TakeEntity, userCompares.CountForShowAfterAndBefore);

            var allData = await query.Paging(pager).ToListAsync();

            #endregion

            return userCompares.SetPaging(pager).SetCompares(allData);
        }

        public async Task<UserFavoritsViewModel> UserFavorits(UserFavoritsViewModel userFavorits)
        {
            var query = _context.UserFavorites.Include(f => f.Product).AsQueryable();

            #region paging

            var pager = Pager.Build(userFavorits.PageId, await query.CountAsync(), userFavorits.TakeEntity, userFavorits.CountForShowAfterAndBefore);

            var allData = await query.Paging(pager).ToListAsync();

            #endregion

            return userFavorits.SetPaging(pager).SetFavorites(allData);
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

            var allData = await query.Paging(pager).ToListAsync();

            #endregion

            return filterUser.SetPaging(pager).SetUsers(allData);
        }

        public async Task AddRoleToUser(List<long> selectedRole, long userId)
        {
            if (selectedRole != null && selectedRole.Any())
            {
                var roleUser = new List<UserRole>();

                foreach (var roleId in selectedRole)
                {
                    roleUser.Add(new UserRole
                    {
                        RoleId = roleId,
                        UserId = userId
                    });
                }

                await _context.UserRoles.AddRangeAsync(roleUser);
            }
        }


        public async Task<EditUserFromAdmin> GetEditUserFromAdmin(long userId)
        {
            return await _context.Users.AsQueryable()
                .Include(r => r.UserRoles)
                .Where(u => u.Id == userId)
                .Select(x => new EditUserFromAdmin
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    UserGender = x.UserGender,
                    Password = x.Password,
                    RoleIds = x.UserRoles.Where(r => r.UserId == userId).Select(r => r.RoleId).ToList()
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
