using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using AuthorizationServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using OpenIddict.Abstractions;
using OpenIddict.Validation;

namespace SignalRChat
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        private UserManager<ApplicationUser> _userManager;

        public ChatHub(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public Task Join()
        {
            
            
            return Groups.AddToGroupAsync(Context.ConnectionId, "foo");
        }

        public async Task Send(string message)
        {
            var user = await _userManager.GetUserAsync(Context.User);

            Clients.All.SendAsync(message);
        }

        public Task Disconnect()
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, "foo");
        }

        public Task SendToAll(string name, string message)
        {
            return Clients.All.SendAsync(message);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}