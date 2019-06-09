using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizeTester.Model;
using AuthorizeTester.Model.Error;
using ManyForMany.Model.Entity;
using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Model.Entity.Orders;
using ManyForMany.Model.Entity.User;
using ManyForMany.Model.Extension;
using ManyForMany.Model.File;
using ManyForMany.ViewModel.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using MvcHelper.Attributes;

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
        public async Task<Dictionary<string,int>> AvailableStatus()
        {
            return Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>()
                .ToDictionary(x => x.ToString(), x => (int) x);
        }


        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet("{id}")]
        public async Task<OrderViewModel> Get(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                _logger.LogWarning(nameof(Order.Id), Error.ElementDoseNotExist.ToString(), id);
            }

            return order.ToViewModel(imageManager);
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet(nameof(LookNew), "{count}")]
        public async Task<OrderViewModel[]> LookNew(int count = 1)
        {
            var user = await UserManager.GetUserAsync(User);

            var orders = _context.Orders.Except(user.WatchedAndUserOrders());

            return orders.Where(x => x.Status == OrderStatus.LookingForOfert).TryTake(count)
                .Select(x => x.ToViewModel(imageManager)).ToArray();
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet(nameof(Watched), "{count}")]
        public async Task<OrderViewModel[]> Watched(int count = 1)
        {
            var user = await UserManager.GetUserAsync(User);

            return user.WatchedOrders().TryTake(count).Select(x => x.ToViewModel(imageManager)).ToArray();
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet(nameof(Interested), "{count}")]
        public async Task<OrderViewModel[]> Interested(int count = 1)
        {
            var user = await UserManager.GetUserAsync(User);

            return user.InterestedOrders.TryTake(count).Select(x => x.ToViewModel(imageManager)).ToArray();
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet(nameof(Rejected), "{count}")]
        public async Task<OrderViewModel[]> Rejected(int count = 1)
        {
            var user = await UserManager.GetUserAsync(User);

            return user.RejectedOrders.TryTake(count).Select(x => x.ToViewModel(imageManager)).ToArray();
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost(nameof(Decide), "{orderId}", "{decision}")] 
        public async Task<bool> Decide(int orderId, bool decision)
        {
            var user = await UserManager.GetUserAsync(User);
            
            if (user == null)
            {
                _logger.LogError(nameof(User), Error.ElementDoseNotExist.ToString(), User);
                throw new MultiLanguageException(nameof(User), Error.ElementDoseNotExist);
            }

            var result =  await Decide(user,orderId, decision);

            await _context.SaveChangesAsync();

            return result;
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost(nameof(Decide))] 
        public async Task<bool> Decide(Decide[] elements)
        {
            var user = await UserManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogError(nameof(User), Error.ElementDoseNotExist.ToString(), User);
                throw new MultiLanguageException(nameof(User), Error.ElementDoseNotExist);
            }

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
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
            {
                _logger.LogError(nameof(Order.Id), Error.ElementDoseNotExist.ToString(), orderId);
                throw new MultiLanguageException(nameof(order), Error.ElementDoseNotExist);
            }

            if (order.Status != OrderStatus.LookingForOfert)
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
