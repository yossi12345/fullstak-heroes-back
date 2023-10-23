using heroes.Models;
using heroes.Repositories;
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
        [HttpPost("")]
        public async Task<IActionResult> Signup([FromBody] SignupModel signupModel) 
        {
            AccountResponseModel responseModel = await _accountRepository.SignUp(signupModel);

            return responseModel.FailReason != null ? Unauthorized(responseModel.FailReason) : Ok(responseModel);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Signin([FromBody] SigninModel signinModel)
        {
            AccountResponseModel responseModel = await _accountRepository.SignIn(signinModel);
            return responseModel.FailReason != null ? Unauthorized(responseModel.FailReason) : Ok(responseModel);
        }

    }
}
