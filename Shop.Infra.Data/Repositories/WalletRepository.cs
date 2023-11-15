using Microsoft.EntityFrameworkCore;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Wallet;
using Shop.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Infra.Data.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        #region constractore

        private readonly ShopDbContext _context;

        public WalletRepository(ShopDbContext context)
        {
            _context = context;
        }

        #endregion

        #region wallet

        public async Task CreateWallet(UserWallet userWallet)
        {
            await _context.UserWallets.AddAsync(userWallet);
        }

        public async Task<UserWallet> GetUserWalletById(long walletId)
        {
            return await _context.UserWallets.AsQueryable().SingleOrDefaultAsync(w => w.Id == walletId);
        }

        public async Task SaveChange()
        {
            await _context.SaveChangesAsync();
        }

        public void UpdateWallet(UserWallet userWallet)
        {
            _context.UserWallets.Update(userWallet);
        }

        #endregion
    }
}
