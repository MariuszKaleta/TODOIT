using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.Rate;
using TODOIT.Model.Entity.Skill;
using TODOIT.Model.Entity.User;
using TODOIT.ViewModel.Opinion;
using TODOIT.ViewModel.Skill;

namespace TODOIT.Repositories.Contracts
{
    public interface IOpinionRepository
    {
        Task<Opinion> Create(OpinionViewModel model, ApplicationUser user, Order order);

        Task<Opinion> Update(Opinion opinion, OpinionViewModel model);

        void Delete(Opinion obj, bool saveChanges);

        Task<Opinion> Get(Guid id);

        Task<Opinion[]> Get(IReadOnlyCollection<Guid> ids);

        Task<Opinion[]> GetByAuthorId(string authorId, int? start = null, int? count = null);

        Task<Opinion[]> GetByOrderId(Guid orderId, int? start = null, int? count = null);

        Task<ILookup<Guid, Opinion>> GetByOrderIds(IEnumerable<Guid> ownerIds);

        Task<ILookup<string, Opinion>> GetByAuthorIds(IEnumerable<string> authorIds);
    }
}
