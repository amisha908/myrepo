using Ecom.Services.CartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Services.CartAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }

    }

    
}
