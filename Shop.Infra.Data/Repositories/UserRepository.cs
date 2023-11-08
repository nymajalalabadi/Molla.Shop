using Microsoft.EntityFrameworkCore;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Account;
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

        #endregion
    }
}
