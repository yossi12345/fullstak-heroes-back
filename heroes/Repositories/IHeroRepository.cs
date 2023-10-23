using heroes.Models;

namespace heroes.Repositories
{
    public interface IHeroRepository
    {
        Task<HeroResponseModel> TryTrain(string userId, string heroId);
        Task<HeroResponseModel> AddHeroToUser(string userId, string heroId);
        Task<HeroResponseModel> RemoveHeroFromUser(string userId, string heroId);
        Task<HeroResponseModel> GetHero(string userId, string heroId);
        Task<HeroResponseModel> GetAllHeroes(string userId);
        Task<bool> CreateHero(string name, string imagePath);
    }
}
