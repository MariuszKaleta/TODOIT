using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity.User;
using ManyForMany.ViewModel.Team;
using Microsoft.Extensions.Logging;
using MvcHelper.Entity;

namespace ManyForMany.Models.Entity.Chat
{
    public class TeamChat : Chat
    {
        private TeamChat()
        {

        }

        public TeamChat(ApplicationUser creator, Context context, CreateChatViewModel model)
        {
            var users = context.Users.Get(model.MemeberUsersId).ToList();
            Members = new List<ApplicationUser>();
            this.Add(users);
            this.Add(creator);

            Admin = creator;
        }

        [Required]
        public ApplicationUser Admin { get; private set; }
    }


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
            return await chats.Get(id, Errors.ChatIsNotExist, logger);
        }
    }
}
