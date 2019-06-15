using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizeTester.Model;
using AuthorizeTester.Model.Error;
using ManyForMany.Model.Entity;
using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Model.Entity.User;
using ManyForMany.Model.Extension;
using ManyForMany.Model.File;
using ManyForMany.ViewModel.Order;
using ManyForMany.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;

namespace ManyForMany.Controller.User
{
    [ApiController]
    [Authorize]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class UserOrderController : Microsoft.AspNetCore.Mvc.Controller
    {
        public UserOrderController(ILogger<OrderController> logger, Context context,
            UserManager<ApplicationUser> userManager)
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

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost(nameof(Create))]
        public async Task Create(OrderViewModel model)
        {
            var user = await UserManager.GetUser(User, _logger);

            var order = new Order(model, user);

            user.OwnOrders.Add(order);

            _context.SaveChanges();

            await ImageManager.UploadFile(model.Images, user.Id, order.Id);

            _logger.LogInformation(nameof(ImageManager.UploadFile), model.Images);
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost("{orderId}", nameof(ChangeStatus), "{status}")]
        public async Task ChangeStatus(int orderId, OrderStatus status)
        {
            var user = await UserManager.GetUser(User, _logger);

            var order = user.OwnOrders.GetOrder(orderId, _logger);

            order.Status = status;

            _context.SaveChanges();
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpDelete("{id}", nameof(Remove))]
        public async Task Remove(int id)
        {
            var user = await UserManager.GetUser(User, _logger);

            var order = user.OwnOrders.GetOrder(id, _logger);

            foreach (var interestedUser in order.InterestedUsers)
            {
                interestedUser.InterestedOrders.Remove(order);
            }

            user.OwnOrders.Remove(order);

            _context.SaveChanges();

            await ImageManager.RemoveFiles(user.Id, order.Id);
        }

        #region Edit

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost(nameof(Edit), "{id}")]
        public async Task Edit(int id, OrderViewModel model)
        {
            var user = await UserManager.GetUser(User, _logger);

            var order = user.OwnOrders.GetOrder(id, _logger);

            order.Edit(model);

            _context.SaveChanges();
        }

        #endregion

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet(nameof(All), "{start}", "{count}")]
        public async Task<OrderViewModel[]> All(int? start = null, int? count = null)
        {
            var user = await UserManager.GetUser(User, _logger);

            return (count == null || start == null ? user.OwnOrders : user.OwnOrders.TryTake(start.Value, count.Value))
                .Select(x => x.ToViewModel(ImageManager)).ToArray();
        }


        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet("{id}", nameof(InterstedUser), "{start}", "{count}")]
        public async Task<UserViewModel[]> InterstedUser(int id, int start, int count)
        {
            var user = await UserManager.GetUser(User, _logger);

            var order = user.OwnOrders.GetOrder(id, _logger);

            return order.InterestedUsers.TryTake(start, count).Select(x => x.ToUserInformation()).ToArray();
        }
    }
}
