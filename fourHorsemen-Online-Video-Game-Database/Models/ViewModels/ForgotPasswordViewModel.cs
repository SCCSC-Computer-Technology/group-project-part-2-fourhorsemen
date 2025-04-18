using System.ComponentModel.DataAnnotations;

namespace fourHorsemen_Online_Video_Game_Database.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
