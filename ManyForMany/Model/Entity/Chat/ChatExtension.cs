using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TODOIT.Model.Entity.User;

namespace TODOIT.Model.Entity.Chat
{
    public static class ChatExtension
    {
        public static void Add(this TeamChat chat, IEnumerable<ApplicationUser> users)
        {
            foreach (var user in users)
            {
                chat.Add(user);
            }
        }

        public static void Add(this TeamChat chat, ApplicationUser user)
        {
            chat.Members.Add(user);
        }

        public static void Remove(this TeamChat chat, IEnumerable<ApplicationUser> users)
        {
            foreach (var user in users)
            {
                chat.Remove(user);
            }
        }

        public static void Remove(this TeamChat chat, ApplicationUser user)
        {
            chat.Members.Remove(user);
        }

        public static async Task<TeamChat> Get(this IQueryable<TeamChat> chats, string id, ILogger logger)
        {
            return await chats.Get(id, logger);
        }
    }
}