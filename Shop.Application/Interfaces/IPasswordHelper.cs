namespace Shop.Application.Interfaces
{
    public interface IPasswordHelper
    {
        string EncodePasswordMd5(string password);
    }
}
