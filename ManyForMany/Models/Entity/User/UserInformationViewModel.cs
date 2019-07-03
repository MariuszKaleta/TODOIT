using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using ManyForMany.Models.Entity.Order;
using ManyForMany.Models.Entity.Rate;

namespace ManyForMany.ViewModel.User
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
            Opinions = user.OpinionsAboutMe;
        }

        public string Id { get;  private set; }

        public string UserName { get; private set; }

        public string Picture { get; private set; }

        public List<Skill> Skills { get; private set; }

        public List<Opinion> Opinions { get; private set; }
    }
}
