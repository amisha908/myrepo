using Ecom.Services.AuthAPI.Service.IService;
using Ecom.Services.AuthAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecom.Services.AuthAPI.RabbitMQSender;

namespace Ecom.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRabbitMQAuthMessageSender _rabbitMQSender;
        private readonly IConfiguration _configuration;
        protected ResponseDto _responseDto;
        public AuthAPIController(IAuthService authService, IConfiguration configuration, IRabbitMQAuthMessageSender rabbitMQAuthMessageSender)
        {
            _authService = authService;
            _rabbitMQSender = rabbitMQAuthMessageSender;
            _configuration = configuration;
            _responseDto = new();
        }

        /* [HttpPost("Register")]
         public async Task<IActionResult> Register([FromBody] RegisterationRequestDto model)
         {
             var errorMessage = await _authService.Register(model);
             if (!string.IsNullOrEmpty(errorMessage))
             {
                 _responseDto.IsSuccess = false;
                 _responseDto.Message = errorMessage;
                 return BadRequest(_responseDto);
             }
           //  _rabbitMQSender.SendMessage(model.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));
             return Ok(_responseDto);
         }
        */

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDto model)
        {
            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = errorMessage;
                return BadRequest(_responseDto);
            }
            var assignRoleSucessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
            //Invalid Authentication
            if (!assignRoleSucessful)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Error Encountered!";
                return BadRequest(_responseDto);
            }

              _rabbitMQSender.SendMessage(model.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));
            return Ok(_responseDto);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);
            //Invalid Authentication
            if (loginResponse.User == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Username or Password is incorrect!";
                return BadRequest(_responseDto);
            }
            _responseDto.Result = loginResponse;
            return Ok(_responseDto);
        }

        //[HttpPost("AssignRole")]
        //public async Task<IActionResult> AssignRole([FromBody] RegisterationRequestDto model)
        //{
        //    var assignRoleSucessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
        //    //Invalid Authentication
        //    if (!assignRoleSucessful)
        //    {
        //        _responseDto.IsSuccess = false;
        //        _responseDto.Message = "Error Encountered!";
        //        return BadRequest(_responseDto);
        //    }
        //    return Ok(_responseDto);
        //}
    }
}