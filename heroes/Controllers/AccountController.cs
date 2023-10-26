using heroes.Models;
using heroes.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace heroes.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController:ControllerBase
    {

        private readonly IAccountRepository _accountRepository;
        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        [HttpGet("g")]
        public IActionResult g()
        {
            return Ok(new { token="UHFD"});
        }
        [HttpPost("")]
        public async Task<IActionResult> Signup([FromBody] SignupModel signupModel) 
        {
            AccountResponseModel responseModel = await _accountRepository.SignUp(signupModel);

            return responseModel.FailReason != null ? Unauthorized(responseModel.FailReason) : Ok(responseModel);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Signin([FromBody] SigninModel signinModel)
        {
            AccountResponseModel response = await _accountRepository.SignIn(signinModel);
            return response.FailReason != null ? Unauthorized(response.FailReason) : Ok(response);
        }
        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> GetUser()
        {
            string? userId = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("you didn't send a token");
            }
            var user = await _accountRepository.GetUserById(userId);
            if (user == null)
            {
                return Unauthorized("this token isn't good");
            }
            return Ok(user);
        }

    }
}
