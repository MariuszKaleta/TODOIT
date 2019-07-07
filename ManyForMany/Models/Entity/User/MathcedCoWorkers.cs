using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManyForMany.Models.Entity.User
{
    public class MathcedCoWorkers
    {
        private MathcedCoWorkers()
        {

        }

        public MathcedCoWorkers(ApplicationUser user1, ApplicationUser user2)
        {
            Persons = new List<ApplicationUser>
            {
                user1,
                user2
            };


            MatchTime = DateTime.Now;
        }

        [Key] public string Id { get; set; }

        [Required] public List<ApplicationUser> Persons { get; set; }


        [Required] public DateTime MatchTime { get; set; }
    }
}
