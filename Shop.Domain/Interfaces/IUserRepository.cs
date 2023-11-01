namespace Shop.Domain.Interfaces
{
    public interface IUserRepository
    {
        #region account

        Task<bool> IsUserExistPhoneNumber(string phoneNumber);

        #endregion
    }
}
