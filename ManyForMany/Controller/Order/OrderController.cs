using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using MvcHelper.Entity;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.Rate;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories.Contracts;
using TODOIT.ViewModel.Opinion;
using TODOIT.ViewModel.Order;

namespace TODOIT.Controller.Order
{
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class OrderController : Microsoft.AspNetCore.Mvc.Controller
    {

        public OrderController(UserManager<ApplicationUser> userManager, IOrderRepository orderRepository, IOpinionRepository opinionRepository)
        {
            UserManager = userManager;
            _orderRepository = orderRepository;
            _opinionRepository = opinionRepository;
        }

        #region Properties

        private readonly UserManager<ApplicationUser> UserManager;
        private readonly IOrderRepository _orderRepository;
        private readonly IOpinionRepository _opinionRepository;

        #endregion

        #region API

        /// <summary>
        /// Make decision
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="decision"></param>
        /// <returns></returns>
        //[Authorize]
        [Authorize(AuthenticationSchemes = CustomGrantTypes.Google)]
        [MvcHelper.Attributes.HttpPost(nameof(AddToInterested), "{orderId}")]
        public async Task AddToInterested(Guid orderId)
        {
            var userAsync = UserManager.GetUserAsync(User);
            var orderAsync = _orderRepository.Get(orderId);

            await _orderRepository.AddToInterested(await userAsync, await orderAsync);
        }

        /// <summary>
        /// Maky many decision
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        //[Authorize]
        [Authorize(AuthenticationSchemes = CustomGrantTypes.Google)]
        [MvcHelper.Attributes.HttpPost(nameof(AddToInterested))]
        public async Task AddToInterested(Guid[] ids)
        {
            var userAsync = UserManager.GetUserAsync(User);

            var ordersAsync = _orderRepository.Get(ids);

            await _orderRepository.AddToInterested(await userAsync, await ordersAsync);
        }

        /// <summary>
        /// Create new Order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [MvcHelper.Attributes.HttpPost(nameof(Create))]
        [Authorize(AuthenticationSchemes = CustomGrantTypes.Google)]
        public async Task Create(CreateOrderViewModel model)
        {
            var userAsync = UserManager.GetUserAsync(User);

            await _orderRepository.Create(model, await userAsync);
        }

        /// <summary>
        /// edit existed Order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = CustomGrantTypes.Google)]
        [MvcHelper.Attributes.HttpPost("orderId", nameof(Update))]
        public async Task Update(Guid orderId, CreateOrderViewModel model)
        {
            var userAsync = UserManager.GetUserAsync(User);

            var orderAsync = _orderRepository.Get(orderId);

            var order = await orderAsync;
            var user = await userAsync;

            if (order.Owner.Id != user.Id)
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }

            await _orderRepository.Update(model, order);
        }

        [Authorize(AuthenticationSchemes = CustomGrantTypes.Google)]
        [MvcHelper.Attributes.HttpPost("orderId", nameof(Remove))]
        public async Task Remove(Guid orderId)
        {
            var userAsync = UserManager.GetUserAsync(User);

            var orderAsync = _orderRepository.Get(orderId);

            var order = await orderAsync;
            var user = await userAsync;

            if (order.Owner.Id != user.Id)
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }

            _orderRepository.Delete(await orderAsync, true, await userAsync);
        }

        /// <summary>
        /// Create new Order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [MvcHelper.Attributes.HttpPost("orderId", nameof(Opinion) , nameof(Create))]
        [Authorize(AuthenticationSchemes = CustomGrantTypes.Google)]
        public async Task Create(Guid orderId, OpinionViewModel model)
        {
            var userAsync = UserManager.GetUserAsync(User);

            var orderAsync = _orderRepository.Get(orderId);

            await _opinionRepository.Create(model, await userAsync, await orderAsync);
        }

        #endregion
    }
}
