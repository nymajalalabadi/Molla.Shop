using Microsoft.EntityFrameworkCore;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Wallet;
using Shop.Domain.ViewModels.Pigging;
using Shop.Domain.ViewModels.Wallet;
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

        public async Task<FilterWalletViewModel> FilterWallets(FilterWalletViewModel filter)
        {
            var query = _context.UserWallets.AsQueryable();

            #region filter

            if (filter.UserId != 0 && filter.UserId != null)
            {
                query = query.Where(w => w.UserId == filter.UserId);
            }

            #endregion

            #region paging

            var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.CountForShowAfterAndBefore);

            var allData = await query.Paging(pager).ToListAsync();

            #endregion

            return filter.SetPaging(pager).SetWallets(allData);
        }

        #endregion
    }
}
