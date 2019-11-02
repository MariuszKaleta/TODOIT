using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlHelper;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.Skill;
using TODOIT.Model.Entity.User;
using TODOIT.ViewModel.User;

namespace TODOIT.Repositories.Contracts
{
    public interface IUserRepository : IRepository<ApplicationUser, string>
    {
        Task<ApplicationUser> Update(ApplicationUser obj, UserViewModel model);

        Task<Skill[]> UpdateSkills(ApplicationUser obj, Skill[] model);

        Task<ILookup<Guid, ApplicationUser>> GetInterestedByOrderIds(IEnumerable<Guid> orderIds);

        void Delete(ApplicationUser obj, bool saveChanges);
    }
}
