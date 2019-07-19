using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity;
using ManyForMany.Models.Entity.Skill;
using ManyForMany.Models.Entity.User;
using ManyForMany.ViewModel.Categories;
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
    public class CateogriesController : Microsoft.AspNetCore.Mvc.Controller
    {

        public CateogriesController(ILogger<CateogriesController> logger, Context context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        #region Properties

        public UserManager<ApplicationUser> _userManager;
        private ILogger<CateogriesController> _logger;
        private readonly Context _context;

        #endregion

        #region Get

        [MvcHelper.Attributes.HttpGet()]
        public ThumbnailCategoryViewModel[] Categories([FromQuery] string text, [FromQuery] int? start, [FromQuery] int? count)
        {
            return _context.Categories
                .Filter(text, x => x.Name)
                .TryTake(start, count)
                .Select(x => x.ToThumbnail())
                .ToArray();
        }

        [MvcHelper.Attributes.HttpGet("{id}")]
        public PublicCategoryViewModel Categories(int id)
        {
            return _context
                .Categories
                .Get(x => x.Id, id, Errors.CategoryDoseNotExist)
                .ToViewModel();
        }

        //todo temp
        [AllowAnonymous]
        [MvcHelper.Attributes.HttpPost()]
        public void Create(CreateCategoryViewModel model)
        {
            var category = new Category(model);

            _context.Categories.Add(category);

            _context.SaveChanges();
        }


        #endregion
    }
}
