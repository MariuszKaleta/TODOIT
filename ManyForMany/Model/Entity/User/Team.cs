using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Model.Entity.Ofert;

namespace ManyForMany.Model.Entity.User
{
    public class Team
    {
        private Team()
        {

        }

        public Team(Order order, ApplicationUser orderOwner)
        {

        }

        [Required]
        public ApplicationUser OrderOwner { get; private set; }

        [Required]
        public Order Order { get; private set; }


        public List<ApplicationUser> Members { get; private set; }
    }
}
