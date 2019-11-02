using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TODOIT.Model.Entity.User;

namespace TODOIT.Model.Entity.Skill
{
    public class HeadSkill
    {
        [Key]
        public Guid Id { get; set; }

        public ApplicationUser User { get; set; }

        public Skill Skill { get; set; }
    }
}
