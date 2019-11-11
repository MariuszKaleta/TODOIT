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

        public OrderController(UserManager<ApplicationUser> userManager, IOrderRepository orderRepository, IOpinionRepository opinionRepository, IOrderMembersRepository orderMembersRepository, IChatRepository chatRepository)
        {
            UserManager = userManager;
            _orderRepository = orderRepository;
            _opinionRepository = opinionRepository;
            _orderMembersRepository = orderMembersRepository;
            _chatRepository = chatRepository;
        }

        #region Properties

        private readonly UserManager<ApplicationUser> UserManager;
        private readonly IOrderRepository _orderRepository;
        private readonly IOpinionRepository _opinionRepository;
        private readonly IOrderMembersRepository _orderMembersRepository;
        private readonly IChatRepository _chatRepository;

        #endregion

        #region API

        /// <summary>
        /// Make decision
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="decision"></param>
        /// <returns></returns>
        //[Authorize]
        [Authorize()]
        [MvcHelper.Attributes.HttpPost(nameof(AddToInterested), "{orderId}")]
        public async Task AddToInterested(Guid orderId)
        {
            var user = UserManager.GetUserId(User);

            await _orderRepository.AddToInterested( user, orderId);
        }

        /// <summary>
        /// Maky many decision
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        //[Authorize]
        [Authorize()]
        [MvcHelper.Attributes.HttpPost(nameof(AddToInterested))]
        public async Task AddToInterested(Guid[] ids)
        {
            var user = UserManager.GetUserId(User);
            
            await _orderRepository.AddToInterested(user, ids);
        }

        /// <summary>
        /// Create new Order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [MvcHelper.Attributes.HttpPost(nameof(Create))]
        [Authorize()]
        public async Task Create(CreateOrderViewModel model)
        {
            var user = UserManager.GetUserId(User);
            
            var order = await _orderRepository.Create(model,  user);

            await _chatRepository.Create(order.Id);
        }

        /// <summary>
        /// edit existed Order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize()]
        [MvcHelper.Attributes.HttpPost("orderId", nameof(Update))]
        public async Task Update(Guid orderId, CreateOrderViewModel model)
        {
            var userAsync = UserManager.GetUserId(User);

            if (!await _orderRepository.IAmOwner(orderId, userAsync))
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }

            await _orderRepository.Update(model, orderId);
        }

        [Authorize()]
        [MvcHelper.Attributes.HttpPost("orderId", nameof(Remove))]
        public async Task Remove(Guid orderId)
        {
            var userId = UserManager.GetUserId(User);

            if (!await _orderRepository.IAmOwner(orderId, userId))
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }

            _orderRepository.Delete(orderId, true);
        }

        /// <summary>
        /// Create new Order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [MvcHelper.Attributes.HttpPost("{orderId}", nameof(Opinion), nameof(Create))]
        [Authorize()]
        public async Task Create([FromRoute] Guid orderId, OpinionViewModel model)
        {
            var orderAsync = _orderRepository.Get(orderId, x => x.Owner);

            var userId = UserManager.GetUserId(User);

            var order = await orderAsync;
            
            await _opinionRepository.Create(model, userId, order);
        }

        /// <summary>
        /// Invite User to Order Task
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [MvcHelper.Attributes.HttpPost("{orderId}", nameof(User), nameof(Invite))]
        [Authorize()]
        public async Task Invite([FromRoute] Guid orderId, string[] usersId)
        {
            var yourUserId = UserManager.GetUserId(User);

            if (! await _orderRepository.IAmOwner(orderId, yourUserId))
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }

            var chat = await _chatRepository.GetByOrderId(orderId);

           await  _chatRepository.AddUserToChat(chat.Id, false, usersId);

            await _orderMembersRepository.InviteUserToMakeOrder(usersId, orderId);
        }

        [MvcHelper.Attributes.HttpPost("{orderId}", nameof(User), nameof(KickOff))]
        [Authorize()]
        public async Task KickOff([FromRoute] Guid orderId, string[] usersId)
        {
            var yourUserId = UserManager.GetUserId(User);

            if (!await _orderRepository.IAmOwner(orderId, yourUserId))
            {
                throw new Exception(Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }

            var chat = await _chatRepository.GetByOrderId(orderId);

            await _chatRepository.AddUserToChat(chat.Id, false, usersId);

            await _orderMembersRepository.InviteUserToMakeOrder(usersId, orderId);
        }

        #endregion
    }
}
