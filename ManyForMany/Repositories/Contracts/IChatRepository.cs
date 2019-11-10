using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GraphQlHelper;
using TODOIT.Model.Entity.Chat;
using TODOIT.Model.Entity.User;
using TODOIT.ViewModel.Team;

namespace TODOIT.Repositories.Contracts
{
    public interface IChatRepository
    {
        Task<Chat> Get(Guid id, params Expression<Func<Chat, object>>[] navigationPropertyPaths);

        Task<Chat[]> Get(IEnumerable<Guid> ids, params Expression<Func<Chat, object>>[] navigationPropertyPaths);

        Task<Chat[]> Get(string name = null, int? start = null, int? count = null, params Expression<Func<Chat, object>>[] navigationPropertyPaths);

        Task<Chat[]> GetAllChatWithUser(string userId, params Expression<Func<Chat, object>>[] navigationPropertyPaths);


        Task<ApplicationUser[]> GetAllChatMembers(Guid chatId);

        Task<ILookup<Guid, ApplicationUser>> GetByChatIdMembers(IEnumerable<Guid> orderIds);

        Task AddUserToChat(Guid chatId, bool saveChanges, params string[] userIds);

        Task RemoveuserFromChat(Guid chatId, bool saveChanges, params string[] userIds);

        Task<Chat> GetByOrderId(Guid orderId);

        Task<Chat> Create(Guid orderId);
    }
}
