using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MultiLanguage.Exception;
using MvcHelper.Entity;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.Order;
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

        public async Task<Order> Get(Guid id)
        {
            var order = await _context.Orders
                .Include(x => x.Owner)
                .FirstOrDefaultAsync(x => x.Id == id);

            if(order == null)
            {
                throw new Exception(Errors.OrderIsNotExistInList);
            }

            return order;
        }

        public async Task<Order[]> Get(IEnumerable<Guid> ids)
        {
            return await _context.Orders
                .Include(x => x.Owner)
                .Where(x => ids.Contains(x.Id))
                .ToArrayAsync();
        }

        public async Task<Order[]> Get(string name = null, int? start = null, int? count = null)
        {
            IQueryable<Order> orders = _context.Orders
                .Include(x=>x.Owner)
                ;

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

        public async Task AddToInterested(ApplicationUser user, Order order)
        {
            Validate(order, user);

            if (_context.InterestedOrders.Any(x => x.Order == order && x.User == user))
            {
                throw new Exception(Errors.YouInterestedThisOrder);
            }

            var interested = new InterestedOrder()
            {
                Order = order,
                User = user
            };

            _context.InterestedOrders.Add(interested);

            await _context.SaveChangesAsync();
        }

        public async Task AddToInterested(ApplicationUser user, IReadOnlyCollection<Order> orders)
        {
            foreach (var order in orders)
            {
                Validate(order, user);
            }

            var decisionExists = _context.InterestedOrders.Where(x => x.User == user && orders.Contains(x.Order));

            if (decisionExists.Any())
            {
                throw new Exception(Errors.YouInterestedThisOrder +
                                    string.Join('\n', decisionExists.Select(x => x.Order.Id)));
            }

            _context.InterestedOrders.AddRange(orders.Select(x=> new InterestedOrder()
            {
                User = user,
                Order = x
            }));

            await _context.SaveChangesAsync();
        }

        public static void Validate(Order order, ApplicationUser user)
        {
            if (order.Status != OrderStatus.CompleteTeam)
            {
                throw new Exception(Errors.OwnerOfOrderDontLookingForTeam);
            }

            if (order.Owner == user)
            {
                throw new Exception(Errors.YouCantJoinToYourOrderTeam);
            }
        }

        public async Task<Order> Create(CreateOrderViewModel model, ApplicationUser user)
        {
            var order = new Order(model, user);

            _context.Orders.Add(order);

            await _skillRepository.UpdateUsedSkills(order, model.Skills, false);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order> Update(CreateOrderViewModel viewModel, Order dbDevice)
        {
            await _skillRepository.UpdateUsedSkills(dbDevice, viewModel.Skills, false);
            dbDevice.Assign(viewModel);

            await _context.SaveChangesAsync();

            return dbDevice;
        }

        public async void Delete(Order device, bool saveChanges, ApplicationUser user)
        {
            if (device.Owner != user)
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }

            _context.Orders.Remove(device);

            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}
