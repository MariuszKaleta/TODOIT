using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity.User;
using ManyForMany.ViewModel.Team;
using Microsoft.Extensions.Logging;
using MvcHelper.Entity;

namespace ManyForMany.Models.Entity.Chat
{
    public abstract class Chat 
    {
        protected Chat()
        {

        }

        [Key]
        public string Id { get; private set; }


        public List<ApplicationUser> Members { get; protected set; }
    }
}
