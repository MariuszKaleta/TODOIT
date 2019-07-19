using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity;
using ManyForMany.Models.Entity.Order;
using ManyForMany.Models.Entity.Rate;
using ManyForMany.Models.Entity.User;
using ManyForMany.Models.File;
using ManyForMany.ViewModel.Opinion;
using ManyForMany.ViewModel.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using MvcHelper.Entity;

namespace ManyForMany.Controller.Order
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
            return _context.Orders.ToPublicInformation(orderId, _orderFileManager);
        }

        [Authorize]
        [MvcHelper.Attributes.HttpGet(nameof(LookNew))]
        public async Task<ShowPublicOrderViewModel[]> LookNew([FromQuery] int? start, [FromQuery] int? count, [FromQuery] int[] skills, [FromQuery] int? howMatchSkillsShouldByIncludeInOrder)
        {
            var userId = UserManager.GetUserId(User);

            var orders = _context.Orders
                .Where(x => x.Status == OrderStatus.CompleteTeam
                            && x.RejectedByUsers.All(y => y.Id != userId)
                            && x.InterestedByUsers.All(y => y.Id != userId)
                            && x.Owner.Id != userId
                );

            if (skills != null)
            {
                var findSkills = _context.Skills.Get(x => x.Id, skills);

                orders = orders.Filter(x => x.RequiredSkills, findSkills, howMatchSkillsShouldByIncludeInOrder);
            }

            return
                orders
                .TryTake(start, count)
                .ToPublicInformation(_orderFileManager).ToArray();
        }

        [Authorize]
        [MvcHelper.Attributes.HttpGet(nameof(Watched))]
        public async Task<ShowPublicOrderViewModel[]> Watched([FromQuery] int? start, [FromQuery] int? count, [FromQuery] int[] skills, [FromQuery] int? howMatchSkillsShouldByIncludeInOrder)
        {
            var userId = UserManager.GetUserId(User);

            var orders = _context.Orders.Where(x =>
                x.InterestedByUsers.Any(y => y.Id == userId)
                || x.RejectedByUsers.Any(y => y.Id == userId)
            );

            if (skills != null)
            {
                var findSkills = _context.Skills.Get(x => x.Id, skills);

                orders = orders.Filter(x => x.RequiredSkills, findSkills, howMatchSkillsShouldByIncludeInOrder);
            }

            return
                orders
                .TryTake(start, count)
                .ToPublicInformation(_orderFileManager).ToArray();
        }

        [Authorize]
        [MvcHelper.Attributes.HttpGet(nameof(Interested))]
        public async Task<ShowPublicOrderViewModel[]> Interested([FromQuery] int? start, [FromQuery] int? count, [FromQuery] int[] skills, [FromQuery] int? howMatchSkillsShouldByIncludeInOrder)
        {
            var userId = UserManager.GetUserId(User);

            var orders = _context.Orders
                .Where(x => x.InterestedByUsers.Any(y => y.Id == userId));

            if (skills != null)
            {
                var findSkills = _context.Skills.Get(x => x.Id, skills);

                orders = orders.Filter(x => x.RequiredSkills, findSkills, howMatchSkillsShouldByIncludeInOrder);
            }

            return orders
                .TryTake(start, count)
                .Select(x => x.ToPublicInformation(_orderFileManager)).ToArray();
        }

        [Authorize]
        [MvcHelper.Attributes.HttpGet(nameof(Rejected))]
        public async Task<ShowPublicOrderViewModel[]> Rejected([FromQuery] int? start, [FromQuery] int? count, [FromQuery] int[] skills, [FromQuery] int? howMatchSkillsShouldByIncludeInOrder)
        {
            var userId = UserManager.GetUserId(User);

            var orders = _context.Orders
                .Where(x => x.RejectedByUsers.Any(y => y.Id == userId));

            if (skills != null)
            {
                var findSkills = _context.Skills.Get(x => x.Id, skills);

                orders = orders.Filter(x => x.RequiredSkills, findSkills, howMatchSkillsShouldByIncludeInOrder);
            }

            return orders
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

            var tasks = elements.Select(x => Decide(user, x.ElementId, x.Decision));

            await Task.WhenAll(tasks);

            await _context.SaveChangesAsync();
        }

        #endregion

        #region Opinion

        [AllowAnonymous]
        [MvcHelper.Attributes.HttpGet("{orderId}", nameof(Opinion))]
        public async Task<ShowOpinionViewModel[]> Opinion(string orderId, [FromQuery] int? start, [FromQuery] int? count)
        {
            return _context.Opinions
                .Where(x => x.Order.Id == orderId)
                .TryTake(start, count)
                .Select(x => x.ToShowOpinionViewModel(_context.Orders))
                .ToArray();
        }

        [Authorize]
        [MvcHelper.Attributes.HttpPost("{orderId}", nameof(Opinion))]
        public async Task CreateOpinion(string orderId, CreateOpinionViewModel model)
        {
            var authorTask = UserManager.GetUserAsync(User);

            var order = await _context.Orders.Get(orderId);

            var author = await authorTask;

            if (!order.UsersWhichCanComment.Exists(x => x.Id == author.Id))
            {
                throw new MultiLanguageException(nameof(author), Errors.YouCantCommentThis);
            }

            var opinion = new Opinion(author, order, model);
            order.UsersWhichCanComment.Remove(author);

            _context.Opinions.Add(opinion);
            _context.SaveChanges();
        }

        [Authorize]
        [MvcHelper.Attributes.HttpDelete("{orderId}", nameof(Opinion))]
        public async Task RemoveOpinion(string orderId)
        {
            var author = await UserManager.GetUserAsync(User);

            var opinion = await _context.Opinions
                .Include(x => x.Order)
                .FirstOrDefaultAsync(x => x.Order.Id == orderId && x.Author == author);


            if (opinion == null)
            {
                throw new MultiLanguageException(nameof(author), Errors.YouNotCommentThis);
            }

            opinion.Order.UsersWhichCanComment.Add(author);

            _context.Opinions.Remove(opinion);
            _context.SaveChanges();
        }

        #endregion

        #endregion

        #region Helper

        private async Task Decide(ApplicationUser user, string orderId, bool decide)
        {
            var order = await _context.Orders
                .Include(x => x.InterestedByUsers)
                .Include(x => x.RejectedByUsers)
                .Get(orderId);

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
