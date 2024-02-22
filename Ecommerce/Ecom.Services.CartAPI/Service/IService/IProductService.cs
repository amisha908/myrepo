using Ecom.Services.CartAPI.Models.Dto;

namespace Ecom.Services.CartAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
