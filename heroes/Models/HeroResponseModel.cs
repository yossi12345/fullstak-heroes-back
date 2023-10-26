namespace heroes.Models
{
    public class HeroResponseModel
    {
        public string? ErrorMessage { get; set; }
        public List<HeroModel>? Heroes { get; set; }
        public int Status { get; set; }

    }
}
