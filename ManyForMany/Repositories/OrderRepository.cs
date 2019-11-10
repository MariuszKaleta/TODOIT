using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MultiLanguage.Exception;
using MvcHelper.Entity;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.Rate;
using TODOIT.Model.Entity.Skill;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories.Contracts;
using TODOIT.ViewModel.Order;

namespace TODOIT.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Context _context;
        private readonly ISkillRepository _skillRepository;

        public OrderRepository(Context context, ISkillRepository skillRepository)
        {
            _context = context;
            _skillRepository = skillRepository;
        }

        public async Task<Order> Get(Guid id, params Expression<Func<Order, object>>[] navigationPropertyPaths)
        {
            IQueryable<Order> opinions = _context.Orders;

            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                opinions = opinions.Include(navigationPropertyPath);
            }

            return await Get(opinions, id);
        }

        private static async Task<Order> Get(IQueryable<Order> orders, Guid id)
        {
            var order = await orders
                .FirstOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                throw new Exception(Errors.OrderIsNotExistInList);
            }

            return order;
        }

        public async Task<Order[]> Get(IEnumerable<Guid> ids, params Expression<Func<Order, object>>[] navigationPropertyPaths)
        {
            IQueryable<Order> orders = _context.Orders;

            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                orders = orders.Include(navigationPropertyPath);
            }

            return await orders
                .Where(x => ids.Contains(x.Id))
                .ToArrayAsync();
        }

        public async Task<Order[]> Get( string name = null, int? start = null, int? count = null,params Expression<Func<Order, object>>[] navigationPropertyPaths)
        {
            IQueryable<Order> orders = _context.Orders;

            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                orders = orders.Include(navigationPropertyPath);
            }

            if (!string.IsNullOrEmpty(name))
            {
                orders = orders.Where(x => x.Name.Contains(name));
            }

            return orders.TryTake(start, count).ToArray();
        }

        public async Task<ILookup<string, Order>> GetByOwnerIds(IEnumerable<string> ownerIds)
        {
            var accounts = await _context.Orders
                .Include(x => x.Owner)
                .Where(a => ownerIds.Contains(a.Owner.Id))
                .ToListAsync();

            return accounts.ToLookup(x => x.Owner.Id);
        }

        public async Task<ILookup<string, Order>> GetByInterestedOrderIds(IEnumerable<string> ownerIds)
        {
            var orders = await _context.InterestedOrders
                .Include(x => x.Order)
                .Where(a => ownerIds.Contains(a.UserId))
                .ToListAsync();

            return orders.ToLookup(x => x.UserId, x => x.Order);
        }

        public async Task AddToInterested(string userId, Guid orderID)
        {
            if (_context.InterestedOrders.Any(x => x.Order.Id == orderID && x.User.Id == userId))
            {
                throw new Exception(Errors.YouInterestedThisOrder);
            }

            var order = await Get(orderID);
            Validate(order, userId);

            var interested = new InterestedOrder(userId,orderID)
            {
            };

            _context.InterestedOrders.Add(interested);

            await _context.SaveChangesAsync();
        }

        public async Task AddToInterested(string user, IReadOnlyCollection<Guid> orderIds)
        {
            var decisionExists = _context.InterestedOrders.Where(x => x.User.Id == user && orderIds.Contains(x.Order.Id));

            if (decisionExists.Any())
            {
                throw new Exception(Errors.YouInterestedThisOrder +
                                    string.Join('\n', decisionExists.Select(x => x.Order.Id)));
            }

            foreach (var order in await Get(orderIds))
            {
                Validate(order, user);
            }

            _context.InterestedOrders.AddRange(orderIds.Select(x=> new InterestedOrder(user,x)
            {

            }));

            await _context.SaveChangesAsync();
        }

        
        public static void Validate(Order order, string user)
        {
            if (order.Status != OrderStatus.CompleteTeam)
            {
                throw new Exception(Errors.OwnerOfOrderDontLookingForTeam);
            }

            if (order.OwnerId == user)
            {
                throw new Exception(Errors.YouCantJoinToYourOrderTeam);
            }
        }

        public async Task<Order> Create(CreateOrderViewModel model, string user)
        {
            var order = new Order(model, user);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            if (model.Skills?.Any() == true)
            {
                await UpdateRequiredSkills(order, model.Skills, false);
                await _context.SaveChangesAsync();
            }

            return order;
        }

        public async Task<Order> Update(CreateOrderViewModel viewModel, Guid orderID)
        {
            var order = await Get(orderID);

            await UpdateRequiredSkills(order, viewModel.Skills, false);
            order.Assign(viewModel);

            await _context.SaveChangesAsync();

            return order;
        }

        public async void Delete(Guid orderId, bool saveChanges)
        {
            var order = await Get(orderId);

            _context.Orders.Remove(order);

            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
        }

       

        public async Task UpdateRequiredSkills(Order order, IReadOnlyCollection<string> updatedSkillNames, bool saveChanges)
        {
            var actualUsedSkillsAsync = _context.UsedSkills
                .Where(x => x.OrderId == order.Id && updatedSkillNames.Contains(x.Skill.Name))
                .ToArrayAsync();

            //  var updatedSkillsAsync = Get(updatedSkillNames);

            var actualUsedSkills = await actualUsedSkillsAsync;
            //  var updatedSkills = await updatedSkillsAsync;

            var addTask = AddUsedSkills(_context.UsedSkills, order.Id, actualUsedSkills, updatedSkillNames);
            var removeTask = RemoveUsedSkills(_context.UsedSkills, actualUsedSkills, updatedSkillNames);

            await addTask;
            await removeTask;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IAmOwner(Guid orderId, string userId)
        {
            return await _context.Orders.AnyAsync(x => x.Id == orderId && x.OwnerId == userId);
        }

        private static async Task AddUsedSkills(DbSet<RequiredSkill> usedSkills, Guid orderId, IReadOnlyCollection<RequiredSkill> actualSkills, IEnumerable<string> updatedSkills)
        {
            var addedSkills = updatedSkills.Where(x => actualSkills.All(y => y.Skill.Name != x));

            await AddUsedSkills(usedSkills, orderId, addedSkills);
        }

        private static async Task AddUsedSkills(DbSet<RequiredSkill> context, Guid orderGuid, IEnumerable<string> skills)
        {
            await context.AddRangeAsync(skills.Select(x => new RequiredSkill(orderGuid, x)));
        }

        public static async Task RemoveUsedSkills(DbSet<RequiredSkill> context, IReadOnlyCollection<RequiredSkill> actualSkills, IReadOnlyCollection<string> updatedSkills)
        {
            var removedSkills = actualSkills.Where(x => updatedSkills.All(y => y != x.Skill.Name));

            await RemoveUsedSkills(context, removedSkills);
        }

        public static async Task RemoveUsedSkills(DbSet<RequiredSkill> context, IEnumerable<RequiredSkill> skills)
        {
            context.RemoveRange(skills);
        }
    }
}
