using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity;
using ManyForMany.Models.Entity.Skill;
using ManyForMany.Models.Entity.User;
using ManyForMany.ViewModel.Categories;
using ManyForMany.ViewModel.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiLanguage.Exception;
using MvcHelper.Entity;

namespace ManyForMany.Controller.User
{

    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class AccountController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Context _context;
        private static bool _databaseChecked;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> _signInManager,
            Context context)
        {
            _userManager = userManager;
            this._signInManager = _signInManager;
            _context = context;
        }

        [Authorize]
        [Microsoft.AspNetCore.Mvc.HttpGet(nameof(UserInfo))]
        public async Task<object> UserInfo()
        {
            var user = await _userManager.GetUserAsync(User);

            return new
            {
                User = user,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }

        [Authorize]
        [MvcHelper.Attributes.HttpGet]
        public async Task<UserViewModel> Infromation()
        {
            var user = await _userManager.GetUserAsync(User);

            return user.ToViewModel();
        }


        //TODO add skill editor and parameter contorller
        [Authorize]
        [MvcHelper.Attributes.HttpPut()]
        public async Task Edit(UserViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (!string.IsNullOrEmpty(model.Name))
            {
                user.Name = model.Name;
            }

            if (!string.IsNullOrEmpty(model.SurName))
            {
                user.UserName = model.UserName;
            }

            if (!string.IsNullOrEmpty(model.UserName))
            {
                var userNameInUse = _context.Users.Any(x => x.UserName == model.UserName);

                if (userNameInUse)
                {
                    throw new MultiLanguageException(nameof(model.UserName), Errors.UserNameIsBusy);
                }

                user.UserName = model.UserName;
            }
            
            _context.SaveChanges();

        }

        #region Skills

        [Authorize]
        [MvcHelper.Attributes.HttpPost(nameof(Skill))]
        public async void Add(int[] skillsId)
        {
            var id = _userManager.GetUserId(User);

            var userTask = _context.Users.Include(x => x.Skills).FirstOrDefaultAsync(x => x.Id == id);

            var skills = _context.Skills.Get(x=>x.Id , skillsId);
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
        [MvcHelper.Attributes.HttpDelete(nameof(Skill))]
        public async void Remove(int[] skillsId)
        {
            var id = _userManager.GetUserId(User);

            var userTask = _context.Users.Include(x => x.Skills).FirstOrDefaultAsync(x => x.Id == id);
            
            var skills = _context.Skills.Get(x=>x.Id, skillsId);

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

        #endregion

        #region Categories


        [Authorize]
        [MvcHelper.Attributes.HttpGet(nameof(Categories))]
        public ThumbnailCategoryViewModel[] Categories()
        {
            var userId = _userManager.GetUserId(User);

            return _context.Users.Include(x => x.LikesCategories).Get(userId).Result.LikesCategories
                .Select(x => x.ToThumbnail()).ToArray();
        }

        [Authorize]
        [MvcHelper.Attributes.HttpPost(nameof(LikedCategories))]
        public async void LikedCategories(int[] skillsId)
        {
            var id = _userManager.GetUserId(User);

            var userTask = _context.Users.Include(x => x.LikesCategories).FirstOrDefaultAsync(x => x.Id == id);

            var categories = _context.Categories.Get(x => x.Id, skillsId);
            var user = await userTask;

            foreach (var likedCategory in categories)
            {
                if (user.LikesCategories.Contains(likedCategory))
                {
                    throw new MultiLanguageException(nameof(Skill.Id), Errors.SkillIsAlreadyExist);
                }

                user.LikesCategories.Add(likedCategory);
            }

            _context.SaveChanges();
        }

        [Authorize]
        [MvcHelper.Attributes.HttpDelete(nameof(Skill))]
        public async void RemoveSkills(int[] categoryId)
        {
            var id = _userManager.GetUserId(User);

            var userTask = _context.Users.Include(x => x.LikesCategories).FirstOrDefaultAsync(x => x.Id == id);

            var skills = _context.Categories.Get(x => x.Id, categoryId);

            var user = await userTask;

            foreach (var skill in skills)
            {
                if (!user.LikesCategories.Contains(skill))
                {
                    throw new MultiLanguageException(nameof(Skill.Id), Errors.SkillIsAlreadyExist);
                }

                user.LikesCategories.Remove(skill);
            }

            _context.SaveChanges();
        }


        #endregion
    }
}
