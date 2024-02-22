using System.ComponentModel.DataAnnotations;

namespace Ecom.Services.ProductAPI.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Size { get; set; }
        public string Design { get; set; }
    }
}
