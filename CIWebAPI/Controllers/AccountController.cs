using CI.BusinessLogic.Repository.Interface;
using CI.Models;
using CI.Utility.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CIWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [HttpResponseExceptionFilter]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        //[HttpPost("role")]
        //public async Task<IActionResult> Create([Required] string name)
        //{
        //        IdentityResult result = await _rolemanager.CreateAsync(new IdentityRole(name));
                
        //    return Ok();
        //}

        [HttpPost("register")]
        public async Task<IActionResult> Registration([FromBody]RegistrationModel register)
        {
            var result = await _accountRepository.SignUpAsync(register);

            if(result.Succeeded)
            {
                return Ok(register);
            }
            else
            {
                return BadRequest(result);
            }

            return Unauthorized();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var result = await _accountRepository.SignInAsync(login);

            if (string.IsNullOrEmpty(result))
            {
                return Unauthorized();
            }

            return Ok(result);
        }

        [HttpPost("forgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordModel forgetPassword)
        {
            var user = await _accountRepository.ForgetPasswordAsync(forgetPassword);

            if (user)
                return Ok(user);

            return Unauthorized();
        }

    }
}
