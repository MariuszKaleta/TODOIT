using System;
using System.ComponentModel.DataAnnotations;
using GraphQL;
using GraphQlHelper;
using TODOIT.ViewModel.Skill;

namespace TODOIT.Model.Entity.Skill
{
    public class Skill 
    {
        private Skill()
        {

        }

        public Skill(CreateSkillViewModel model)
        {
            this.Assign(model);
        }

        [Key]
        [Required]
        public string Name { get; set; }
    }
}
