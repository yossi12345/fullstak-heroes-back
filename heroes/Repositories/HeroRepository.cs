using heroes.Data;
using heroes.Models;
using Microsoft.EntityFrameworkCore;

namespace heroes.Repositories
{
    public class HeroRepository:IHeroRepository
    {
        private readonly HeroesContext _context;
        public HeroRepository(HeroesContext context) 
        {
            _context = context;
        }
        public async Task<HeroResponseModel> TryTrain(string userId,string heroId)
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
            bool isConvertingToGuidSucceeded = Guid.TryParse(heroId, out var heroIdGuid);
            HeroModel? hero = null;
            if (isConvertingToGuidSucceeded)
            {
                hero = await _context.Heroes.Include(h => h.Owner).FirstOrDefaultAsync(h => h.Id == heroIdGuid);
            }
            if (hero == null)
            {
                result.ErrorMessage = "this hero doesn't exist";
                result.Status = 404;
                return result;
            }
            if (hero.LastTrainingDate==null||hero.AmountOfTrainingsToday<5||(DateTime.Now - hero.LastTrainingDate).TotalMilliseconds > heroRestTimeInMiliseconds)
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
        public async Task<HeroResponseModel> AddHeroToUser(string userId,string heroId)
        {
            UserModel? user=await _context.Users.Include(u=>u.Heroes).FirstOrDefaultAsync(u=>u.Id==userId);
            HeroResponseModel result = new();
            if (user==null)
            {
                result.ErrorMessage= "this user doesn't exist";
                result.Status = 401;
                return result;
            }
            HeroModel? hero=await _context.Heroes.FindAsync(heroId);
            if (hero==null)
            {
                result.ErrorMessage ="this hero doesn't exist";
                result.Status = 404;
                return result;
            }
            result.Heroes = new() { hero };
            result.Status= 200;
            if (!user.Heroes.Contains(hero))
            {
                user.Heroes.Add(hero);
                _context.SaveChanges();
            }
            return result;    
        }
        public async Task<HeroResponseModel> RemoveHeroFromUser(string userId, string heroId)
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
            result.Heroes = new() { hero };
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
                result.Heroes = await _context.Heroes.Include(h=>h.Owner).ToListAsync();
                result.Status = 200;
            }
            return result;
        }
        public async Task<HeroResponseModel> GetHero(string userId,string heroId)
        {
            UserModel? user = await _context.Users.FindAsync(userId);
            HeroResponseModel result = new();
            if (user == null)
            {
                result.ErrorMessage = "this user doesn't exist";
                result.Status = 401;
                return result;
            }
            bool isConvertingToGuidSucceeded=Guid.TryParse(heroId, out var heroIdGuid);
            HeroModel? hero = null;
            if (isConvertingToGuidSucceeded)
            {     
                hero = await _context.Heroes.Include(h=>h.Owner).FirstOrDefaultAsync(h=>h.Id==heroIdGuid);
            }
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
        public async Task<bool> CreateHero(string name, string imagePath)
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
            
        }
    }
}
