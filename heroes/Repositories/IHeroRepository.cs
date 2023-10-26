using heroes.Models;

namespace heroes.Repositories
{
    public interface IHeroRepository
    {
        Task<HeroResponseModel> TryTrain(string userId, Guid heroId);
        Task<HeroResponseModel> AddHeroToUser(string userId, Guid heroId);
        Task<HeroResponseModel> RemoveHeroFromUser(string userId, Guid heroId);
        Task<HeroResponseModel> GetHero(string userId, Guid heroId);
        Task<HeroResponseModel> GetAllHeroes(string userId);
        //Task<bool> CreateHero(string name, string imagePath);
    }
}
