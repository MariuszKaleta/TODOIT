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
    public interface IOrderRepository : IRepository<Order,Guid>
    {
        Task<ILookup<string, Order>> GetByOwnerIds(IEnumerable<string> ownerIds);

        Task AddToInterested(ApplicationUser user, Order order);

        Task AddToInterested(ApplicationUser user, IReadOnlyCollection<Order> orders);

        Task<Order> Create(CreateOrderViewModel model, ApplicationUser user);

        Task<Order> Update(CreateOrderViewModel model, Order order);

        void Delete(Order device, bool saveChanges, ApplicationUser user);
    }
}
