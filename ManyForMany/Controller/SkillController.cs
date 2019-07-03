using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity.Order;
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
        public Skill[] Find([FromQuery] string text, [FromQuery] int? start, [FromQuery] int? count, [FromQuery] bool ifTextNullGetAll = false)
        {
            return _context.Skills.Filter(x=>x.Name, text, ifTextNullGetAll).TryTake(start, count).ToArray();
        }

        [Authorize]
        [MvcHelper.Attributes.HttpGet("{skillId}",nameof(UsedInProjects))]
        public ThumbnailOrderViewModel[] UsedInProjects(int skillId)
        {
            throw new NotImplementedException();
        }

        #endregion

        [Authorize]
        [MvcHelper.Attributes.HttpPost(nameof(User))]
        public async void Add(int[] skillsId)
        {
            var id = _userManager.GetUserId( User);

            var userTask = _context.Users.Include(x => x.Skills).FirstOrDefaultAsync(x => x.Id == id);

            var skills = _context.Skills.Get(skillsId);

            var user = await userTask;

            foreach (var skill in skills)
            {
                if (user.Skills.Contains(skill))
                {
                    throw new MultiLanguageException(nameof(Skill.Id), Errors.SkillIsAlreadyExist);
                }

                user.Skills.Add(skill);
            }

            _context.SaveChanges();
        }

        [Authorize]
        [MvcHelper.Attributes.HttpDelete(nameof(User))]
        public async void Remove(int[] skillsId)
        {
            var id = _userManager.GetUserId(User);

            var userTask = _context.Users.Include(x => x.Skills).FirstOrDefaultAsync(x => x.Id == id);

            var skills = _context.Skills.Get(skillsId);

            var user = await userTask;

            foreach (var skill in skills)
            {
                if (!user.Skills.Contains(skill))
                {
                    throw new MultiLanguageException(nameof(Skill.Id), Errors.SkillIsAlreadyExist);
                }

                user.Skills.Remove(skill);
            }

            _context.SaveChanges();
        }

    }
}
