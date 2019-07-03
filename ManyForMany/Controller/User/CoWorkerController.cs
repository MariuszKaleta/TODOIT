using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using ManyForMany.Controller.Chat;
using ManyForMany.ViewModel.Team;
using ManyForMany.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MvcHelper.Attributes;

namespace ManyForMany.Controller.User
{
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class CoWorkerController
    {
        public CoWorkerController(ILogger<CoWorkerController> logger, Context context, UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            _logger = logger;
            _context = context;
        }

        #region Properties

        public UserManager<ApplicationUser> UserManager { get; }
        private ILogger _logger;
        private readonly Context _context;

        #endregion



        #region Get

        [AllowAnonymous]
        [MvcHelper.Attributes.HttpGet("{userId}")]
        public async Task<UserInformationViewModel> Get(string userId)
        {
            var user = await _context.Users
                .Include(x => x.OpinionsAboutMe)
                .Include(x => x.Skills)
                .Get(userId, _logger);

            return user.ToUserInformation();
        }

        #endregion

        [Authorize]
        [MvcHelper.Attributes.HttpPost("{userId}", nameof(Opinion))]
        public async Task Opinion(string userId, OpinionViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
