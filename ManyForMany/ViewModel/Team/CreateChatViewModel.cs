using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManyForMany.ViewModel.Team
{
    public class CreateChatViewModel
    {
        public string Name { get; set; }

        public string[] MemeberUsersId { get; set; }
    }
}
