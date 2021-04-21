using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TODOIT.Repositories.Contracts;

namespace TODOIT.Model.Entity.Chat
{
    public static class ChatHubExtension
    {
        public static async void SubscribeAllMyChats(this Hub hub, IChatRepository repository, string userId)
        {
            var allMyChats = (await repository.GetAllChatWithUser(userId)).ToArray();

            await Task.WhenAll(allMyChats.Select(chat =>
                hub.Groups.AddToGroupAsync(hub.Context.ConnectionId, chat.Id.ToString())));
        }
        public static async void UnSubscribeAllMyChats(this Hub hub, IChatRepository repository, string userId)
        {
            var allMyChats = (await repository.GetAllChatWithUser(userId)).ToArray();

            await Task.WhenAll(allMyChats.Select(chat =>
                hub.Groups.RemoveFromGroupAsync(hub.Context.ConnectionId, chat.Id.ToString())));
        }
    }
}