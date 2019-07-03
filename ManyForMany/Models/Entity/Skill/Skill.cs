using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using ManyForMany.Models.Configuration;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using MvcHelper.Entity;

namespace ManyForMany.Models.Entity.Order
{
    public class Skill : IId<int>
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
        public static async Task<Skill> Get(this IQueryable<Skill> users, int id, ILogger logger)
        {
            return await users.Get(id, Errors.SkillIsNotExistInList, logger);
        }

        public static async Task Add(this IList<Skill> skills, int id, IQueryable<Skill> dataSkills, ILogger logger)
        {
            var skill = await dataSkills.Get(id, logger);

            if (skills.FirstOrDefault(x => x.Id == id) != null)
            {
                throw new MultiLanguageException(nameof(id), Errors.SkillIsAlreadyExist, id);
            }

            skills.Add(skill);
        }

        public static void Add(this List<Skill> skills, int[] idCollection, IQueryable<Skill> dataSkills)
        {
            var skillsToAdd = dataSkills.Get(idCollection);

            var commonPart = skills.AsQueryable().Intersect(skillsToAdd);

            if (commonPart.Any())
            {
                throw new MultiLanguageException(nameof(Skill.Id), Errors.SkillIsAlreadyExist, commonPart.First());
            }

            skills.AddRange(skillsToAdd);
        }
    }
}
