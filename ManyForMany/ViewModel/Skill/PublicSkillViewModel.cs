using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManyForMany.ViewModel.Skill
{
    public class PublicSkillViewModel :  SkillThumbnailViewModel
    {
        public PublicSkillViewModel(Models.Entity.Skill.Skill skill) : base(skill)
        {
        }
    }
}
