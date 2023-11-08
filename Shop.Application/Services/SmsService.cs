using Shop.Application.Interfaces;

namespace Shop.Application.Services
{
    public class SmsService : ISmsService
    {
        public string ApiKey = "";

        public async Task SendVerificationCode(string mobile, string activeCode)
        {
            Kavenegar.KavenegarApi api = new Kavenegar.KavenegarApi(ApiKey);

            await api.VerifyLookup(mobile, activeCode,"Molla");
        }

    }
}
