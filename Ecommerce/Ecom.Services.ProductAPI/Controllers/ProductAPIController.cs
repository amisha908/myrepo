using AutoMapper;
using Azure;
using Ecom.Services.ProductAPI.Data;
using Ecom.Services.ProductAPI.Models;
using Ecom.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.Services.ProductAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;
        public ProductAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
        }
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Product> objList = _db.Products.ToList();
                _response.Result= _mapper.Map<IEnumerable<ProductDto>>(objList);
            }
            catch (Exception ex) 
            {
                _response.IsSuccess=false;
                _response.Message=ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Product obj = _db.Products.FirstOrDefault(p => p.Id == id);

                _response.Result= _mapper.Map<ProductDto>(obj);
                
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;

            }
            return _response;
        }

        [HttpPost]
        [Authorize("ADMIN")]
        public ResponseDto Post([FromBody] ProductDto productDto)
        {
            try
            {

                Product obj = _mapper.Map<Product>(productDto);
                _db.Products.Add(obj);
                _db.SaveChanges();


                _response.Result = _mapper.Map<ProductDto>(obj);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;

            }
            return _response;
        }

        [HttpPut("{id:int}")]
        [Authorize("ADMIN")]
        public ResponseDto Put([FromBody] ProductDto productDto)
        {
            try
            {

                Product obj = _mapper.Map<Product>(productDto);
                _db.Products.Update(obj);
                _db.SaveChanges();


                _response.Result = _mapper.Map<ProductDto>(obj);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;

            }
            return _response;
        }

        [HttpDelete("{id:int}")]
        [Authorize("ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {

                Product obj = _db.Products.First(p => p.Id == id);
                _db.Products.Remove(obj);
                _db.SaveChanges();


               
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;

            }
            return _response;
        }

    }
}
