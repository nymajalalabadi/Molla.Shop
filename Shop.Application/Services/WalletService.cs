using Shop.Application.Interfaces;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Wallet;
using Shop.Domain.ViewModels.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Services
{
    public class WalletService : IWalletService
    {
        #region constractore

        private readonly IWalletRepository _walletRepository;
        private readonly IUserRepository _userRepository;

        #endregion

        public WalletService(IWalletRepository walletRepository, IUserRepository userRepository)
        {
            _walletRepository = walletRepository;
            _userRepository = userRepository;
        }

        public async Task<long> ChargeWallet(long userId, ChargeWalletViewModel chargeWallet, string description)
        {
            var user = await _userRepository.GetUserById(userId);

            if (user == null) return 0;

            var wallet = new UserWallet()
            {
                UserId = user.Id,
                Description = description,
                Amount = chargeWallet.Amount,
                IsPay = false,
                WalletType = WalletType.Variz
            };

           await _walletRepository.CreateWallet(wallet);
           await _walletRepository.SaveChange();
            
            return wallet.Id;
        }

        public async Task<UserWallet> GetUserWalletById(long walletId)
        {
            return await _walletRepository.GetUserWalletById(walletId);
        }

        public async Task<bool> UpdateWalletForCharge(UserWallet wallet)
        {
            if (wallet != null)
            {
                wallet.IsPay = true;

                _walletRepository.UpdateWallet(wallet);
                await _walletRepository.SaveChange();

                return true;
            }
            return false;
        }

        public async Task<FilterWalletViewModel> FilterWallets(FilterWalletViewModel filter)
        {
            return await _walletRepository.FilterWallets(filter);
        }

        public async Task<int> GetUserWalletAmount(long userId)
        {
            return await _walletRepository.GetUserWalletAmount(userId);
        }

    }
}
