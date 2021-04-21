using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TODOIT.Model.Entity.User;

namespace TODOIT.Model.Entity.Skill
{
    public class HeadSkill
    {
        private HeadSkill()
        {

        }

        public HeadSkill(string userId, string skillName)
        {
            UserId = userId;
            SkillName = skillName;
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; private set; }

        [Required]
        public Skill Skill { get; set; }

        [ForeignKey(nameof(Skill))]
        public string SkillName { get; private set; }
    }
}
