using Microsoft.AspNetCore.Identity;

namespace heroes.Models
{
    public class UserModel: IdentityUser
    {
        public ICollection<HeroModel> Heroes { get; set; }=new List<HeroModel>();
    }
}
