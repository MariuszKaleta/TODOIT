using System.ComponentModel.DataAnnotations;

namespace TODOIT.ViewModel.User
{
    public class PasswordViewModel : UserViewModel
    {
        [Required]
        public string Password { get; set; }
    }
}