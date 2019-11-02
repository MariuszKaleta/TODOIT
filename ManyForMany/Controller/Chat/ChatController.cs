﻿using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.User;

namespace TODOIT.Controller.Chat
{
    [Obsolete]
    [Authorize]
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class ChatController : Microsoft.AspNetCore.Mvc.Controller
    {
        public ChatController( Context context, UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            _context = context;
        }

        #region Properties

        public UserManager<ApplicationUser> UserManager { get; }
        private ILogger<ChatController> _logger;
        private readonly Context _context;

        #endregion

        /*

        [MvcHelper.Attributes.HttpGet("{chatId}", nameof(Messages))]
        public async Task<Message[]> Messages(string chatId, [FromQuery] string searchText, [FromQuery] int? start, [FromQuery]  int? count)
        {
            var userId = UserManager.GetUserId(User);

            return _context.Messages
                .Where(x => x.Chat.Id == chatId && x.Chat.Members.Any(y => y.Id == userId))
                .Filter(searchText, y => y.Text)
                .TryTake(start, count)
                .ToArray();
        }

    */

        #region Group

        /*
    #region Get

    [MvcHelper.Attributes.HttpGet(nameof(Group))]
    public async Task<TeamChat[]> Group([FromQuery] int? start, [FromQuery] int? count)
    {
        var id = UserManager.GetUserId(User);

        return _context.TeamChats
            .Where(x => x.Members.Any(y => y.Id == id))
            .TryTake(start, count).ToArray();
    }

    [MvcHelper.Attributes.HttpGet(nameof(Group), "{chatId}")]
    public async Task<TeamChat> Get(string chatId)
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

        var admin = await _context.Users.Get(id);

        var chat = new TeamChat(admin, _context, model);

        _context.TeamChats.Add(chat);

        _context.SaveChanges();
    }


    [MvcHelper.Attributes.HttpPost(nameof(Group), nameof(Users))]
    public async Task Users(string[] userId, string chatId)
    {
        var adminId = UserManager.GetUserId(User);

        var chatTask = _context.TeamChats
            .Where(x => x.Admin.Id == adminId).Get(chatId,_logger);

        var users = _context.Users.Get(userId);

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

        var users = _context.Users.Get(userId);

        var chat = await chatTask;

        if (chat == null)
        {
            throw new MultiLanguageException(adminId, Errors.ThisIsNotYourChat);
        }

        chat.Remove(users);

        _context.SaveChanges();
    }

    */

        #endregion


        #region Helper



        #endregion
    }
}
