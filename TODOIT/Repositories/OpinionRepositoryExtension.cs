using System;
using System.Threading.Tasks;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity.Rate;
using TODOIT.ViewModel.Opinion;

namespace TODOIT.Repositories.Contracts
{
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