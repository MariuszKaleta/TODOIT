﻿using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TODOIT.Model.Entity.Order;

namespace TODOIT.Repositories.Contracts
{
    public interface IOrderMembersRepository
    {
        Task<OrderMember> Get(Guid id, params Expression<Func<OrderMember, object>>[] navigationPropertyPaths);

        Task<OrderMember> Get(Guid orderId, string userId, params Expression<Func<OrderMember, object>>[] navigationPropertyPaths);

        Task InviteUserToMakeOrder(Guid orderId, string[] usersId);
        
        Task KickUserFromMakeOrder(Guid orderId, string[] usersId);
    }
}