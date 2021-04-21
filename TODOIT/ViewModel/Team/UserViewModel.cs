using TODOIT.Model.Entity.User;

namespace TODOIT.ViewModel.Team
{
    public class UserViewModel
    {
        public UserViewModel(ApplicationUser user)
        {
            UserName = user.UserName;
            Name = user.Name;
            SurName = user.Surrname;
            
        }

        public UserViewModel()
        {

        }

        public string UserName { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }


    }
}
