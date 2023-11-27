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

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RolePermission>()
                .HasOne(b => b.Role)
                .WithMany(b => b.RolePermissions)
                .HasForeignKey(b => b.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(b => b.Permission)
                .WithMany(b => b.RolePermissions)
                .HasForeignKey(b => b.PermissionId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
