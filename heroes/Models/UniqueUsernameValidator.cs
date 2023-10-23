using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace heroes.Models
{
    public class UniqueUsernameValidator<TUser> : IUserValidator<TUser> where TUser : IdentityUser
    {
        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            var errors = new List<IdentityError>();
            bool isUsernameExist = await manager.Users.AnyAsync(u => u.UserName == user.UserName);
            if (isUsernameExist)
            {
                errors.Add(new IdentityError
                {
                    Code = "DuplicateUserName",
                    Description = "Username is already taken."
                });
            }

            return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }
}
