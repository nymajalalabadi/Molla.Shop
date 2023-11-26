using Shop.Domain.Models.Account;
using Shop.Domain.ViewModels.Pigging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Admin.Account
{
    public class FilterRolesViewModel : BasePaging
    {
        public string RoleName { get; set; }

        public List<Role> Roles { get; set; }

        #region method

        public FilterRolesViewModel SetRoles(List<Role> roles)
        {
            this.Roles = roles;
            return this;
        }

        public FilterRolesViewModel SetPaging(BasePaging paging)
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
