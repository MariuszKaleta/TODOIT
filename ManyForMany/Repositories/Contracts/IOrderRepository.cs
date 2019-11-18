using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlHelper;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.User;
using TODOIT.ViewModel.Order;

namespace TODOIT.Repositories.Contracts
{
    public interface IOrderRepository : IRepository<Order, Guid>
    {
        Task<ILookup<string, Order>> GetByOwnerIds(IEnumerable<string> ownerIds);

        Task<ILookup<string, Order>> GetByInterestedOrderIds(IEnumerable<string> ownerIds);

        Task<Order> Create(CreateOrderViewModel model, string userId);

        Task<Order> Update(CreateOrderViewModel model, Guid orderId);

        void Delete(Guid orderId, bool saveChanges);

        Task AddToInterested(string user, Guid orderId);

        Task RemoveFromInterested(string userId, Guid orderId);

        Task UpdateRequiredSkills(Order order, IReadOnlyCollection<string> updatedSkillNames, bool saveChanges);

        Task<bool> IAmOwner(Guid orderId, string userId);
    }

    public static class OrderRepositoryExtension
    {
        public static async Task<Order> Update(this IOrderRepository orderRepository, CreateOrderViewModel model, Guid orderId, string userId)
        {
            var iAmOwnerAsync = orderRepository.IAmOwner(orderId, userId);

            if (!await iAmOwnerAsync)
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }

            return await orderRepository.Update(model, orderId);
        }

        public static async void Delete(this IOrderRepository orderRepository, Guid orderId, string userId)
        {
            var iAmOwnerAsync = orderRepository.IAmOwner(orderId, userId);

            if (!await iAmOwnerAsync)
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }

            orderRepository.Delete(orderId, true);
        }
    }
}
