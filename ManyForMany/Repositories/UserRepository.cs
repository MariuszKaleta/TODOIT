using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcHelper.Entity;
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

        public async Task<ApplicationUser> Get(string id)
        {
            //throw new NotImplementedException();
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ApplicationUser[]> Get(IEnumerable<string> ids)
        {
            //  throw new NotImplementedException();
            return await _context.Users.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        }

        public async Task<ApplicationUser[]> Get(string name = null, int? start = null, int? count = null)
        {
            // throw new NotImplementedException();
            IQueryable<ApplicationUser> users = _context.Users;

            if (!string.IsNullOrEmpty(name))
            {
                users = users.Where(x => x.Name.Contains(name));
            }


            return users.TryTake(start, count).ToArray();
        }

        public async Task<ApplicationUser> Update(ApplicationUser obj, UserViewModel model)
        {
            obj.Assign(model);

            await _context.SaveChangesAsync();

            return obj;
        }

        public async Task<Skill[]> UpdateSkills(ApplicationUser obj, Skill[] model)
        {
            _context.HeadSkills.AddRange(model.Select(x => new HeadSkill()
            {
                Skill = x,
                User = obj
            }));

            await _context.SaveChangesAsync();

            return model;
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
