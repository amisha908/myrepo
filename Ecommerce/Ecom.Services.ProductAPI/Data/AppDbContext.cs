using Ecom.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Services.ProductAPI.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) 
        {
            
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = 1,
                Name = "Book",
                Description = "The book name is 'How to Rule'",
                Price = 60,
                Size = "",
                Design = ""
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = 2,
                Name = "Phone",
                Description = "The new launch",
                Price = 60000,
                Size = "",
                Design = "Premium"
            });

        }

    }
}
