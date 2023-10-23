

namespace heroes.Models
{
    public class AccountResponseModel
    {
        public string? FailReason { get; set; }
        public string? Token { get; set; }
        public ICollection<HeroModel>? Heroes { get; set; }
        public string? Username { get; set; }
    }
}
