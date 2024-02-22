using Microsoft.AspNetCore.Identity;

namespace Ecom.Services.AuthAPI.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }

    }
}
