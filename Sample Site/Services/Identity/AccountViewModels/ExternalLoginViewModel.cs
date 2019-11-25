using System.ComponentModel.DataAnnotations;

namespace SampleSite.Services.Identity.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
