using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories.Contracts;

namespace TODOIT.Model.Entity.Chat
{
    public class ChatHub : Hub
    {
        public const string RecaiveMessage = nameof(RecaiveMessage);

        private readonly IChatRepository _chatRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMessageRepository _messageRepository;

        public ChatHub(IChatRepository chatRepository, UserManager<ApplicationUser> userManager, IMessageRepository messageRepository)
        {
            _chatRepository = chatRepository;
            _userManager = userManager;
            _messageRepository = messageRepository;
        }

        public override Task OnConnectedAsync()
        {
            var userId = _userManager.GetUserId(Context.User);

            this.SubscribeAllMyChats(_chatRepository, userId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = _userManager.GetUserId(Context.User);

            this.UnSubscribeAllMyChats(_chatRepository, userId);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task Send(Guid chatId, string message)
        {
            var userId = _userManager.GetUserId(Context.User);

            //TODO add message to database
            var dbMessageTask = _messageRepository.Create(userId, chatId, message, true);

            await Clients.Group(chatId.ToString()).SendAsync(RecaiveMessage, chatId, message);

            await dbMessageTask;
        }
    }
}
