using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcHelper.Entity;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.Skill;
using TODOIT.Repositories.Contracts;
using TODOIT.ViewModel.Skill;

namespace TODOIT.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly Context _context;

        public SkillRepository(Context context)
        {
            _context = context;
        }

        public async Task<Skill> Get(string name, params Expression<Func<Skill, object>>[] navigationPropertyPaths)
        {
            IQueryable<Skill> opinions = _context.Skills;

            foreach (var path in navigationPropertyPaths)
            {
                opinions = opinions.Include(path);
            }

            return await Get(opinions, name);
        }

        private static async Task<Skill> Get(IQueryable<Skill> orders, string id)
        {
            var order = await orders
                .FirstOrDefaultAsync(x => x.Name == id);

            if (order == null)
            {
                throw new Exception(Errors.SkillIsNotExistInList);
            }

            return order;
        }


        public async Task<Skill[]> Get(IReadOnlyCollection<string> names, params Expression<Func<Skill, object>>[] navigationPropertyPaths)
        {
            IQueryable<Skill> opinions = _context.Skills;

            foreach (var path in navigationPropertyPaths)
            {
                opinions = opinions.Include(path);
            }

            var skills = await opinions
                .Where(x => names.Any(y => y == x.Name)).ToArrayAsync();

            if (skills.Length != names.Count())
            {
                throw new Exception();
            }

            return skills;
        }

        public async Task<Skill[]> Get( string name = null, int? start = null, int? count = null, params Expression<Func<Skill, object>>[] navigationPropertyPaths)
        {
            IQueryable<Skill> skills = _context.Skills;

            foreach (var path in navigationPropertyPaths)
            {
                skills = skills.Include(path);
            }

            if (!string.IsNullOrEmpty(name))
            {
                skills = skills.Where(x => x.Name.Contains(name));
            }

            return await skills.TryTake(start, count).ToArrayAsync();
        }

        public async Task<ILookup<Guid, Skill>> GetByOrderIds(IEnumerable<Guid> ownerIds)
        {
            var usedSkills = await _context.UsedSkills
                .Include(x => x.Order)
                .Include(x => x.Skill)
                .Where(a => ownerIds.Contains(a.Order.Id)).ToListAsync();

            return usedSkills.ToLookup(x => x.Order.Id, x => x.Skill);
        }

        public async Task<ILookup<string, Skill>> GetByUserIds(IEnumerable<string> ownerIds)
        {
            var headSkills = await _context.HeadSkills
                .Include(x => x.Skill)
                .Include(x => x.User)
                .Where(a => ownerIds.Contains(a.User.Id)).ToListAsync();

            return headSkills.ToLookup(x => x.User.Id, x => x.Skill);
        }

        public async Task<Skill> Create(CreateSkillViewModel model)
        {
            if (_context.Skills.Any(x => x.Name == model.Name))
            {
                throw new Exception(Errors.SkillIsAlreadyExist);
            }

            var skill = new Skill(model);

            _context.Skills.Add(skill);

            await _context.SaveChangesAsync();

            return skill;
        }

        public async Task<Skill> Update(string skillName, CreateSkillViewModel model)
        {
            var skill = await Get(skillName);

            if (_context.Skills.Any(x => x.Name == model.Name))
            {
                throw new Exception(Errors.SkillIsAlreadyExist);
            }

            skill.Assign(model);

            await _context.SaveChangesAsync();

            return skill;
        }

        public void Delete(string skillName, bool saveChanges)
        {
            var skill = Get(skillName).Result;

            _context.Skills.Remove(skill);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
        


        

       
    }
}
