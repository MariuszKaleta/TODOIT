using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using ManyForMany.Models.Configuration;
using ManyForMany.ViewModel.Team;
using Microsoft.Extensions.Logging;
using MvcHelper.Entity;

namespace ManyForMany.Models.Entity.Chat
{
    public class Chat : IId<string>
    {
        public Chat()
        {

        }

        public Chat(ApplicationUser creator, Context context, CreateChatViewModel model)
        {
            var users  = context.Users.Get(model.MemeberUsersId).ToList();

            this.Add(users);
            this.Add(creator);

            AdminId = creator.Id;
        }

        [Key]
        public string Id { get; private set; }

        public List<ApplicationUser> Members { get; private set; }

        public string AdminId { get; private set; }

        public List<Message> Messages { get; private set; }
    }

    public static class ChatExtension
    {
        public static bool IAmAdmin(this ApplicationUser user, Chat chat)
        {
            return user.Id == chat.AdminId;
        }

        public static void Add(this Chat chat, IEnumerable<ApplicationUser> users)
        {
            foreach (var user in users)
            {
                chat.Add(user);
            }
        }

        public static void Add(this Chat chat, ApplicationUser user)
        {
            chat.Members.Add(user);

            user.Chats.Add(chat);
        }

        public static void Remove(this Chat chat, IEnumerable<ApplicationUser> users)
        {
            foreach (var user in users)
            {
                chat.Remove(user);
            }
        }

        public static void Remove(this Chat chat, ApplicationUser user)
        {
            chat.Members.Remove(user);

            user.Chats.Remove(chat);
        }

        public static async Task<Chat> Get(this IQueryable<Chat> users, string id, ILogger logger)
        {
            return await users.Get(id, Errors.ChatIsNotExist, logger);
        }
    }
}
