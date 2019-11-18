using System.ComponentModel.DataAnnotations;

namespace TODOIT.ViewModel.User
{
    public class ExternalLoginViewModel : UserViewModel
    {
        public string Id { get; set; }
    }

    public class PasswordViewModel : UserViewModel
    {
        [Required]
        public string Password { get; set; }
    }

    public class UserViewModel
    {
        // [Required]
        [EmailAddress] 
        public string Email { get; set; }

        public string Name { get; set; }

        public string Surrname { get; set; }

        public string[] Skills { get; set; }
    }
}