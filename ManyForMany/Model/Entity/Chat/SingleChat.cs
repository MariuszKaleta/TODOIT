using System.Collections.Generic;
using TODOIT.Model.Entity.User;

namespace TODOIT.Model.Entity.Chat
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
