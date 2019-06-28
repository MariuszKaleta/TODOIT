using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using ManyForMany.Model.File;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity.Chat;
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

        #region Get


        [MvcHelper.Attributes.HttpGet()]
        public async Task<List<Models.Entity.Chat.Chat>> GetAll()
        {
            var id = UserManager.GetUserId(User);
            var user = await _context.Users.Include(x => x.Chats).Get(id, _logger);

            return user.Chats;
        }

        [MvcHelper.Attributes.HttpGet("{chatId}")]
        public async Task<Models.Entity.Chat.Chat> Get(int chatId)
        {
            var id = UserManager.GetUserId(User);
            var user = await _context.Users.Include(x => x.Chats).Get(id, _logger);

            var chat = user.Chats.Get(chatId, _logger);

            return chat;
        }

        #endregion

        [MvcHelper.Attributes.HttpPost(nameof(Create))]
        public async Task Create(CreateChatViewModel model)
        {
            var id = UserManager.GetUserId(User);

            var admin = await _context.Users.Include(x => x.Chats).Get(id, _logger);

            var chat = new Models.Entity.Chat.Chat(admin, _context, model);

            _context.SaveChanges();
        }

        [MvcHelper.Attributes.HttpPost(nameof(AddUsers))]
        public async Task AddUsers(string[] userId, int chatId)
        {
            var adminTask = _context.Users.Include(x => x.Chats).
                Get(UserManager.GetUserId(User), _logger);

            var chat = await _context.Chats.Get(chatId, _logger);
            var admin = await adminTask;
            var users = _context.Users.Get<ApplicationUser>(userId);


            if (chat.AdminId != admin.Id)
            {
                throw new MultiLanguageException(chat.AdminId, Errors.ThisIsNotYourChat);
            }

            chat.Add(users);

            _context.SaveChanges();
        }

        [MvcHelper.Attributes.HttpPost(nameof(RemoveUsers))]
        public async Task RemoveUsers(string[] userId, int chatId)
        {
            var adminTask = _context.Users.Include(x => x.Chats).
                Get(UserManager.GetUserId(User), _logger);

            var chat = await _context.Chats.Get(chatId, _logger);
            var admin = await adminTask;
            var users = _context.Users.Get<ApplicationUser>(userId);


            if (chat.AdminId != admin.Id)
            {
                throw new MultiLanguageException(chat.AdminId, Errors.ThisIsNotYourChat);
            }

            chat.Remove(users);

            _context.SaveChanges();
        }

        [MvcHelper.Attributes.HttpGet("{chatId}", nameof(Messages))]
        public async Task<List<Message>> Messages(int chatId, [FromQuery] string searchText, [FromQuery] int? start, [FromQuery]  int? count)
        {
            var user = await UserManager.GetUserAsync(User);

            var chat = _context.Chats
                .Include(x => x.Members)
                .Where(x => x.Members.AsQueryable().Contains(user))
                .Include(y => y
                    .Messages.AsQueryable()
                    .Filter(z => z.Text, searchText, true)
                    .TryTake(start, count))
                .FirstOrDefault(x => x.Id == chatId);
                
            if (chat == null)
            {
                throw new MultiLanguageException(nameof(chatId), Errors.ThisIsNotYourChat);
            }

            return chat.Messages;

        }

        #region Helper



        #endregion
    }
}
