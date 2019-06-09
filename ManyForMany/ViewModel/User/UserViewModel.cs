using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Model.Entity.User;

namespace ManyForMany.ViewModel.User
{
    public class UserViewModel
    {
        private UserViewModel()
        {

        }

        public UserViewModel(ApplicationUser user)
        {
            UserName = user.UserName;
            Email = user.Email;
            PhoneNumber = user.PhoneNumber;
        }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
