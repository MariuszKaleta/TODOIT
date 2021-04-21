using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        Task<Opinion> Create(CreateOpinionViewModel model, string userId);

        Task<Opinion> Update(Guid opinionId, OpinionViewModel model);

        void Delete(Guid opinionId, bool saveChanges);

        Task<Opinion> Get(Guid id, params Expression<Func<Opinion, object>>[] navigationPropertyPaths);

        Task<Opinion[]> Get(IReadOnlyCollection<Guid> ids, int? start = null, int? count = null, params Expression<Func<Opinion, object>>[] navigationPropertyPaths);

        Task<Opinion[]> Get(int? start = null, int? count = null, params Expression<Func<Opinion, object>>[] navigationPropertyPaths);

        Task<bool> IAmAuthor(string userId, Guid opinionId);

        Task<ILookup<Guid, Opinion>> GetByOrderIds(IEnumerable<Guid> ownerIds);

        Task<ILookup<string, Opinion>> GetByAuthorIds(IEnumerable<string> authorIds);
    }
}
