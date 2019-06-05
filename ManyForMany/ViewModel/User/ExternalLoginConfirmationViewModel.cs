using System.ComponentModel.DataAnnotations;

namespace AuthorizeTester.ViewModel.User
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required] [EmailAddress] public string Email { get; set; }
    }
}