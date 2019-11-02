using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TODOIT.Model.Entity.Skill
{
    public class ReuiredSkill
    {
        [Key]
        public Guid Id { get; set; }

        public Order.Order Order { get; set; }

        public Skill Skill { get; set; }
    }
}
