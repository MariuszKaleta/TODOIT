using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlHelper;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.User;
using TODOIT.ViewModel.Order;

namespace TODOIT.Repositories.Contracts
{
    public interface IOrderRepository : IRepository<Order, Guid>
    {
        Task<ILookup<string, Order>> GetByOwnerIds(IEnumerable<string> ownerIds);

        Task<ILookup<string, Order>> GetByInterestedOrderIds(IEnumerable<string> ownerIds);

        Task<ILookup<string, Order>> GetAllowedOrderToMakeOpinion(IEnumerable<string> userIds);

        Task<Order> Create(CreateOrderViewModel model, string userId);

        Task<Order> Update(CreateOrderViewModel model, Guid orderId);

        void Delete(Guid orderId, bool saveChanges);

        Task AddToInterested(string user, Guid orderId);

        Task RemoveFromInterested(string userId, Guid orderId);

        Task UpdateRequiredSkills(Order order, IReadOnlyCollection<string> updatedSkillNames, bool saveChanges);

        Task<bool> IAmOwner(Guid orderId, string userId);
    }
}
