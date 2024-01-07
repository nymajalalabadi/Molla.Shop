using Shop.Domain.Models.Account;
using Shop.Domain.ViewModels.Pigging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Account
{
    public class UserFavoritsViewModel : BasePaging
    {
        public List<UserFavorite> UserFavorites { get; set; }

        #region methods

        public UserFavoritsViewModel SetFavorites(List<UserFavorite> userFavorites)
        {
            this.UserFavorites = userFavorites;
            return this;
        }

        public UserFavoritsViewModel SetPaging(BasePaging paging)
        {
            this.PageId = paging.PageId;
            this.AllEntityCount = paging.AllEntityCount;
            this.StartPage = paging.StartPage;
            this.EndPage = paging.EndPage;
            this.TakeEntity = paging.TakeEntity;
            this.CountForShowAfterAndBefore = paging.CountForShowAfterAndBefore;
            this.SkipEntity = paging.SkipEntity;
            this.PageCount = paging.PageCount;

            return this;
        }

        #endregion
    }

}
