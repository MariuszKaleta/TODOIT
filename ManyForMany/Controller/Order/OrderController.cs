using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using AuthorizeTester.Model;
using ManyForMany.Model.Entity;
using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Model.Entity.Orders;
using ManyForMany.Model.File;
using ManyForMany.ViewModel.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using MvcHelper;
using MvcHelper.Attributes;
using MvcHelper.Entity;

namespace ManyForMany.Controller
{
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class OrderController : Microsoft.AspNetCore.Mvc.Controller
    {

        public OrderController(ILogger<OrderController> logger, Context context, UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            _logger = logger;
            _context = context;
        }

        #region Properties
        
        public UserManager<ApplicationUser> UserManager { get; }
        private ILogger<OrderController> _logger;
        private readonly Context _context;
        private ImageManager imageManager = new ImageManager();

        #endregion

        #region API

        #region Get

        [AllowAnonymous]
        [MvcHelper.Attributes.HttpGet(nameof(AvailableStatus))]
        public Dictionary<int, string> AvailableStatus()
        {
            return Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>()
                .ToDictionary(x => (int) x, x => x.ToString());
        }


        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet("{id}")]
        public async Task<ShowPublicOrderViewModel> Get(int id)
        {
            var order = await _context.Orders.Get(id, _logger);

            return order.ToPublicInformation(imageManager);
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet(nameof(LookNew))]
        public async Task<ShowPublicOrderViewModel[]> LookNew([FromQuery] int? start,[FromQuery] int? count = 5)
        {
            var userId = UserManager.GetUserId(User);

            var user = await _context.Users
                .Include(x => x.InterestedOrders)
                .Include(x => x.RejectedOrders)
                .Include(x => x.OwnOrders)
                .Get(userId, _logger);

            var watched = user.WatchedAndUserOrdersId().ToArray();

            var orders = _context.Orders.Except(watched);

            return orders.Where(x => x.Status == OrderStatus.CompleteTeam).TryTake(start, count)
                .Select(x => x.ToPublicInformation(imageManager)).ToArray();
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet(nameof(Watched))]
        public async Task<ShowPublicOrderViewModel[]> Watched([FromQuery] int? start, [FromQuery] int? count = 5)
        {
            var userId = UserManager.GetUserId(User);

            var user = await _context.Users
                    .Include(x => x.InterestedOrders)
                    .Include(x => x.RejectedOrders)
                    .Get(userId, _logger);

            var watched = user.WatchedOrdersId();

            return watched.TryTake(start ,count).Select(x => x.ToPublicInformation(imageManager)).ToArray();
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet(nameof(Interested))]
        public async Task<ShowPublicOrderViewModel[]> Interested([FromQuery] int? start, [FromQuery] int? count = 5)
        {
            var userId = UserManager.GetUserId(User);

            var user = await _context.Users
                    .Include(x => x.InterestedOrders)
                    .Get(userId, _logger);

            return user.InterestedOrders.TryTake(start, count).Select(x => x.ToPublicInformation(imageManager)).ToArray();
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet(nameof(Rejected))]
        public async Task<ShowPublicOrderViewModel[]> Rejected([FromQuery] int? start, [FromQuery] int? count = 5)
        {
            var userId = UserManager.GetUserId(User);

            var user = await _context.Users
                    .Include(x => x.RejectedOrders)
                    .Get(userId, _logger);

            return user.RejectedOrders.TryTake(start, count).Select(x => x.ToPublicInformation(imageManager)).ToArray();
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost(nameof(Decide), "{orderId}", "{decision}")] 
        public async Task<bool> Decide(int orderId, bool decision)
        {
            var userId = UserManager.GetUserId(User);

            var user = await _context.Users
                .Include(x => x.InterestedOrders)
                .Include(x => x.RejectedOrders)
                .Get(userId, _logger);

            var result =  await Decide(user,orderId, decision);

            await _context.SaveChangesAsync();

            return result;
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost(nameof(Decide))] 
        public async Task<bool> Decide(DecideViewModel[] elements)
        {
            var userId = UserManager.GetUserId(User);

            var user = await _context.Users
                .Include(x => x.InterestedOrders)
                .Include(x => x.RejectedOrders)
                .Get(userId, _logger);

            var tasks = elements.Select(x => Decide(user, x.OrderId, x.Decision));

            var result = (await Task.WhenAll(tasks)).All(x => x);

            await _context.SaveChangesAsync();

            return result;
        }

        #endregion

        #endregion

        #region Helper

        private async Task<bool> Decide(ApplicationUser user, int orderId, bool decide)
        {
            var order = await _context.Orders
                .Include(x => x.InterestedUsers)
                .Get(orderId, _logger);

            if (order.Status != OrderStatus.CompleteTeam)
            {
                return false;
            }

            order.InterestedUsers.Remove(user);
            user.InterestedOrders.Remove(order);
            user.RejectedOrders.Remove(order);

            if (decide)
            {
                order.InterestedUsers.Add(user);

                user.InterestedOrders.Add(order);
            }
            else
            {
                user.RejectedOrders.Add(order);
            }

            return true;
        }

        

        #endregion
    }
}
