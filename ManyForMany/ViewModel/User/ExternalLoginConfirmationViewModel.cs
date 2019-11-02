using System.ComponentModel.DataAnnotations;

namespace TODOIT.ViewModel.User
{
    public class CreateUserViewModel : UserViewModel
    {
        public string Id { get; set; }
    }

    public class UserViewModel
    {
        // [Required]
        [EmailAddress] 
        public string Email { get; set; }

        public string Name { get; set; }

        public string Surrname { get; set; }
    }
}