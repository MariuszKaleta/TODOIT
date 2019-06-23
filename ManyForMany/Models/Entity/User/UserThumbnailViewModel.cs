using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;

namespace ManyForMany.ViewModel.User
{
    public class UserThumbnailViewModel
    {
        public UserThumbnailViewModel(ApplicationUser user)
        {
            Id = user.Id;
            Name = user.Name;
            Picture = user.Picture;
        }

        public string Id { get; private set; }

        public string Name { get; set; }

        public string Picture { get; private set; }
    }
}
