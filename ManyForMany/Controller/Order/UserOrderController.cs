using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using AuthorizeTester.Model;
using ManyForMany.Model.Entity;
using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Model.File;
using ManyForMany.Models.Configuration;
using ManyForMany.ViewModel.Order;
using ManyForMany.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using MvcHelper;
using MvcHelper.Entity;

namespace ManyForMany.Controller.User
{
    [ApiController]
    [Authorize]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class UserOrderController : Microsoft.AspNetCore.Mvc.Controller
    {
        public UserOrderController(ILogger<OrderController> logger, Context context, UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            _logger = logger;
            _context = context;
        }

        #region Properties

        public UserManager<ApplicationUser> UserManager { get; }
        private ILogger<OrderController> _logger;
        private readonly Context _context;
        private readonly ImageManager ImageManager = new ImageManager();

        #endregion

        /// <summary>
        /// Create Order using creteorderViewModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost()]
        public async Task Create(OrderViewModel model)
        {
            var userId = UserManager.GetUserId(User);

            var user = await _context.Users
                //.Include(x => x.Chats)
                .Include(x => x.OwnOrders)
                .Get(userId, _logger);

            var order = new Order(model, user);
            /*
            var chat = new Chat(user)
            {
                Name = model.Title
            };
            */
            _context.Orders.Add(order);

           // _context.Chats.Add(chat);

            _context.SaveChanges();

            //order.ProjectChatId = chat.Id;

            //user.Chats.Add(chat);
            user.OwnOrders.Add(order);

            _context.SaveChanges();

            if (model.Images != null)
            {
                await ImageManager.UploadOrderImages(model.Images, user.Id, order.Id);

                _logger.LogInformation(nameof(ImageManager.UploadOrderImages), model.Images);
            }
        }

        /*

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpDelete("{id}")]
        public async Task Remove(int id)
        {
            var userId = UserManager.GetUserId(User);

            var user = await _context.Users
                    .Include(x => x.OwnOrdersId)
                    .FirstOrDefaultAsync(x => x.Id == userId)
                ;

            var order = await _context.Orders
                .Include(x=>x.InterestedUsersId)
                
                .GetIfContain(user.OwnOrdersId, id);

            var interestedUsers = _context.Users.Get(order.InterestedUsersId);

            foreach (var interestedUser in interestedUsers)
            {
                interestedUser.InterestedOrdersId.Remove(order.Id);
            }

            order.


            user.OwnOrdersId.Remove(order.Id);

            _context.SaveChanges();

            await ImageManager.RemoveFiles(user.Id, order.Id);
        }
        */

        #region Edit

        /// <summary>
        /// Change Status of order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost("{orderId}", nameof(ChangeStatus), "{status}")]
        public async Task ChangeStatus(int orderId, OrderStatus status)
        {
            var userId = UserManager.GetUserId(User);

            var user = await _context.Users
                .Include(x => x.OwnOrders)
                .Get(userId, _logger);

            var order = user.OwnOrders.Get(orderId, _logger);

            order.Status = status;

            _context.SaveChanges();
        }

        /// <summary>
        /// Edit Order 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPut("{id}")]
        public async Task Edit(int id, OrderViewModel model)
        {
            var userId = UserManager.GetUserId(User);

            var user = await _context.Users
                .Include(x => x.OwnOrders)
                .Get(userId, _logger);


            var order = user.OwnOrders.Get(id, _logger);

            order.Edit(model);

            _context.SaveChanges();
        }

        #endregion

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet(nameof(All))]
        public async Task<ShowPublicOrderViewModel[]> All([FromQuery] int? start, [FromQuery] int? count)
        {
            var userId = UserManager.GetUserId(User);

            var user = await _context.Users
                .Include(x => x.OwnOrders)
                .Get(userId, _logger);


            return user.OwnOrders.TryTake(start, count).Select(x => x.ToPublicInformation(ImageManager)).ToArray();
        }


        #region Users

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet("{orderId}", nameof(InterstedUsers))]
        public async Task<UserThumbnailViewModel[]> InterstedUsers(int orderId, [FromQuery] int? start, [FromQuery] int? count)
        {
            var userId = UserManager.GetUserId(User);

            var user = await _context.Users
                    .Include(x => x.OwnOrders)
                        .ThenInclude(x => x.InterestedUsers)
                    .FirstOrDefaultAsync(x => x.Id == userId)
                ;

            var order = user.OwnOrders.Get(orderId, _logger);

            return order.InterestedUsers.TryTake(start, count).Select(x => x.ToUserThumbnail()).ToArray();
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost("{orderId}", nameof(AddUser), "{addUserId}")]
        public async Task AddUser(int orderId, string addUserId)
        {
            var userId = UserManager.GetUserId(User);

            var ownOrders = _context.Users
                .Include(x => x.OwnOrders);

            ownOrders.ThenInclude(x => x.ActualTeam);
            ownOrders.ThenInclude(x => x.InterestedUsers);

            var user = await ownOrders.Get(userId, _logger);

            var order = user.OwnOrders
                .Get(orderId, _logger);

            if (order.ActualTeam.Exists(x => x.Id == userId))
            {
                throw new MultiLanguageException(nameof(userId), Errors.UserIsAlredyAdded);
            }

            var userToAdd = await _context.Users
                .Include(x => x.MemberOfOrders)
                .Include(x => x.InterestedOrders)
                .Get(addUserId, _logger);

            if (!userToAdd.InterestedOrders.Exists(x => x.Id == orderId))
            {
                throw new MultiLanguageException(nameof(userId), Errors.UserIsNotInterestedOrder);
            }

            order.AddUserToProject(userToAdd);

            _context.SaveChanges();
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost("{id}", nameof(RemoveUser), "{removeUserId}")]
        public async Task RemoveUser(int id, string removeUserId)
        {
            var userId = UserManager.GetUserId(User);

            var ownOrders = _context.Users
                .Include(x => x.OwnOrders);

            ownOrders.ThenInclude(x => x.ActualTeam);
            ownOrders.ThenInclude(x => x.InterestedUsers);

            var user = await ownOrders.Get(userId, _logger);

            var order = user.OwnOrders
                .Get(id, _logger);

            if (!order.ActualTeam.Exists(x => x.Id == userId))
            {
                throw new MultiLanguageException(nameof(userId), Errors.UserIsNotAdded);
            }

            var userToRemove = await _context.Users
                .Include(x => x.MemberOfOrders)
                .Include(x => x.InterestedOrders)
                .Get(removeUserId, _logger);

            order.RemoveUserFromProject(userToRemove);

            _context.SaveChanges();
        }

        #endregion
    }
}
