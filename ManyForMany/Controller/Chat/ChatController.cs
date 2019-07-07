using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity;
using ManyForMany.Models.Entity.Chat;
using ManyForMany.Models.Entity.User;
using ManyForMany.ViewModel.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using MvcHelper;
using MvcHelper.Attributes;
using MvcHelper.Entity;

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

        [MvcHelper.Attributes.HttpGet("{chatId}", nameof(Messages))]
        public async Task<Message[]> Messages(string chatId, [FromQuery] string searchText, [FromQuery] int? start, [FromQuery]  int? count)
        {
            var userId = UserManager.GetUserId(User);


            return _context.Messages
                .Where(x => x.Chat.Id == chatId && x.Chat.Members.Any(y => y.Id == userId))
                .Filter(y => y.Text, searchText, true)
                .TryTake(start, count)
                .ToArray();
        }

        #region Group


        #region Get

        [MvcHelper.Attributes.HttpGet(nameof(Group))]
        public async Task<Models.Entity.Chat.TeamChat[]> Group([FromQuery] int? start, [FromQuery] int? count)
        {
            var id = UserManager.GetUserId(User);

            return _context.TeamChats
                .Where(x => x.Members.Any(y => y.Id == id))
                .TryTake(start, count).ToArray();
        }

        [MvcHelper.Attributes.HttpGet(nameof(Group), "{chatId}")]
        public async Task<Models.Entity.Chat.TeamChat> Get(string chatId)
        {

            var userId = UserManager.GetUserId(User);

            return await _context.TeamChats
                .Where(x => x.Members.Any(y => y.Id == userId))
                .Get(chatId, _logger);
        }

        #endregion

        [MvcHelper.Attributes.HttpPost(nameof(Group), nameof(Create))]
        public async Task Create(CreateChatViewModel model)
        {
            var id = UserManager.GetUserId(User);

            var admin = await _context.Users.Get(id, _logger);

            var chat = new Models.Entity.Chat.TeamChat(admin, _context, model);

            _context.TeamChats.Add(chat);

            _context.SaveChanges();
        }

        [MvcHelper.Attributes.HttpPost(nameof(Group), nameof(Users))]
        public async Task Users(string[] userId, string chatId)
        {
            var adminId = UserManager.GetUserId(User);

            var chatTask = _context.TeamChats
                .Where(x => x.Admin.Id == adminId).Get(chatId, _logger);

            var users = _context.Users.Get<ApplicationUser>(userId);

            var chat = await chatTask;

            if (chat == null)
            {
                throw new MultiLanguageException(adminId, Errors.ThisIsNotYourChat);
            }

            chat.Add(users);

            _context.SaveChanges();
        }

        [MvcHelper.Attributes.HttpDelete(nameof(Group), nameof(Users))]
        public async Task RemoveUsers(string[] userId, string chatId)
        {
            var adminId = UserManager.GetUserId(User);

            var chatTask = _context.TeamChats.Where(x => x.Admin.Id == adminId).Get(chatId, _logger);

            var users = _context.Users.Get<ApplicationUser>(userId);

            var chat = await chatTask;

            if (chat == null)
            {
                throw new MultiLanguageException(adminId, Errors.ThisIsNotYourChat);
            }

            chat.Remove(users);

            _context.SaveChanges();
        }

        #endregion


        #region Helper



        #endregion
    }
}
