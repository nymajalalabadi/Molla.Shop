using Shop.Domain.Interfaces;
using Shop.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
