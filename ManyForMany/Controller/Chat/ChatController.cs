using System;
using System.Threading.Tasks;
using GraphQL.Tests.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.Chat;
using TODOIT.Model.Entity.User;

namespace TODOIT.Controller.Chat
{
    [Obsolete]
    [Authorize]
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class ChatController : Microsoft.AspNetCore.Mvc.Controller
    {
    
        /*
      [MvcHelper.Attributes.HttpPost(nameof(SendMessage))]
      public void SendMessage(string messageText)
      {
          var message = new Message(messageText)
          {
              Content = messageText
          };

          _chat.AddMessage(message);
      }


      [MvcHelper.Attributes.HttpGet("{chatId}", nameof(Messages))]
      public async Task<Message[]> Create(string chatId, [FromQuery] string searchText, [FromQuery] int? start, [FromQuery]  int? count)
      {
          var receivedMessage = context.GetArgument<ReceivedMessage>("message");
          var message = chat.AddMessage(receivedMessage);
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
