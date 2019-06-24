using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using ManyForMany.Models.Entity.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcHelper;
using MvcHelper.Attributes;

namespace ManyForMany.Controller
{
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class SkillController
    {

        public SkillController(ILogger<SkillController> logger, Context context, UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            _logger = logger;
            _context = context;
        }

        #region Properties

        public UserManager<ApplicationUser> UserManager { get; }
        private ILogger<SkillController> _logger;
        private readonly Context _context;

        #endregion

        #region Get

        [MvcHelper.Attributes.HttpGet()]
        public Skill[] Find([FromQuery] string text, [FromQuery] int? start, [FromQuery] int? count, [FromQuery] bool ifTextNullGetAll = false)
        {
            return _context.Skills.Filter(text, ifTextNullGetAll).TryTake(start, count).ToArray();
        }

        #endregion
        
    }
}
