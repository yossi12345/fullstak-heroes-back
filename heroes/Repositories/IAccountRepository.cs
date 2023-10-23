using heroes.Models;
using Microsoft.AspNetCore.Identity;

namespace heroes.Repositories
{
    public interface IAccountRepository
    {
        Task<AccountResponseModel> SignUp(SignupModel signupModel);
        Task<AccountResponseModel> SignIn(SigninModel signinModel);
    }
}