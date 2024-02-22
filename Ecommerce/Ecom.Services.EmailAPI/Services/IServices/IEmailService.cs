namespace Ecom.Services.EmailAPI.Services.IServices
{
    public interface IEmailService
    {
        Task RegisterUserEmailAndLog(string email);
    }
}
