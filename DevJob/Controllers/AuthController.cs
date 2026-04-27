using DevJob.Application.DTOs.Auth;
using DevJob.Domain.Entities;
using DevJob.Application.ServiceContract;
using DevJob.Application.Validation;
using DevJob.Infrastructure.Service;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace DevJob.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RegisterValidation validations;

        private readonly IAuthService authService;
        private readonly IJwtServices jwtServices;
       private readonly IValidator<CompanyRegisterDTO> validation;
        public AuthController(IAuthService authService,IJwtServices jwtServices, IValidator<CompanyRegisterDTO> validations)
        {
            this.authService = authService;
            this.jwtServices = jwtServices;
            this.validation = validations;
        }
        [HttpPost("user-Register")]
        public async Task<IActionResult> Register (RegisterDTO registerDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await authService.UserRegister(registerDTO);
                if (result.Success)
                    return Ok(result);
                return BadRequest(result);
            }
            return BadRequest();
        }
        [HttpPost("Login")]
        public async Task <IActionResult>Login(LoginDTO loginDTO)
        {
            var result = await authService.Login(loginDTO);
            if (result.Success)
            {
                return Ok(result);
            }
            return Unauthorized(result);
        }
        //[HttpPost("google-login")]
        //public async Task <IActionResult>LoginByGoogle([FromQuery] string token)
        //{
        //    var response = await authService.GoogleResponse(token);
        //    if (response.Success)
        //        return Ok(response);
        //    return Unauthorized(response);
        //}
      
        [Authorize]
        [HttpGet]
        public IActionResult Test()
        {
            return Ok("Hello");
        }
        [HttpGet("confirm-email")]
        public async Task< IActionResult> ConfirmEamil (string email,string token)
        {
            // return Ok("Email Confirmed");
           var result=await authService.ConfirmEmail(email,token);
            if (result.Success)
                return Ok(result);
            return BadRequest(result.Message);
        }
        [HttpPost("Generate-new-jwt-token")]
        public async Task< IActionResult> GenerateNewJwtToken(TokenModel tokenModel)
        {
          var response =await  jwtServices.GenerateNewAccessToken(tokenModel);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);

        }
        [HttpPost("Forget-Password")]
        public async Task< IActionResult> ForgetPassword([FromBody] ForgetPasswordDTO forgetPasswordDTO)
        {
            if (!ModelState.IsValid)
                return  BadRequest();
          var result =  await authService.ForgetPassword(forgetPasswordDTO);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("Resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result =await authService.ResetPassword(resetPasswordDTO);
            if (result.Success)
                return Ok();
            return BadRequest(result);
        }
        [HttpPost("Register")]
        public async Task<IActionResult> CompanyRegister(CompanyRegisterDTO companyRegisterDTO)
        {

            var validationResult = await validation.ValidateAsync(companyRegisterDTO);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);
            var response = await authService.CompanyRegister(companyRegisterDTO);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            var result = await authService.ResendConfirmationEmail(email);
            return Ok(result);
        }

    }
}
