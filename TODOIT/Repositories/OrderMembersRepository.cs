using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.Rate;
using TODOIT.Model.Entity.Skill;
using TODOIT.Repositories.Contracts;

namespace TODOIT.Repositories
{
    public class OrderMembersRepository : IOrderMembersRepository
    {
        private readonly Context _context;

        public OrderMembersRepository(Context context)
        {
            _context = context;
        }

        public async Task<OrderMember> Get(Guid id, params Expression<Func<OrderMember, object>>[] navigationPropertyPaths)
        {
            IQueryable<OrderMember> orderMembers = _context.OrderMembers;

            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                orderMembers.Include(navigationPropertyPath);
            }

            var orderMember =
                await orderMembers
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (orderMember == null)
            {
                throw new Exception(Errors.ElementDoseNotExist);
            }

            return orderMember;
        }

        public async Task<OrderMember> Get(Guid orderId, string userId, params Expression<Func<OrderMember, object>>[] navigationPropertyPaths)
        {
            IQueryable<OrderMember> orderMembers = _context.OrderMembers;

            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                orderMembers.Include(navigationPropertyPath);
            }

            var orderMember =
                await orderMembers
                    .FirstOrDefaultAsync(x => x.OrderId == orderId && x.UserId == userId);

            if (orderMember == null)
            {
                throw new Exception(Errors.ElementDoseNotExist);
            }

            return orderMember;
        }

        public async Task InviteUserToMakeOrder(Guid orderId, string[] usersId)
        {
            if (_context.OrderMembers.Any(x => usersId.Any(y => y == x.UserId) && x.OrderId == orderId))
            {
                throw new Exception(Errors.MatchAlredyExist);
            }
            
            foreach (var userId in usersId)
            {
                var orderMember = new OrderMember(userId, orderId);
                
                _context.OrderMembers.Add(orderMember);

                var allowToMakeOrder = new AllowToMakeOpinion(orderId, userId);

                _context.AllowToMakeOpines.Add(allowToMakeOrder);
            }

            await _context.SaveChangesAsync();
        }

        public async Task KickUserFromMakeOrder(Guid orderId, string[] usersId)
        {
            var orderMember = await Get(orderId, usersId[0]);

            _context.OrderMembers.Remove(orderMember);

            await _context.SaveChangesAsync();
        }

    }
}
