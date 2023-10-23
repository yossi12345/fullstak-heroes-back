using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace heroes.Models
{
    public class HeroModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Range(0,5)]       
        public int AmountOfTrainingsToday { get; set; }
        public UserModel? Owner { get; set; }
        [Required]
        public string ImagePath {  get; set; }
        [Range(0, double.MaxValue)]
        public double Level {  get; set; }
        public DateTime LastTrainingDate { get; set; }  
    }
}
