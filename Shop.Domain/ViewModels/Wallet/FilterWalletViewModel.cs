using Shop.Domain.Models.Wallet;
using Shop.Domain.ViewModels.Pigging;

namespace Shop.Domain.ViewModels.Wallet
{
    public class FilterWalletViewModel : BasePaging
    {
        #region properties

        public long? UserId { get; set; }

        public List<UserWallet> UserWallets { get; set; }

        #endregion

        #region methods

        public FilterWalletViewModel SetWallets(List<UserWallet> wallets)
        {
            this.UserWallets = wallets;
            return this;
        }

        public FilterWalletViewModel SetPaging(BasePaging basePaging)
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
