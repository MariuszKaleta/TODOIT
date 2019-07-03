using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using AuthorizeTester.Model;
using ManyForMany.Controller.User;
using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Model.File;
using ManyForMany.Models.Configuration;
using ManyForMany.ViewModel;
using ManyForMany.ViewModel.Order;
using ManyForMany.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using MvcHelper.Entity;

namespace ManyForMany.Controller
{
    [AllowAnonymous]
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class TestController : Microsoft.AspNetCore.Mvc.Controller
    {
        public TestController(ILogger<TestController> logger, Context context, UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            _logger = logger;
            _context = context;

            UserId = _context.Users.First().Id;
        }

        #region Properties

        private string UserId;

        public UserManager<ApplicationUser> UserManager { get; }
        private ILogger _logger;
        private readonly Context _context;

        private readonly OrderFileManager _orderFileManager = new OrderFileManager();

        #endregion

        /*
        #region API

        #region Get

        [AllowAnonymous]
        [MvcHelper.Attributes.HttpGet(nameof(AvailableStatus))]
        public Dictionary<int, string> AvailableStatus()
        {
            return Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>()
                .ToDictionary(x => (int)x, x => x.ToString());
        }


        [MvcHelper.Attributes.HttpGet("{orderId}")]
        public ShowPublicOrderViewModel Get(string orderId)
        {
            return _context.Orders.ToPublicInformation(orderId, _logger, _orderFileManager);
        }

        [MvcHelper.Attributes.HttpGet(nameof(LookNew))]
        public async Task<ShowPublicOrderViewModel[]> LookNew([FromQuery] int? start, [FromQuery] int? count)
        {
            var userId = UserId;

            return _context.Orders
                .Where(x => x.Status == OrderStatus.CompleteTeam
                            && x.RejectedByUsers.All(y => y.Id != userId)
                            && x.InterestedByUsers.All(y => y.Id != userId)
                            && x.Owner.Id != UserId
                            )
                .TryTake(start, count)
                .ToPublicInformation(_orderFileManager).ToArray();
        }

        [MvcHelper.Attributes.HttpGet(nameof(Watched))]
        public async Task<ShowPublicOrderViewModel[]> Watched([FromQuery] int? start, [FromQuery] int? count = 5)
        {
            var userId = UserId;

            return _context.Orders.Where
                (x =>
                    x.InterestedByUsers.Any(y => y.Id == userId)
                    || x.RejectedByUsers.Any(y => y.Id == userId)
                )
                .TryTake(start, count)
                .ToPublicInformation(_orderFileManager).ToArray();
        }

        [MvcHelper.Attributes.HttpGet(nameof(Interested))]
        public async Task<ShowPublicOrderViewModel[]> Interested([FromQuery] int? start, [FromQuery] int? count = 5)
        {
            var userId = UserId;

            return _context.Orders.Where
                (
                    x => x.InterestedByUsers.Any(y => y.Id == userId)
                )
                .TryTake(start, count)
                .ToPublicInformation(_orderFileManager).ToArray();
        }

        [MvcHelper.Attributes.HttpGet(nameof(Rejected))]
        public async Task<ShowPublicOrderViewModel[]> Rejected([FromQuery] int? start, [FromQuery] int? count = 5)
        {
            var userId = UserId;

            return _context.Orders
                .Where(x => x.RejectedByUsers.Any(y => y.Id == userId))
                .TryTake(start, count)
                .Select(x => x.ToPublicInformation(_orderFileManager)).ToArray();
        }

        [MvcHelper.Attributes.HttpPost(nameof(Decide), "{orderId}", "{decision}")]
        public async Task<bool> Decide(string orderId, bool decision)
        {
            var userId = UserId;

            var user = await _context.Users.Get(userId, _logger);

            var result = await Decide(user, orderId, decision);

            await _context.SaveChangesAsync();

            return result;
        }

        [MvcHelper.Attributes.HttpPost(nameof(Decide))]
        public async Task<bool> Decide(DecideViewModel[] elements)
        {
            var userId = UserId;

            var user = await _context.Users.Get(userId, _logger);

            var tasks = elements.Select(x => Decide(user, x.OrderId, x.Decision));

            var result = (await Task.WhenAll(tasks)).All(x => x);

            await _context.SaveChangesAsync();

            return result;
        }

        #endregion

        #endregion

        #region Helper

        private async Task<bool> Decide(ApplicationUser user, string orderId, bool decide)
        {
            var order = await _context.Orders
                .Include(x => x.InterestedByUsers)
                .Include(x => x.RejectedByUsers)
                .Get(orderId, _logger);

            if (order.Status != OrderStatus.CompleteTeam)
            {
                return false;
            }



            if (decide)
            {
                order.InterestedByUsers.Add(user);
                order.RejectedByUsers.Remove(user);



                //  user.InterestedOrders.Add(order);
            }
            else
            {
                order.RejectedByUsers.Add(user);
                order.InterestedByUsers.Remove(user);
            }

            return true;
        }



        #endregion

        
        */
    }
}
