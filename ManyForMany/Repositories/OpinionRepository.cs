using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcHelper.Entity;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.Rate;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories.Contracts;
using TODOIT.ViewModel.Opinion;

namespace TODOIT.Repositories
{
    public class OpinionRepository : IOpinionRepository
    {
        private readonly Context _context;

        public OpinionRepository(Context context)
        {
            _context = context;
        }

        public async Task<Opinion> Create(OpinionViewModel model, ApplicationUser user, Order order)
        {
            var opinion = new Opinion(user, order, model);

            _context.Opinions.Add(opinion);

            await _context.SaveChangesAsync();

            return opinion;
        }

        public async Task<Opinion> Update(Opinion opinion, OpinionViewModel model)
        {
            opinion.Assign(model);
            await _context.SaveChangesAsync();

            return opinion;
        }

        public void Delete(Opinion obj, bool saveChanges)
        {
            _context.Opinions.Remove(obj);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public async Task<Opinion> Get(Guid id)
        {
            var opinion = await _context.Opinions
                .Include(x => x.Author)
                .Include(x => x.Order)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (opinion == null)
            {
                throw new Exception(Errors.ElementDoseNotExist);
            }

            return opinion;
        }

        public async Task<Opinion[]> Get(IReadOnlyCollection<Guid> ids)
        {
            return await _context.Opinions
                    .Include(x => x.Author)
                    .Include(x => x.Order)
                .Where(x => ids.Contains(x.Id))
                .ToArrayAsync();
        }

        public async Task<Opinion[]> GetByAuthorId(string authorId, int? start = null, int? count = null)
        {
            return await _context.Opinions
                .Include(x => x.Author)
                .Include(x => x.Order)
                .Where(x => x.Author.Id == authorId)
                .TryTake(start,count)
                .ToArrayAsync();
        }

        public async Task<Opinion[]> GetByOrderId(Guid orderId, int? start = null, int? count = null)
        {
            return await _context.Opinions
                .Include(x => x.Author)
                .Include(x => x.Order)
                .Where(x => x.Order.Id == orderId)
                .TryTake(start, count)
                .ToArrayAsync();
        }

        public async Task<ILookup<Guid, Opinion>> GetByOrderIds(IEnumerable<Guid> ownerIds)
        {
            var accounts = await _context.Opinions
                .Include(x => x.Author)
                .Include(x => x.Order)
                .Where(a => ownerIds.Contains(a.Order.Id)).ToListAsync();

            return accounts.ToLookup(x => x.Order.Id);
        }

        public async Task<ILookup<string, Opinion>> GetByAuthorIds(IEnumerable<string> authorIds)
        {
            var accounts = await _context.Opinions
                .Include(x => x.Author)
                .Include(x => x.Order)
                .Where(a => authorIds.Contains(a.Author.Id)).ToListAsync();

            return accounts.ToLookup(x => x.Author.Id);
        }
    }
}
