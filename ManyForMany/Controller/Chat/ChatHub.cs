using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using Newtonsoft.Json;
using OpenIddict.Validation;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.Chat;
using TODOIT.Model.Entity.User;
using IClientProxy = Microsoft.AspNetCore.SignalR.IClientProxy;

namespace TODOIT.Controller.Chat
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly Context _context;
        private readonly Logger<ChatHub> _logger;

        private ConcurrentDictionary<ApplicationUser, (Model.Entity.Chat.Chat chat, IClientProxy client)> ActualUserChatToGroupName =
            new ConcurrentDictionary<ApplicationUser, (Model.Entity.Chat.Chat chat, IClientProxy client)>();
        /*
        public ChatHub(UserManager<ApplicationUser> userManager, Context context, Logger<ChatHub> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async void Echo(string text)
        {
            await Clients.Caller.SendAsync(text);
        }

        public async Task Connect(string chatId)
        {
            _context.SaveChanges();

            var user = await _userManager.GetUserAsync(Context.User);

            var chat = _context.TeamChats
                .FirstOrDefault(x => x.Id == chatId && x.Members.Any(y => y.Id == user.Id));

            //TODO tylko w jednej grupie naraz można być przy connec
            if (chat == null)
            {
                throw new MultiLanguageException(chatId, Errors.YouDontBelngToChat);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            var group = Clients.Group(chatId);

            ActualUserChatToGroupName.AddOrUpdate(user, (chat, group), (x, y) => y);
        }

        public async Task Send(string text)
        {
            var user = await _userManager.GetUserAsync(Context.User);

            if (!ActualUserChatToGroupName.TryGetValue(user, out var value))
            {
                throw new MultiLanguageException(nameof(user), Errors.YouMustLog);
            }

            var message = new Message(user, value.chat, text);

            _context.Messages.Add(message);

            var json = JsonConvert.SerializeObject(message);

            var task = value.client.SendAsync(json);

            await task;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await _userManager.GetUserAsync(Context.User);

            if (!ActualUserChatToGroupName.Remove(user, out var value))
            {
                throw new MultiLanguageException(nameof(user), Errors.YouMustLog);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, value.chat.Id);

            _context.SaveChanges();

            await base.OnDisconnectedAsync(exception);
        }
        */

    }
}