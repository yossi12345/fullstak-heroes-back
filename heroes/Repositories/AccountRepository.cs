using heroes.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace heroes.Repositories
{ 
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly IConfiguration _configuration;
        public AccountRepository(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        public async Task<AccountResponseModel> SignUp(SignupModel signupModel)
        {
            UserModel user = new()
            {
                UserName = signupModel.Username
            };
            IdentityResult identityResult = await _userManager.CreateAsync(user, signupModel.Password);
            AccountResponseModel result = new();
            if (!identityResult.Succeeded) 
            {
                result.FailReason = "this username has already taken";
                return result;
            }
            result.Token = CreateNewToken(user.Id);
            result.Heroes = user.Heroes;
            result.Username = user.UserName;
            return result;
        }
        public async Task<AccountResponseModel> SignIn(SigninModel signinModel)
        {                                                            
            SignInResult signInResult = await _signInManager.PasswordSignInAsync(signinModel.Username, signinModel.Password, false, false);
            AccountResponseModel result = new();
            if (!signInResult.Succeeded)
            {
                result.FailReason = "the username or password are incorrect";
                return result;
            }
            UserModel user =await _userManager.Users.FirstAsync(u => u.UserName == signinModel.Username);
            result.Token = CreateNewToken(user.Id);
            result.Heroes = user.Heroes;
            result.Username = user.UserName;
            return result;
        }
        private string CreateNewToken(string id)
        {
            var authClaims = new List<Claim>
            {          
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var authSignKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSignKey, SecurityAlgorithms.HmacSha256Signature)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task GetUserByToken(string token)
        {
            
        }
    }
}
