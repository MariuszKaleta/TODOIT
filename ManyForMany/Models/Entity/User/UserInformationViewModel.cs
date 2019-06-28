using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using ManyForMany.Models.Entity.Order;

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
            Name = user.Name;
            Picture = user.Picture;
            Skills = user.Skills;
        }

        public string Id { get;  private set; }

        public string Name { get; private set; }

        public string Picture { get; private set; }

        public List<Skill> Skills { get; private set; }
    }
}
