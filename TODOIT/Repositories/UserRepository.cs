using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcHelper.Entity;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.Skill;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories.Contracts;
using TODOIT.ViewModel.User;

namespace TODOIT.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Context _context;

        public UserRepository(Context context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> Get(string id, params Expression<Func<ApplicationUser, object>>[] navigationPropertyPaths)
        {
            IQueryable<ApplicationUser> opinions = _context.Users;

            foreach (var path in navigationPropertyPaths)
            {
                opinions = opinions.Include(path);
            }

            return await Get(opinions, id);
        }

        private static async Task<ApplicationUser> Get(IQueryable<ApplicationUser> orders, string id)
        {
            var order = await orders
                .FirstOrDefaultAsync(x => x.Name == id);

            if (order == null)
            {
                throw new Exception(Errors.UserIsNotExist);
            }

            return order;
        }


        public async Task<ApplicationUser[]> Get(IEnumerable<string> ids, params Expression<Func<ApplicationUser, object>>[] navigationPropertyPaths)
        {
            IQueryable<ApplicationUser> opinions = _context.Users;

            foreach (var path in navigationPropertyPaths)
            {
                opinions = opinions.Include(path);
            }

            var skills = await opinions
                .Where(x => ids.Contains(x.Id)).ToArrayAsync();

            if (skills.Length != ids.Count())
            {
                throw new Exception();
            }

            return skills;
        }

        public async Task<ApplicationUser[]> Get( string name = null, int? start = null, int? count = null, params Expression<Func<ApplicationUser, object>>[] navigationPropertyPaths)
        {
            // throw new NotImplementedException();
            IQueryable<ApplicationUser> users = _context.Users;

            foreach (var path in navigationPropertyPaths)
            {
                users = users.Include(path);
            }

            if (!string.IsNullOrEmpty(name))
            {
                users = users.Where(x => x.Name.Contains(name));
            }
            return await users.TryTake(start, count).ToArrayAsync();
        }

        public async Task<ApplicationUser> Update(string userId, UserViewModel model)
        {
            var updateSkillTask = UpdateSkills(userId, model.Skills); 

            var user = await Get(userId);

            user.Assign(model);


            await updateSkillTask;
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<string[]> UpdateSkills(string userId, string[] model)
        {
            _context.HeadSkills.AddRange(model.Select(x => new HeadSkill(userId,x)
            {
            }));

            await _context.SaveChangesAsync();

            return model;
        }

        public async Task<ApplicationUser[]> GetInvitetedUserToMakeOrder(Guid orderId)
        {
            return await _context.OrderMembers.Include(x => x.User).Where(x => x.OrderId == orderId).Select(x => x.User)
                .ToArrayAsync();
        }

        public async Task<ILookup<Guid, ApplicationUser>> GetInterestedByOrderIds(IEnumerable<Guid> orderIds)
        {
            var interestedOrders = await _context.InterestedOrders
                .Include(x => x.Order)
                .Include(x => x.User)
                .Where(x => orderIds.Contains(x.Order.Id))
                .ToListAsync()
                ;

            return interestedOrders.ToLookup(x => x.Order.Id, x => x.User);
        }

        public async Task<ILookup<Guid, ApplicationUser>> GetOrderMembersByOrderIds(IEnumerable<Guid> orderIds)
        {
            var interestedOrders = await _context.OrderMembers
                    .Include(x => x.Order)
                    .Include(x => x.User)
                    .Where(x => orderIds.Contains(x.Order.Id))
                    .ToListAsync()
                ;

            return interestedOrders.ToLookup(x => x.Order.Id, x => x.User);
        }

        public void Delete(ApplicationUser obj, bool saveChanges)
        {
            _context.Users.Remove(obj);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        
    }
}
