using System;
using System.Threading.Tasks;
using FileHelper.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.User;
using TODOIT.Model.File.Path;

namespace TODOIT.Controller
{
    [Obsolete]
    [ApiController]
    [Authorize(Roles = CustomRoles.Admin)]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class AdminController : Microsoft.AspNetCore.Mvc.Controller
    {

        public AdminController(Context context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        #region Properties

        public UserManager<ApplicationUser> _userManager;
        private readonly Context _context;

        #endregion

        /*

       /// <summary>
       /// Check if i have access to this api
       /// </summary>
       /// <returns></returns>
       [AllowAnonymous]
       [MvcHelper.Attributes.HttpGet(nameof(IAmAdmin))]
       public async Task<bool> IAmAdmin()
       {
           var user = await _userManager.GetUserAsync(User);

           return await _userManager.IsInRoleAsync(user, CustomRoles.Admin);
       }


       [MvcHelper.Attributes.HttpGet(nameof(Context.Users))]
       public async Task<ThumbnailUserViewModel[]> Users([FromQuery] string text, [FromQuery] int start, [FromQuery] int count)
       {
           return _context.Users
               .Filter(text, x => x.Name, x => x.SurName)
               .TryTake(start, count)
               .Select(x => x.ToThumbnail()).ToArray();
       }



       [MvcHelper.Attributes.HttpPost(nameof(Category), nameof(Create))]
       public async Task Create(CreateCategoryViewModel model)
       {
           var category = new Category(model);

           _context.Categories.Add(category);

           _context.SaveChanges();

           await FileManager.UploadFile(model.Logo, new LogoCategoryPath(category));
       }
        */
    }
}
