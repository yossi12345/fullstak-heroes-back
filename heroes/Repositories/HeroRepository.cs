using heroes.Data;
using heroes.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace heroes.Repositories
{
    public class HeroRepository:IHeroRepository
    {
        private readonly HeroesContext _context;
        public HeroRepository(HeroesContext context) 
        {
            _context = context;
        }
        public async Task<HeroResponseModel> TryTrain(string userId,Guid heroId)
        {
            int heroRestTimeInMiliseconds = 86400000;
            UserModel? user = await _context.Users.Include(u => u.Heroes).FirstOrDefaultAsync(u => u.Id == userId);
            HeroResponseModel result = new();
            if (user == null)
            {
                result.ErrorMessage = "this user doesn't exist";
                result.Status = 401;
                return result;
            }
            HeroModel? hero = await _context.Heroes.Include(h => h.Owner).FirstOrDefaultAsync(h => h.Id == heroId);
            
            if (hero == null)
            {
                result.ErrorMessage = "this hero doesn't exist";
                result.Status = 404;
                return result;
            }
            if (hero.AmountOfTrainingsToday<5||(DateTime.Now - hero.LastTrainingDate).TotalMilliseconds > heroRestTimeInMiliseconds)
            {
                Train(hero);
                result.Status = 200;
            }
            else
            {
                DateTime nextAllowedTraingDate = hero.LastTrainingDate.AddMilliseconds(heroRestTimeInMiliseconds);
                result.ErrorMessage = "this hero too tired now. it can be trained in " + nextAllowedTraingDate.ToString("dd.MM.yy HH:mm");
                result.Status = 400;
            }
            result.Heroes = new() { hero };
            return result;
        }
        private void Train(HeroModel hero) 
        {
            hero.Level =(new Random().Next(10000, 100000))/ 100.0;
            hero.AmountOfTrainingsToday = hero.AmountOfTrainingsToday < 5 ? hero.AmountOfTrainingsToday + 1 : 1;
            hero.LastTrainingDate = DateTime.Now;
            _context.SaveChanges();
        }
        public async Task<HeroResponseModel> AddHeroToUser(string userId,Guid heroId)
        {
            UserModel? user=await _context.Users.Include(u=>u.Heroes).FirstOrDefaultAsync(u=>u.Id==userId);
            HeroResponseModel result = new();
            if (user==null)
            {
                result.ErrorMessage= "this user doesn't exist";
                result.Status = 401;
                return result;
            }
            HeroModel? hero=await _context.Heroes.Include(h=>h.Owner).FirstOrDefaultAsync(h=>h.Id==heroId);
            if (hero==null)
            {
                result.ErrorMessage ="this hero doesn't exist";
                result.Status = 404;
                return result;
            }
            if (hero.Owner?.Id == user.Id)
            {
                result.Heroes = user.Heroes.ToList();
                result.Status = 200;
                return result;
            }
            if (hero.Owner != null)
            {
                result.ErrorMessage = "this hero already belong to other user";
                result.Status = 400;
                return result;
            }
            user.Heroes.Add(hero);
            _context.SaveChanges();
            result.Heroes = user.Heroes.Select((h)=>new HeroModel()
            {

            }).ToList();
            result.Status = 200;
            return result;    
        }
        public async Task<HeroResponseModel> RemoveHeroFromUser(string userId, Guid heroId)
        {
            UserModel? user = await _context.Users.Include(u => u.Heroes).FirstOrDefaultAsync(u => u.Id == userId);
            HeroResponseModel result = new();
            if (user == null)
            {
                result.ErrorMessage="this user doesn't exist";
                result.Status = 401;
                return result;
            }
            HeroModel? hero = await _context.Heroes.FindAsync(heroId);
            if (hero == null)
            {
                result.ErrorMessage="this hero doesn't exist";
                result.Status = 404;
                return result;
            }
            bool isHeroRemovedSuccessfully=user.Heroes.Remove(hero);
            
            if (isHeroRemovedSuccessfully) 
            {               
                _context.SaveChanges();
            }
            result.Heroes = user.Heroes.ToList();
            result.Status= 200;
            return result;
        }
        public async Task<HeroResponseModel> GetAllHeroes(string userId)
        {
            UserModel? user = await _context.Users.FindAsync(userId);
            HeroResponseModel result= new();
            if (user == null)
            {
                result.ErrorMessage = "this user doesn't exist";
                result.Status = 401;
            }
            else
            {
                result.Heroes = await _context.Heroes.Include(h => h.Owner).Select((h) =>
                    new HeroModel
                    {
                        Owner = h.Owner == null ? null : new UserModel { UserName = h.Owner.UserName },
                        Id = h.Id,
                        Name = h.Name,
                        Description = h.Description,
                        AmountOfTrainingsToday = h.AmountOfTrainingsToday,
                        ImagePath = h.ImagePath,
                        Level = h.Level,
                        LastTrainingDate = h.LastTrainingDate
                    }).
                ToListAsync();
                result.Status = 200;
            }
            return result;
        }
        public async Task<HeroResponseModel> GetHero(string userId,Guid heroId)
        {
            UserModel? user = await _context.Users.FindAsync(userId);
            HeroResponseModel result = new();
            if (user == null)
            {
                result.ErrorMessage = "this user doesn't exist";
                result.Status = 401;
                return result;
            }
         
            HeroModel? hero = await _context.Heroes.Include(h=>h.Owner).FirstOrDefaultAsync(h=>h.Id==heroId);
            
            if (hero == null)
            {
                result.ErrorMessage = "this hero doesn't exist";
                result.Status = 404;
            }
            else
            {
                result.Heroes = new() {hero};
                result.Status = 200;
            }
            return result;
        }
        private HeroModel GetHeroToTransfer(HeroModel hero)
        {
            return new HeroModel()
            {
                Owner = hero.Owner == null ? null : new UserModel { UserName = hero.Owner.UserName },
                Id = hero.Id,
                Name = hero.Name,
                Description = hero.Description,
                AmountOfTrainingsToday = hero.AmountOfTrainingsToday,
                ImagePath = hero.ImagePath,
                Level = hero.Level,
                LastTrainingDate = hero.LastTrainingDate

            };
        }
     /*   public async Task<bool> CreateHero(string name, string imagePath)
        {
            await _context.Heroes.AddAsync(new HeroModel
            {
                Name = name,
                ImagePath = imagePath,
                Description= "dragons of lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem lorem",
                LastTrainingDate = DateTime.Now,
                Level = 0,
                AmountOfTrainingsToday=0
            });
            return _context.SaveChanges()>0; 
        }*/
    }
}
