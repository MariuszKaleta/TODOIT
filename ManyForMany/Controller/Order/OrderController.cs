﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using AuthorizeTester.Model;
using ManyForMany.Model.Entity;
using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Model.File;
using ManyForMany.Models.Configuration;
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
        private ILogger _logger;
        private readonly Context _context;
        private OrderFileManager _orderFileManager = new OrderFileManager();

        #endregion

        #region API

        #region Get

        [AllowAnonymous]
        [MvcHelper.Attributes.HttpGet(nameof(AvailableStatus))]
        public Dictionary<int, string> AvailableStatus()
        {
            return Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>()
                .ToDictionary(x => (int)x, x => x.ToString());
        }


        [Authorize]
        [MvcHelper.Attributes.HttpGet("{orderId}")]
        public ShowPublicOrderViewModel Get(string orderId)
        {
            return _context.Orders.ToPublicInformation(orderId, _logger, _orderFileManager);
        }

        [Authorize]
        [MvcHelper.Attributes.HttpGet(nameof(LookNew))]
        public async Task<ShowPublicOrderViewModel[]> LookNew([FromQuery] int? start, [FromQuery] int? count)
        {
            var userId = UserManager.GetUserId(User);

            return _context.Orders
                .Where(x => x.Status == OrderStatus.CompleteTeam
                            && x.RejectedByUsers.All(y => y.Id != userId)
                            && x.InterestedByUsers.All(y => y.Id != userId)
                            && x.Owner.Id != userId
                            )
                .TryTake(start, count)
                .ToPublicInformation(_orderFileManager).ToArray();
        }

        [Authorize]
        [MvcHelper.Attributes.HttpGet(nameof(Watched))]
        public async Task<ShowPublicOrderViewModel[]> Watched([FromQuery] int? start, [FromQuery] int? count )
        {
            var userId = UserManager.GetUserId(User);

            return _context.Orders.Where(x =>
                    x.InterestedByUsers.Any(y => y.Id == userId)
                    || x.RejectedByUsers.Any(y => y.Id == userId)
                )
                .TryTake(start, count)
                .ToPublicInformation(_orderFileManager).ToArray();
        }

        [Authorize]
        [MvcHelper.Attributes.HttpGet(nameof(Interested))]
        public async Task<ShowPublicOrderViewModel[]> Interested([FromQuery] int? start, [FromQuery] int? count )
        {
            var userId = UserManager.GetUserId(User);

            return _context.Orders
                .Where(x => x.InterestedByUsers.Any(y => y.Id == userId))
                .TryTake(start, count)
                .Select(x => x.ToPublicInformation(_orderFileManager)).ToArray();
        }

        [Authorize]
        [MvcHelper.Attributes.HttpGet(nameof(Rejected))]
        public async Task<ShowPublicOrderViewModel[]> Rejected([FromQuery] int? start, [FromQuery] int? count )
        {
            var userId = UserManager.GetUserId(User);

            return _context.Orders
                .Where(x => x.RejectedByUsers.Any(y => y.Id == userId))
                .TryTake(start, count)
                .Select(x => x.ToPublicInformation(_orderFileManager)).ToArray();
        }

        [Authorize]
        [MvcHelper.Attributes.HttpPost(nameof(Decide), "{orderId}", "{decision}")]
        public async Task Decide(string orderId, bool decision)
        {
            var user = await UserManager.GetUserAsync(User);

            await Decide(user, orderId, decision);

            await _context.SaveChangesAsync();
        }

        [Authorize]
        [MvcHelper.Attributes.HttpPost(nameof(Decide))]
        public async Task Decide(DecideViewModel[] elements)
        {
            var user = await UserManager.GetUserAsync(User);

            var tasks = elements.Select(x => Decide(user, x.OrderId, x.Decision));

             await Task.WhenAll(tasks);

            await _context.SaveChangesAsync();
        }

        #endregion

        #endregion

        #region Helper

        private async Task Decide(ApplicationUser user, string orderId, bool decide)
        {
            var order = await _context.Orders
                .Include(x => x.InterestedByUsers)
                .Include(x => x.RejectedByUsers)
                .Get(orderId, _logger);

            if (order.Status != OrderStatus.CompleteTeam)
            {
                throw new MultiLanguageException(nameof(order.Status), Errors.OwnerOfOrderDontLookingForTeam);
            }

            if (order.Owner == user)
            {
                throw new MultiLanguageException(nameof(order.Owner), Errors.YouCantJoinToYourOrderTeam);
            }


            if (decide)
            {
                order.InterestedByUsers.Add(user);
                order.RejectedByUsers.Remove(user);
            }
            else
            {
                order.RejectedByUsers.Add(user);
                order.InterestedByUsers.Remove(user);
            }
        }



        #endregion
    }
}
