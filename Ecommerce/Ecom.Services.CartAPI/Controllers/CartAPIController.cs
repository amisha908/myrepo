using AutoMapper;
using Ecom.Services.CartAPI.Data;
using Ecom.Services.CartAPI.Models;
using Ecom.Services.CartAPI.Models.Dto;
using Ecom.Services.CartAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Steeltoe.Common.Discovery;
using Steeltoe.Discovery;
using Steeltoe.Discovery.Eureka;
using System.Reflection.Metadata;

namespace Ecom.Services.CartAPI.Controllers
{
    //[Authorize]
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private IProductService _productService;
        private readonly DiscoveryHttpClientHandler _handler;


        public CartAPIController(AppDbContext db,
            IMapper mapper, IProductService productService, IDiscoveryClient discoveryClient)
        {
            _db = db;
            this._response = new ResponseDto();
            _mapper = mapper;
            _productService = productService;
            _handler = new DiscoveryHttpClientHandler(discoveryClient);
            
        }
        [Authorize]
        [HttpGet("Getproduct")]
        public async Task<ActionResult> Get21()
        {
            using var client = new HttpClient(_handler, false);
            var response = await client.GetAsync("http://ECOM.SERVICES.PRODUCTAPI/api/ProductAPI");
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        [Authorize]
        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
           // string user = HttpContent.User.Claims.FirstorDefault(c => c.Type == ClaimsTypes.NameIdentifier)?.value;
            try
            {
                CartDto cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.First(u => u.UserId == userId))
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails
                    .Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> productDtos = await _productService.GetProducts();

                foreach (var item in cart.CartDetails)
                {
                     item.Product = productDtos.FirstOrDefault(u => u.Id == item.ProductId);
                 //   cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                //apply coupon if any
                //if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                //{
                //    CouponDto coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                //    if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                //    {
                //        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                //        cart.CartHeader.Discount = coupon.DiscountAmount;
                //    }
                //}

                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking()
                   .FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //create header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                       u => u.ProductId == cartDto.CartDetails.First().ProductId &&
                       u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        //create cartdetails
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();

                    }
                    else
                    {
                        //update count in cart details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                }
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }
        [Authorize]
        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _db.CartDetails
                   .First(u => u.CartDetailsId == cartDetailsId);

                int totalCountofCartItem = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);
                if (totalCountofCartItem == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders
                       .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }






    }




}
