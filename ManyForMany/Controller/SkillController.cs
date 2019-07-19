﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity;
using ManyForMany.Models.Entity.Skill;
using ManyForMany.Models.Entity.User;
using ManyForMany.ViewModel.Order;
using ManyForMany.ViewModel.Skill;
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
        public SkillThumbnailViewModel[] Find([FromQuery] string text, [FromQuery] int? start, [FromQuery] int? count)
        {
            return _context.Skills.Filter(text, x => x.Name).TryTake(start, count).Select(x => x.ToThumbnail())
                .ToArray();
        }

        [MvcHelper.Attributes.HttpGet("{id}")]
        public async Task<PublicSkillViewModel> Get(int id)
        {
            return (await _context.Skills.Get(id)).ToViewModel();
        }



        #endregion
    }
}
