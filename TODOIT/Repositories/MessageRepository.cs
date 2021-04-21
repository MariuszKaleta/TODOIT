using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.Chat;
using TODOIT.Repositories.Contracts;

namespace TODOIT.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly Context _context;

        public MessageRepository(Context context)
        {
            _context = context;
        }

        public async Task<Message> Get(Guid id, params Expression<Func<Message, object>>[] navigationPropertyPaths)
        {
            IQueryable<Message> messages = _context.Messages;

            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                messages = messages.Include(navigationPropertyPath);
            }

            var chat = await messages.FirstOrDefaultAsync(x => x.Id == id);

            if (chat == null)
            {
                throw new Exception(Errors.ChatIsNotExist);
            }

            return chat;
        }

        public async Task<Message[]> Get(IEnumerable<Guid> ids, params Expression<Func<Message, object>>[] navigationPropertyPaths)
        {
            IQueryable<Message> messages = _context.Messages;

            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                messages = messages.Include(navigationPropertyPath);
            }

            return await messages
                .Where(x => ids.Contains(x.Id))
                .ToArrayAsync();
        }

        public async Task<Message[]> Get(Guid chatId, string name = null, int? start = null, int? count = null,
            params Expression<Func<Message, object>>[] navigationPropertyPaths)
        {
            IQueryable<Message> messages = _context.Messages.Where(x => x.ChatId == chatId);
            
            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                messages = messages.Include(navigationPropertyPath);
            }

            if (!string.IsNullOrEmpty(name))
            {
                messages = messages.Where(x => x.Text.Contains(name) || x.Author.Name.Contains(name));
            }

            return await messages.ToArrayAsync();
        }

        public async Task<ILookup<Guid, Message>> GetMessagesByChatIds(IEnumerable<Guid> orderIds)
        {
            var accounts = await _context.Messages
                .Where(a => orderIds.Contains(a.ChatId)).ToListAsync();

            return accounts.ToLookup(x => x.ChatId);
        }

        public async Task Create(string authorId, Guid chatId, string text, bool saveChanges)
        {
            var message = new Message(authorId, chatId, text);
            _context.Messages.Add(message);
            if (saveChanges)
                await _context.SaveChangesAsync();


        }
    }
}
