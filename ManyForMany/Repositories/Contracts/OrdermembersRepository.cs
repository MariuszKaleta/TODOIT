using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.Skill;

namespace TODOIT.Repositories.Contracts
{
    public interface IOrderMembersRepository
    {
        Task<OrderMember> Get(Guid id, params Expression<Func<OrderMember, object>>[] navigationPropertyPaths);

        Task<OrderMember> Get(Guid orderId, string userId, params Expression<Func<OrderMember, object>>[] navigationPropertyPaths);

        Task InviteUserToMakeOrder(string[] usersId, Guid orderId);
        
        Task KickUserFromMakeOrder(string[] usersId, Guid orderId);
    }
}
