using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TODOIT.Model.Entity.User;

namespace TODOIT.Model.Entity.Chat
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
