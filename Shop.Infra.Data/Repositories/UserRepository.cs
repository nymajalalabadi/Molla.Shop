using Microsoft.EntityFrameworkCore;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Account;
using Shop.Domain.ViewModels.Admin.Account;
using Shop.Domain.ViewModels.Pigging;
using Shop.Infra.Data.Context;

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
            var user =  await _context.Users.AsQueryable()
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

        #endregion
    }
}
