using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcHelper.Entity;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.User;

namespace TODOIT.Controller
{
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class CateogriesController : Microsoft.AspNetCore.Mvc.Controller
    {

        public CateogriesController( Context context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        #region Properties

        public UserManager<ApplicationUser> _userManager;
        private readonly Context _context;

        #endregion

        #region Get

        /*

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
        */
        #endregion
    }
}
