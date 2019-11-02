using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Skill> Update(Skill skill, CreateSkillViewModel model)
        {
            if (_context.Skills.Any(x => x.Name == model.Name))
            {
                throw new Exception(Errors.SkillIsAlreadyExist);
            }

            skill.Assign(model);

            await _context.SaveChangesAsync();

            return skill;
        }

        public void Delete(Skill obj, bool saveChanges)
        {
            _context.Skills.Remove(obj);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public async Task<Skill> Get(string name)
        {
            var skill = await _context.Skills.FirstOrDefaultAsync(x => x.Name == name);

            if (skill == null)
            {
                throw new Exception(Errors.SkillIsNotExistInList);
            }

            return skill;
        }

        public async Task<Skill[]> Get(IReadOnlyCollection<string> names)
        {
            var skills = await _context.Skills.Where(x => names.Any(y => y == x.Name)).ToArrayAsync();

            if (skills.Length != names.Count())
            {
                throw new Exception();
            }

            return skills;
        }
        
        public async Task<Skill[]> Get(string name = null, int? start = null, int? count = null)
        {
            IQueryable<Skill> skills = _context.Skills;

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

        public async Task UpdateUsedSkills(Order order, IReadOnlyCollection<string> updatedSkillNames, bool saveChanges)
        {
            var actualUsedSkillsAsync = _context.UsedSkills
                .Where(x => x.Order == order && updatedSkillNames.Contains(x.Skill.Name))
                .ToArrayAsync();

            var updatedSkillsAsync = Get(updatedSkillNames);

            var actualUsedSkills = await actualUsedSkillsAsync;
            var updatedSkills = await updatedSkillsAsync;

            var addTask = AddUsedSkills(_context.UsedSkills, order, actualUsedSkills, updatedSkills);
            var removeTask = RemoveUsedSkills(_context.UsedSkills, actualUsedSkills, updatedSkills);

            await addTask;
            await removeTask;

            await _context.SaveChangesAsync();
        }

        private static async Task AddUsedSkills(DbSet<ReuiredSkill> usedSkills, Order order, IReadOnlyCollection<ReuiredSkill> actualSkills, IEnumerable<Skill> updatedSkills)
        {
            var addedSkills = updatedSkills.Where(x => actualSkills.All(y => y.Skill != x));
            
            await AddUsedSkills(usedSkills, order, addedSkills);
        }

        private static async Task AddUsedSkills(DbSet<ReuiredSkill> context, Order order, IEnumerable<Skill> skills)
        {
            await context.AddRangeAsync(skills.Select(x => new ReuiredSkill()
            {
                Order = order,
                Skill = x
            }));
        }

        public static async Task RemoveUsedSkills(DbSet<ReuiredSkill> context, IReadOnlyCollection<ReuiredSkill> actualSkills, IReadOnlyCollection<Skill> updatedSkills)
        {
            var removedSkills = actualSkills.Where(x => updatedSkills.All(y => y != x.Skill));

            await RemoveUsedSkills(context, removedSkills);
        }

        public static async Task RemoveUsedSkills(DbSet<ReuiredSkill> context, IEnumerable<ReuiredSkill> skills)
        {
            context.RemoveRange(skills);
        }
    }
}
