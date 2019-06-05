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
        private ImageManager ImageManager = new ImageManager();
        #endregion

        #region API

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPut(nameof(Create))]
        public async Task Create(OrderViewModel model)
        {
            var userTask = UserManager.GetUserAsync(User);

            var order = new Order(model);

            var user = await userTask;

            if (null == user)
            {
                throw new MultiLanguageException(nameof(User), Error.ElementDoseNotExist);
            }

            user.UserOrders.Add(order);

            _context.SaveChanges();

            ImageManager.UploadFile(model.Images, user.Id, order.Id);

            _logger.LogInformation(nameof(ImageManager.UploadFile), model.Images);
        }

        #region Edit

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPut("{id}", nameof(Edit), nameof(Order.Title), "{title}")]
        public async Task EditTitle(int id, string title)
        {
            var user = await UserManager.GetUserAsync(User);

            if (null == user)
            {
                _logger.LogError(nameof(User), Error.ElementDoseNotExist.ToString(), id);
                throw new MultiLanguageException(nameof(User), Error.ElementDoseNotExist);
            }

            var order =  user.UserOrders.FirstOrDefault(x => x.Id == id);

            if (order == null)
            {
                _logger.LogError(nameof(Order.Id), Error.ElementDoseNotExist.ToString(), id);
                throw new MultiLanguageException(nameof(order), Error.ElementDoseNotExist);
            }

            order.Title = title;

            _context.SaveChanges();
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPut("{id}", nameof(Edit), nameof(Order.Describe), "{describe}")]
        public async Task Edit(int id, string describe)
        {
            var user = await UserManager.GetUserAsync(User);

            if (null == user)
            {
                _logger.LogError(nameof(User), Error.ElementDoseNotExist.ToString(), User);
                throw new MultiLanguageException(nameof(User), Error.ElementDoseNotExist);
            }

            var order =  user.UserOrders.FirstOrDefault(x => x.Id == id);

            if (order == null)
            {
                _logger.LogError(nameof(Order.Id), Error.ElementDoseNotExist.ToString(), id);
                throw new MultiLanguageException(nameof(order), Error.ElementDoseNotExist);
            }

            order.Describe = describe;

            _context.SaveChanges();
        }

        #endregion

        #region Get

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet("{id}")]
        public async Task<Order> Get(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                _logger.LogWarning(nameof(Order.Id), Error.ElementDoseNotExist.ToString(), id);
            }

            return order;
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet(nameof(LookNew), "{count}")]
        public async Task<Order[]> LookNew(int count = 1)
        {
            var user = await UserManager.GetUserAsync(User);

            var orders = _context.Orders.Except(user.DecidedOrders.Select(x => x.Order)).Except(user.UserOrders);

            var ordersCount = orders.Count();

            return orders.Take(count < ordersCount ? count : ordersCount).ToArray();
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet(nameof(LookWatched), "{count}")]
        public async Task<Decizion[]> LookWatched(int count = 1)
        {
            var user = await UserManager.GetUserAsync(User);

            var ordersCount = user.DecidedOrders.Count();

            return user.DecidedOrders.Take(count < ordersCount ? count : ordersCount).ToArray();
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost(nameof(Decide), "{orderId}")] 
        public async Task Decide(bool decision, int orderId)
        {
            var userTask = UserManager.GetUserAsync(User);

            var orderTask = _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);

            var user = await userTask;
            var order = await orderTask;

            if (user == null)
            {
                _logger.LogError(nameof(User), Error.ElementDoseNotExist.ToString(), User);
                throw new MultiLanguageException(nameof(User), Error.ElementDoseNotExist);
            }

            if (order == null)
            {
                _logger.LogError(nameof(Order.Id), Error.ElementDoseNotExist.ToString(), orderId);
                throw new MultiLanguageException(nameof(order), Error.ElementDoseNotExist);
            }

            var decidedOrder = new Decizion(order, decision);

            user.DecidedOrders.Add(decidedOrder);
            _context.SaveChangesAsync();
        }

        #endregion

        #endregion
    }
}
