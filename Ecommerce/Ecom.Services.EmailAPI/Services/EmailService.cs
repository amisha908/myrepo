using Ecom.Services.EmailAPI.Data;
using Ecom.Services.EmailAPI.Models;
using Ecom.Services.EmailAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Ecom.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly DbContextOptions<AppDbContext> _dbOptions;

        public EmailService(DbContextOptions<AppDbContext> dbOptions)
        {
            this._dbOptions = dbOptions;
        }
    

        public async Task RegisterUserEmailAndLog(string email)
        {
            string message = "User Registered Successfully. <br/> Email : " + email;
            await LogAndEmail(message, "admin@ecomm.com");
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLog = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };

                await using var _db = new AppDbContext(_dbOptions);
                await _db.EmailLoggers.AddAsync(emailLog);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}