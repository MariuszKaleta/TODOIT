using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcHelper.Entity;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.Chat;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories.Contracts;
using TODOIT.ViewModel.Team;

namespace TODOIT.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly Context _context;

        public ChatRepository(Context context)
        {
            _context = context;
        }

        public async Task<Chat> Get(Guid id, params Expression<Func<Chat, object>>[] navigationPropertyPaths)
        {
            IQueryable<Chat> chats = _context.Chats;

            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                chats = chats.Include(navigationPropertyPath);
            }

            var chat = await chats.FirstOrDefaultAsync(x => x.Id == id);

            if (chat == null)
            {
                throw new Exception(Errors.ChatIsNotExist);
            }

            return chat;
        }

        public async Task<Chat[]> Get(IEnumerable<Guid> ids, params Expression<Func<Chat, object>>[] navigationPropertyPaths)
        {
            IQueryable<Chat> chats = _context.Chats;

            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                chats = chats.Include(navigationPropertyPath);
            }

            return await chats
                .Where(x => ids.Contains(x.Id))
                .ToArrayAsync();
        }

        public async Task<Chat[]> Get(string name = null, int? start = null, int? count = null, params Expression<Func<Chat, object>>[] navigationPropertyPaths)
        {
            IQueryable<Chat> chats = _context.Chats;

            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                chats = chats.Include(navigationPropertyPath);
            }

            if (!string.IsNullOrEmpty(name))
            {
                chats = chats.Where(x => x.Order.Name.Contains(name) || x.Order.Owner.Name.Contains(name));
            }

            return await chats.TryTake(start, count).ToArrayAsync();
        }

        public async Task<Chat[]> GetAllChatWithUser(string userId, params Expression<Func<Chat, object>>[] navigationPropertyPaths)
        {
            IQueryable<Chat> chats = _context.ChatMembers.Where(x => x.UserId == userId).Select(x => x.Chat);

            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                chats = chats.Include(navigationPropertyPath);
            }

            var myOrderChatsTask = _context.Chats.Where(x => x.Order.OwnerId == userId).ToArrayAsync();

            var memberOfChatsTask = chats.ToArrayAsync();

            var chatList = new List<Chat>();

            chatList.AddRange(await myOrderChatsTask);
            chatList.AddRange(await memberOfChatsTask);


            return chatList.ToArray();
        }

        public async Task<ILookup<Guid, ApplicationUser>> GetByChatIdMembers(IEnumerable<Guid> orderIds)
        {
            {
                var accounts = await _context.ChatMembers
                    .Include(x => x.User)
                    .Where(a => orderIds.Contains(a.ChatId)).ToListAsync();

                return accounts.ToLookup(x => x.ChatId, x => x.User);
            }
        }

        public async Task<ApplicationUser[]> GetAllChatMembers(Guid chatId)
        {
            return await _context.ChatMembers
                .Where(x => x.ChatId == chatId)
                .Select(x => x.User)
                .ToArrayAsync();
        }

        public async Task<Chat> GetByOrderId(Guid orderId)
        {
            return await _context.Chats.FirstOrDefaultAsync(x => x.Id == orderId);
        }

        public async Task AddUserToChat(Guid chatId, bool saveChanges, params string[] userIds)
        {
            foreach (var userId in userIds)
            {
                var chatMember = new ChatMember(userId, chatId);

                _context.ChatMembers.Add(chatMember);
            }

            if (saveChanges)
                await _context.SaveChangesAsync();
        }

        public async Task RemoveUserFromChat(Guid chatId, bool saveChanges, params string[] userIds)
        {
            var chatMembers = _context.ChatMembers.Where(x => userIds.Any(y => y == x.UserId));

            _context.ChatMembers.RemoveRange(chatMembers);

            if (saveChanges)
                await _context.SaveChangesAsync();
        }



        public async Task<Chat> Create(Guid orderId)
        {
            if (_context.Chats.Any(x => x.Id == orderId))
            {
                throw new Exception(Errors.ChatIsExist);
            }

            var chat = new Chat(orderId);

            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();

            return chat;

        }
    }
}
