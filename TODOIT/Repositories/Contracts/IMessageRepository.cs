using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GraphQlHelper;
using TODOIT.Model.Entity.Chat;

namespace TODOIT.Repositories.Contracts
{
    public interface IMessageRepository 
    {
        Task<Message> Get(Guid id, params Expression<Func<Message, object>>[] navigationPropertyPaths);

        Task<Message[]> Get(IEnumerable<Guid> ids, params Expression<Func<Message, object>>[] navigationPropertyPaths);

        Task<Message[]> Get(Guid chatId, string name = null, int? start = null, int? count = null, params Expression<Func<Message, object>>[] navigationPropertyPaths);
        
        Task<ILookup<Guid, Message>> GetMessagesByChatIds(IEnumerable<Guid> orderIds);

        Task Create(string authorId, Guid chatId, string text, bool saveChanges);
    }
}
