using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity.User;
using ManyForMany.ViewModel.Team;
using Microsoft.Extensions.Logging;
using MvcHelper.Entity;

namespace ManyForMany.Models.Entity.Chat
{
    public class SingleChat : Chat
    {

        private SingleChat()
        {

        }

        public SingleChat(ApplicationUser member1, ApplicationUser member2): base()
        {
            Members = new List<ApplicationUser> {member1, member2};
        }
    }
}
