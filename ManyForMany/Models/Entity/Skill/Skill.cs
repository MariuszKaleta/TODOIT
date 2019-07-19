using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Configuration;
using ManyForMany.ViewModel.Skill;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using MvcHelper.Entity;

namespace ManyForMany.Models.Entity.Skill
{
    public class Skill 
    {
        private Skill()
        {

        }

        public Skill(string name)
        {
            Name = name;
        }

        [Key]
        public int Id { get; private set; }

        [Required]
        public string Name { get; private set; }
    }

    public static class SkillExtension
    {
        public static SkillThumbnailViewModel ToThumbnail(this Skill skill)
        {
            return  new SkillThumbnailViewModel(skill);
        }

        public static PublicSkillViewModel ToViewModel(this Skill skill)
        {
            return  new PublicSkillViewModel(skill);
        }
        
        public static async Task<Skill> Get(this IQueryable<Skill> skills, int id)
        {
            return skills.Get(x => x.Id, id, Errors.SkillIsNotExistInList);
        }

        public static async Task Add(this IList<Skill> skills, int id, IQueryable<Skill> dataSkills)
        {
            var skill = await dataSkills.Get(id);

            if (skills.FirstOrDefault(x => x.Id == id) != null)
            {
                throw new MultiLanguageException(nameof(id), Errors.SkillIsAlreadyExist, id);
            }

            skills.Add(skill);
        }

        public static void Add(this List<Skill> skills, int[] idCollection, IQueryable<Skill> dataSkills)
        {
            var skillsToAdd = dataSkills.Get(x => x.Id, idCollection);

            var commonPart = skills.AsQueryable().Intersect(skillsToAdd);

            if (commonPart.Any())
            {
                throw new MultiLanguageException(nameof(Skill.Id), Errors.SkillIsAlreadyExist, commonPart.First());
            }

            skills.AddRange(skillsToAdd);
        }
    }
}
