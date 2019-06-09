using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UserController : Microsoft.AspNetCore.Mvc.Controller
    {
        public UserController(ILogger<OrderController> logger, Context context, UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            _logger = logger;
            _context = context;
        }

        #region Properties

        public UserManager<ApplicationUser> UserManager { get; }
        private ILogger<OrderController> _logger;
        private readonly Context _context;

        #endregion

        #region Edit

        [Authorize(Roles = CustomRoles.BasicUser)]
        [MvcHelper.Attributes.HttpPost(nameof(Edit))]
        public async Task Edit(UserViewModel userViewMode)
        {
            var user = await UserManager.GetUser(User, _logger);

            user.Edit(userViewMode);

            _context.SaveChanges();
        }

        #endregion

    }
}
