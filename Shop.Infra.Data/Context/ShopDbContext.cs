using Microsoft.EntityFrameworkCore;
using Shop.Domain.Models.Account;
using Shop.Domain.Models.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Infra.Data.Context
{
    public class ShopDbContext : DbContext
    {
        #region constractor

        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {

        }

        #endregion


        #region user

        public DbSet<User> Users { get; set; }

        public DbSet<UserWallet> UserWallets { get; set; }

        #endregion
    }
}
