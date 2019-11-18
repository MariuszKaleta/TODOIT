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

        public async Task<Opinion> Get(Guid id, params Expression<Func<Opinion, object>>[] navigationPropertyPaths)
        {
            IQueryable<Opinion> opinions = _context.Opinions;

            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                opinions = opinions.Include(navigationPropertyPath);
            }

            return await Get(opinions, id);
        }

        private static async Task<Opinion> Get(IQueryable<Opinion> opinions, Guid id)
        {
            var opinion = await opinions
                .FirstOrDefaultAsync(x => x.Id == id);

            if (opinion == null)
            {
                throw new Exception(Errors.ElementDoseNotExist);
            }

            return opinion;
        }

        public async Task<Opinion[]> Get(IReadOnlyCollection<Guid> ids, int? start = null, int? count = null, params Expression<Func<Opinion, object>>[] navigationPropertyPaths)
        {
            IQueryable<Opinion> opinions = _context.Opinions;

            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                opinions = opinions.Include(navigationPropertyPath);
            }

            return await opinions
                .Where(x => ids.Contains(x.Id))
                .TryTake(start, count)
                .ToArrayAsync();
        }

        public async Task<Opinion[]> Get(int? start = null, int? count = null, params Expression<Func<Opinion, object>>[] navigationPropertyPaths)
        {
            IQueryable<Opinion> opinions = _context.Opinions;

            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                opinions = opinions.Include(navigationPropertyPath);
            }

            return await opinions
                .TryTake(start, count)
                .ToArrayAsync();
        }

        public async Task<bool> IAmAuthor(string userId, Guid opinionId)
        {
            return await _context.Opinions.AnyAsync(x => x.Id == opinionId && x.AuthorId == userId);
        }

        public async Task<ILookup<Guid, Opinion>> GetByOrderIds(IEnumerable<Guid> ownerIds)
        {
            var accounts = await _context.Opinions
                //  .Include(x => x.Author)
                .Include(x => x.Order)
                .Where(a => ownerIds.Contains(a.Order.Id)).ToListAsync();

            return accounts.ToLookup(x => x.Order.Id);
        }

        public async Task<ILookup<string, Opinion>> GetByAuthorIds(IEnumerable<string> authorIds)
        {
            var accounts = await _context.Opinions
                .Include(x => x.Author)
                //.Include(x => x.Order)
                .Where(a => authorIds.Contains(a.Author.Id)).ToListAsync();

            return accounts.ToLookup(x => x.Author.Id);
        }

        public async Task<Opinion> Create(CreateOpinionViewModel model, string userId)
        {
            var opinionExistTask = _context.Opinions.AnyAsync(x => x.AuthorId == userId && x.OrderId == model.OrderId);

            var myOrderTask = _context.Orders.AnyAsync(x => x.OwnerId == userId && x.Id == model.OrderId);

            if (await  opinionExistTask)
            {
                throw new Exception(Errors.OrderIsNotExistInList);
            }

            if(await myOrderTask)
            {
                throw new Exception(Errors.YouCantCommentyourOrder);
            }
            
            var opinion = new Opinion(userId, model.OrderId, model);

            _context.Opinions.Add(opinion);

            await _context.SaveChangesAsync();

            return opinion;
        }

        public async Task<Opinion> Update(Guid opinionId, OpinionViewModel model)
        {
            var opinion = await Get(opinionId);

            opinion.Assign(model);
            await _context.SaveChangesAsync();

            return opinion;
        }

        public void Delete(Guid opinionId, bool saveChanges)
        {
            var opinion = Get(opinionId).Result;

            _context.Opinions.Remove(opinion);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
    }
}
