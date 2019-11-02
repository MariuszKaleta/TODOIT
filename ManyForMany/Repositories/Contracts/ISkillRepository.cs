using System;
using System.Collections.Generic;
using System.Linq;
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

        Task<Skill> Update(Skill skill, CreateSkillViewModel model);

        void Delete(Skill obj, bool saveChanges);

        Task<Skill> Get(string name);

        Task<Skill[]> Get(IReadOnlyCollection<string> names);

        Task<Skill[]> Get(string name = null, int? start = null, int? count = null);

        Task<ILookup<Guid, Skill>> GetByOrderIds(IEnumerable<Guid> ownerIds);

        Task<ILookup<string, Skill>> GetByUserIds(IEnumerable<string> ownerIds);

        Task UpdateUsedSkills(Order order, IReadOnlyCollection<string> updatedSkillNames, bool saveChanges);
       // Task AddUsedSkills(Order order, IReadOnlyCollection<string> skills, bool saveChanges);
        //Task RemoveUsedSkills(Order order, IReadOnlyCollection<string> skills, bool saveChanges);
    }
}