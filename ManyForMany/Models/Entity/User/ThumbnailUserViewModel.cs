using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;

namespace ManyForMany.ViewModel.User
{
    public class ThumbnailUserViewModel
    {
        public ThumbnailUserViewModel(ApplicationUser user)
        {
            Id = user.Id;
            FirstName = user.FirstName();
            Picture = user.Picture;
        }

        public string Id { get; private set; }

        public string FirstName { get; set; }

        public string Picture { get; private set; }
    }
}
