using System.ComponentModel.DataAnnotations;

namespace SampleSite.Services.Identity.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
