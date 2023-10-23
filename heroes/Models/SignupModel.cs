using System.ComponentModel.DataAnnotations;

namespace heroes.Models
{
    public class SignupModel
    {
        [Required,MinLength(2),MaxLength(10)]
        public string Username {  get; set; }
        [Required, RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*]).{6,20}$", ErrorMessage = "The password must be between 6 and 20 characters and include at least one number, one lowercase letter, one uppercase letter, and one of the special characters !@#$%^&*.")]
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; } 
    }
}
