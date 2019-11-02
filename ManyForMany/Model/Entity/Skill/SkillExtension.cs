using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiLanguage.Exception;
using MvcHelper.Entity;
using TODOIT.Model.Configuration;
using TODOIT.ViewModel.Skill;

namespace TODOIT.Model.Entity.Skill
{
    public static class SkillExtension
    {
        public static void Assign(this Skill skill, CreateSkillViewModel model)
        {
            skill.Name = model.Name;
        }
    }
}