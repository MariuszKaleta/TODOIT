using System.Collections.Generic;

namespace ManyForMany.Models.Entity.User
{
    public class UserInformationViewModel
    {
        private UserInformationViewModel()
        {

        }

        public UserInformationViewModel(ApplicationUser user)
        {
            Id = user.Id;
            UserName = user.UserName;
            Picture = user.Picture;
            Skills = user.Skills;
        }

        public string Id { get;  private set; }

        public string UserName { get; private set; }

        public string Picture { get; private set; }

        public List<Skill.Skill> Skills { get; private set; }
    }
}
