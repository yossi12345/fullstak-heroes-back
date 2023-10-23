using heroes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;

namespace heroes.Data
{
    public class HeroesContext:IdentityDbContext<UserModel>
    {
        public HeroesContext(DbContextOptions<HeroesContext> options) : base(options) { }
        public DbSet<HeroModel> Heroes {get; set;}

    }
}
