﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TODOIT.Model.Configuration;
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

    public static class OpinionRepositoryExtension
    {
        public static async Task<Opinion> Update(this  IOpinionRepository repository, Guid opinionId, OpinionViewModel model, string userId)
        {
            if (!await repository.IAmAuthor(userId, opinionId))
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }
            
            return await repository.Update(opinionId, model);
        }
        public static async Task Delete(this  IOpinionRepository repository, Guid opinionId, string userId)
        {
            if (!await repository.IAmAuthor(userId, opinionId))
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }

            repository.Delete(opinionId, true);
        }

    }
}
