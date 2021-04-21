using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GraphQlHelper;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.Skill;
using TODOIT.ViewModel.Skill;

namespace TODOIT.Repositories.Contracts
{
    public interface ISkillRepository
    {

        Task<Skill> Create(CreateSkillViewModel model);

        Task<Skill> Update(string skillName, CreateSkillViewModel model);

        void Delete(string skillName, bool saveChanges);

        Task<Skill> Get(string name, params Expression<Func<Skill, object>>[] navigationPropertyPaths);

        Task<Skill[]> Get(IReadOnlyCollection<string> names, params Expression<Func<Skill, object>>[] navigationPropertyPaths);

        Task<Skill[]> Get(string name = null, int? start = null, int? count = null, params Expression<Func<Skill, object>>[] navigationPropertyPaths);

        Task<ILookup<Guid, Skill>> GetByOrderIds(IEnumerable<Guid> ownerIds);

        Task<ILookup<string, Skill>> GetByUserIds(IEnumerable<string> ownerIds);
        
       // Task AddUsedSkills(Order order, IReadOnlyCollection<string> skills, bool saveChanges);
        //Task RemoveUsedSkills(Order order, IReadOnlyCollection<string> skills, bool saveChanges);
    }
}