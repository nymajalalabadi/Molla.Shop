using Shop.Domain.Models.Account;
using Shop.Domain.ViewModels.Pigging;
using System.Collections.Generic;

namespace Shop.Domain.ViewModels.Admin.Account
{
    public class FilterUserViewModel : BasePaging
    {
        public string PhoneNumber { get; set; }

        public List<User> Users { get; set; }

        #region methods

        public FilterUserViewModel SetUsers(List<User> users)
        {
            this.Users = users;
            return this;
        }

        public FilterUserViewModel SetPaging(BasePaging basePaging)
        {
            this.PageId = basePaging.PageId;
            this.AllEntityCount = basePaging.AllEntityCount;
            this.CountForShowAfterAndBefore = basePaging.CountForShowAfterAndBefore;
            this.StartPage = basePaging.StartPage;
            this.EndPage = basePaging.EndPage;
            this.TakeEntity = basePaging.TakeEntity;
            this.SkipEntity = basePaging.SkipEntity;
            this.PageCount = basePaging.PageCount;

            return this;
        }

        #endregion
    }
}
