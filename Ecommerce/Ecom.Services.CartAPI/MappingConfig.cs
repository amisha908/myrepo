using AutoMapper;
using Ecom.Services.CartAPI.Models;
using Ecom.Services.CartAPI.Models.Dto;

namespace Ecom.Services.CartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
                {
                    config.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
                    config.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
                });
            return mappingConfig;
        
        }

    }
}
