using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity;
using ManyForMany.Models.Entity.Order;
using ManyForMany.Models.Entity.User;
using ManyForMany.Models.File;
using ManyForMany.ViewModel;
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
    [Authorize]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class UserOrderController : Microsoft.AspNetCore.Mvc.Controller
    {
        public UserOrderController(ILogger<UserOrderController> logger, Context context, UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            _logger = logger;
            _context = context;
        }

        #region Properties

        public UserManager<ApplicationUser> UserManager { get; }
        private ILogger _logger;
        private readonly Context _context;
        private readonly OrderFileManager _orderFileManager = new OrderFileManager();

        #endregion

        /// <summary>
        /// Create Order using creteorderViewModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost()]
        public async Task Create(CreateOrderViewModel model)
        {
            var user = await UserManager.GetUserAsync(((ControllerBase) this).User);

            var order = new Models.Entity.Order.Order(model, user, _context.Skills);

            _context.Orders.Add(order);

            _context.SaveChanges();

            if (model.Images != null)
            {
                await _orderFileManager.UploadOrderImages(model.Images, user.Id, order.Id);

               // _logger.LogInformation(nameof(_orderFileManager.UploadOrderImages), model.Images);
            }

            if (model.Files != null)
            {
                await _orderFileManager.UploadOrderFiles(model.Images, user.Id, order.Id);

                //_logger.LogInformation(nameof(_orderFileManager.UploadOrderImages), model.Images);
            }
        }

        #region Edit

        /// <summary>
        /// Edit Order 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPut("{orderId}")]
        public async Task Edit(string orderId, EditOrderViewModel model)
        {
            var userId = UserManager.GetUserId(((ControllerBase) this).User);

            var order = await _context.Orders.Where(x => x.Owner.Id == userId).Get(orderId);

            order.Edit(model);

            _context.SaveChanges();
        }

        #endregion

        #region Photo


        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost("{orderId}", nameof(Photo))]
        public async Task AddPhoto(string orderId, FileViewModel[] images)
        {
            var userId = UserManager.GetUserId(((ControllerBase) this).User);

            var orderBelongToUser = _context.Orders.Any(x => x.Owner.Id == userId && x.Id == orderId);

            if (orderBelongToUser)
            {
                await _orderFileManager.UploadOrderImages(images, userId, orderId);

                //_logger.LogInformation(nameof(_orderFileManager.UploadOrderImages), images);
            }
            else
            {
                throw new MultiLanguageException(nameof(orderId), Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpDelete("{orderId}", nameof(Photo))]
        public async Task RemovePhoto(string orderId, string[] imagesId)
        {
            var userId = UserManager.GetUserId(((ControllerBase) this).User);

            var orderBelongToUser = _context.Orders.Any(x => x.Owner.Id == userId && x.Id == orderId);

            if (orderBelongToUser)
            {
                _orderFileManager.RemoveOrderImages(userId, orderId, imagesId);

                //_logger.LogInformation(nameof(_orderFileManager.UploadOrderImages), imagesId);
            }
            else
            {
                throw new MultiLanguageException(nameof(orderId), Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet("{orderId}", nameof(Photo))]
        public async Task<File[]> Photo(string orderId)
        {
            var userId = UserManager.GetUserId(((ControllerBase) this).User);

            var orderBelongToUser = _context.Orders.Any(x => x.Owner.Id == userId && x.Id == orderId);

            if (orderBelongToUser)
            {
                return await _orderFileManager.DownladOrderImages(userId, orderId);

            }
            else
            {
                throw new MultiLanguageException(nameof(orderId), Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }
        }

        #endregion

        #region Files


        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost("{orderId}", nameof(File))]
        public async Task AddFile(string orderId, FileViewModel[] files)
        {
            var userId = UserManager.GetUserId(((ControllerBase) this).User);

            var orderBelongToUser = _context.Orders.Any(x => x.Owner.Id == userId && x.Id == orderId);

            if (orderBelongToUser)
            {
                await _orderFileManager.UploadOrderFiles(files, userId, orderId);

                //_logger.LogInformation(nameof(_orderFileManager.UploadOrderImages), images);
            }
            else
            {
                throw new MultiLanguageException(nameof(orderId), Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpDelete("{orderId}", nameof(File))]
        public async Task RemoveFile(string orderId, string[] imagesId)
        {
            var userId = UserManager.GetUserId(((ControllerBase) this).User);

            var orderBelongToUser = _context.Orders.Any(x => x.Owner.Id == userId && x.Id == orderId);

            if (orderBelongToUser)
            {
                _orderFileManager.RemoveOrderFiles(userId, orderId, imagesId);

                //_logger.LogInformation(nameof(_orderFileManager.UploadOrderImages), imagesId);
            }
            else
            {
                throw new MultiLanguageException(nameof(orderId), Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet("{orderId}", nameof(File))]
        public async Task<File[]> File(string orderId)
        {
            var userId = UserManager.GetUserId(((ControllerBase) this).User);

            var orderBelongToUser = _context.Orders.Any(x => x.Owner.Id == userId && x.Id == orderId);

            if (orderBelongToUser)
            {
                return await _orderFileManager.DownladOrderFiles(userId, orderId);

            }
            else
            {
                throw new MultiLanguageException(nameof(orderId), Errors.OrderDoseNotExistOrIsNotBelongToYou);
            }
        }

        #endregion

        /// <summary>
        /// Get All my Orders
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet()]
        public ShowPublicOrderViewModel[] All([FromQuery] int? start, [FromQuery] int? count)
        {
            var userId = UserManager.GetUserId(((ControllerBase) this).User);

            return _context.Orders
                .Include(x => x.Owner)
                .Include(x => x.RequiredSkills)
                .Include(x => x.GoodIfHave)
                .Where(x => x.Owner.Id == userId)
                .TryTake(start, count)
                .Select(x => x.ToPublicInformation(_orderFileManager)).ToArray();
        }

        [AllowAnonymous]
        [MvcHelper.Attributes.HttpGet("{userId}", nameof(Models.Entity.Order.Order))]
        public async Task<ShowPublicOrderViewModel[]> Orders(string userId, [FromQuery] int? start, [FromQuery] int? count)
        {
           return _context.Orders.Where(x => x.Owner.Id == userId)
               .TryTake(start, count)
               .Select(x => x.ToPublicInformation(_orderFileManager)).ToArray();
        }

        #region Users

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet("{orderId}", nameof(InterstedUsers))]
        public async Task<ThumbnailUserViewModel[]> InterstedUsers(string orderId, [FromQuery] int? start, [FromQuery] int? count)
        {
            //TODO każdy morze obejżeć kto ogląda w wersji premium
            var userId = UserManager.GetUserId(((ControllerBase) this).User);

            var result = _context.Orders.Where(x => x.Id == orderId && x.Owner.Id == userId)
                .SelectMany(x => x.InterestedByUsers)
                .TryTake(start, count)
                .Select(x => x.ToUserThumbnail()).ToArray();

            if (!result.Any())
            {
                var order = _context.Orders.Any(x => x.Id == orderId && x.Owner.Id == userId);

                if (!order)
                {
                    throw new MultiLanguageException(nameof(orderId), Errors.OrderDoseNotExistOrIsNotBelongToYou);
                }
            }

            return result;
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpGet("{orderId}", nameof(Team))]
        public async Task<ThumbnailUserViewModel[]> Team(string orderId)
        {
            var userId = UserManager.GetUserId(((ControllerBase)this).User);

            var order = await _context.Orders
                .Include(x => x.ActualTeam)
                .Where(x => x.Owner.Id == userId)
                .Get(orderId);

            return order.ActualTeam.Select(x=>x.ToUserThumbnail()).ToArray();
        }

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost("{orderId}", nameof(Team), "{addUserId}")]
        public async Task User(string orderId, string addUserId)
        {
            var userToAddTask = _context.Users.Get(addUserId);

            var userId = UserManager.GetUserId(((ControllerBase)this).User);

            var order = await _context.Orders
                .Include(x=>x.ActualTeam)
                .Include(x=>x.InterestedByUsers)
                .Where(x => x.Owner.Id == userId)
                .Get(orderId);


            if (order.ActualTeam.Exists(x => x.Id == addUserId))
            {
                throw new MultiLanguageException(nameof(userId), Errors.UserIsAlredyAdded);
            }

            if (!order.InterestedByUsers.Exists(x => x.Id == addUserId))
            {
                throw new MultiLanguageException(nameof(userId), Errors.UserIsNotInterestedOrder);
            }

            var userToAdd = await userToAddTask;

            order.InterestedByUsers.Remove(userToAdd);

            order.ActualTeam.Add(userToAdd);
            order.UsersWhichCanComment.Add(userToAdd);

            _context.SaveChanges();
        }
        

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpDelete("{orderId}", nameof(Team), "{addUserId}")]
        public async Task RemoveUser(string orderId, string addUserId)
        {
            var userToAddTask = _context.Users.Get(addUserId);

            var userId = UserManager.GetUserId(((ControllerBase)this).User);

            var order = await _context.Orders
                .Include(x => x.ActualTeam)
                .Include(x => x.InterestedByUsers)
                .Where(x => x.Owner.Id == userId)
                .Get(orderId);


            if (!order.ActualTeam.Exists(x => x.Id == addUserId))
            {
                throw new MultiLanguageException(nameof(userId), Errors.UserIsNotAdded);
            }

            if (order.InterestedByUsers.Exists(x => x.Id == addUserId))
            {
                throw new MultiLanguageException(nameof(userId), Errors.UserIsAlredyAdded);
            }

            var userToAdd = await userToAddTask;

            order.InterestedByUsers.Add(userToAdd);

            order.ActualTeam.Remove(userToAdd);

            _context.SaveChanges();
        }

        #endregion
    }
}
