using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManyForMany.ViewModel.Skill
{
    public class SkillThumbnailViewModel : SkillViewModel
    {
        public SkillThumbnailViewModel(Models.Entity.Skill.Skill skill)
        {
            Id = skill.Id;
            Name = skill.Name;
        }

        public int Id { get; set; }
    }
}
