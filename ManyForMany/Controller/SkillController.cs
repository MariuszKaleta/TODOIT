﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity;
using ManyForMany.Models.Entity.Skill;
using ManyForMany.Models.Entity.User;
using ManyForMany.ViewModel.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using MvcHelper;
using MvcHelper.Attributes;
using MvcHelper.Entity;

namespace ManyForMany.Controller
{
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class SkillController : Microsoft.AspNetCore.Mvc.Controller
    {

        public SkillController(ILogger<SkillController> logger, Context context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        #region Properties

        public UserManager<ApplicationUser> _userManager;
        private ILogger<SkillController> _logger;
        private readonly Context _context;

        #endregion

        #region Get

        [MvcHelper.Attributes.HttpGet()]
        public Skill[] Find([FromQuery] string text, [FromQuery] int? start, [FromQuery] int? count)
        {
            return _context.Skills.Filter(x=>x.Name, text, true).TryTake(start, count).ToArray();
        }

        #endregion
    }
}
