using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using ManyForMany.Model.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcHelper.Attributes;

namespace ManyForMany.Controller.Chat
{
    [Authorize]
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class ChatController : Microsoft.AspNetCore.Mvc.Controller
    {
        public ChatController(ILogger<ChatController> logger, Context context, UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            _logger = logger;
            _context = context;
        }

        #region Properties

        public UserManager<ApplicationUser> UserManager { get; }
        private ILogger<ChatController> _logger;
        private readonly Context _context;

        #endregion

        [MvcHelper.Attributes.HttpPost("{chatId}", nameof(Send))]
        public void Send(string text)
        {
            throw new NotImplementedException();
        }
    }
}
