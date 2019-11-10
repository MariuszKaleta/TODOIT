using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TODOIT.Model.Entity.Skill
{
    public class RequiredSkill
    {
        private RequiredSkill()
        {

        }

        public RequiredSkill(Guid orderId, string skillName)
        {
            OrderId = orderId;
            SkillName = skillName;
        }
        
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Order.Order Order { get; set; }

        [ForeignKey(nameof(Order))]
        public Guid OrderId { get; private set; }

        [Required]
        public Skill Skill { get; set; }

        [ForeignKey(nameof(Skill))]
        public string SkillName { get; private set; }
    }
}
