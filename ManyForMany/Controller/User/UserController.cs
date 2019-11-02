using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories.Contracts;
using TODOIT.ViewModel.Order;
using TODOIT.ViewModel.User;

namespace TODOIT.Controller.User
{

    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class UserController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly ISkillRepository _skillRepository;

        public UserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserRepository userRepository,
            ISkillRepository skillRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _skillRepository = skillRepository;
        }

        #region Api

        /// <summary>
        /// edit existed User
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = CustomGrantTypes.Google)]
        [MvcHelper.Attributes.HttpPost(nameof(Update))]
        public async Task Update(UserViewModel model)
        {
            var userAsync = _userManager.GetUserAsync(User);

            await _userRepository.Update(await userAsync, model);
        }

        /// <summary>
        /// edit existed User
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = CustomGrantTypes.Google)]
        [MvcHelper.Attributes.HttpPost(nameof(Skills), nameof(Update))]
        public async Task Skills(string[] skills)
        {
            var userAsync = _userManager.GetUserAsync(User);
            var skillAsync = _skillRepository.Get(skills);

            await _userRepository.UpdateSkills(await userAsync, await skillAsync);
        }

        /*
        [Authorize(AuthenticationSchemes = CustomGrantTypes.Google)]
        [MvcHelper.Attributes.HttpPost(nameof(Remove))]
        public async Task Remove(Guid orderId)
        {
            var user = await _userManager.GetUserAsync(User);

            _userRepository.Delete(user, true);
        }
        */

        #endregion

        /*
        /// <summary>
        /// Return Information about user
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [MvcHelper.Attributes.HttpGet(nameof(Me))]
        public async Task<PublicUserViewModel> Me()
        {
            var user = await _userManager.GetUserAsync(User);

            return user.ToViewModel(_context);
        }
       
        /// <summary>
        /// Return Information about user
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [MvcHelper.Attributes.HttpGet("{userId}")]
        public async Task<PublicUserViewModel> Get(string userId)
        {
            var user = await _context.Users
                
                .Get(userId);

            return user.ToViewModel(_context);
        }


        /// <summary>
        /// Edit base user properties
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [MvcHelper.Attributes.HttpPut(nameof(Me))]
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
        /*
        /// <summary>
        /// Return all your skills
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Obsolete]
        [MvcHelper.Attributes.HttpGet(nameof(ApplicationUser.Skills))]
        public SkillThumbnailViewModel[] Skills()
        {
            var userId = _userManager.GetUserId(User);

            return _context.Users.Include(x => x.Skills).Get(userId).Result.Skills
                .Select(x => x.ToThumbnail()).ToArray();
        }
        
        /// <summary>
        /// Add skills tou your profile
        /// </summary>
        /// <param name="skillsId"></param>
        [Authorize]
        [Obsolete]
        [MvcHelper.Attributes.HttpPost(nameof(ApplicationUser.Skills), nameof(Add))]
        public async void Add(int[] skillsId)
        {
            var id = _userManager.GetUserId(User);

            var userTask = _context.Users.Include(x => x.Skills).FirstOrDefaultAsync(x => x.Id == id);

            var skills = _context.Skills.Get(x => x.Id, skillsId);
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
        

        /// <summary>
        /// Remove skills from your profile
        /// </summary>
        /// <param name="skillsId"></param>
        [Authorize]
        [Obsolete]
        [MvcHelper.Attributes.HttpDelete(nameof(ApplicationUser.Skills), nameof(Remove))]
        public async void Remove(int[] skillsId)
        {
            var id = _userManager.GetUserId(User);

            var userTask = _context.Users.Include(x => x.Skills).FirstOrDefaultAsync(x => x.Id == id);

            var skills = _context.Skills.Get(x => x.Id, skillsId);

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

        /// <summary>
        /// Return all your interested categoriesz
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [MvcHelper.Attributes.HttpGet(nameof(ApplicationUser.InterestedCategories))]
        public ThumbnailCategoryViewModel[] Categories()
        {
            var userId = _userManager.GetUserId(User);

            return _context.Users.Include(x => x.InterestedCategories).Get(userId).Result.InterestedCategories
                .Select(x => x.ToThumbnail()).ToArray();
        }

        /// <summary>
        /// Add interested categories to your profile
        /// </summary>
        /// <param name="skillsId"></param>
        [Authorize]
        [MvcHelper.Attributes.HttpPost(nameof(ApplicationUser.InterestedCategories), nameof(Add))]
        public async void LikedCategories(int[] skillsId)
        {
            var id = _userManager.GetUserId(User);

            var userTask = _context.Users.Include(x => x.InterestedCategories).FirstOrDefaultAsync(x => x.Id == id);

            var categories = _context.Categories.Get(x => x.Id, skillsId);
            var user = await userTask;

            foreach (var likedCategory in categories)
            {
                if (user.InterestedCategories.Contains(likedCategory))
                {
                    throw new MultiLanguageException(nameof(Category.Id), Errors.YouLikedThisCategoryBefore);
                }

                user.InterestedCategories.Add(likedCategory);
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// Remove interested categories from your profile
        /// </summary>
        /// <param name="categoryId"></param>
        [Authorize]
        [MvcHelper.Attributes.HttpDelete(nameof(ApplicationUser.InterestedCategories), nameof(Remove))]
        public async void RemoveCategory(int[] categoryId)
        {
            var id = _userManager.GetUserId(User);

            var userTask = _context.Users.Include(x => x.InterestedCategories).FirstOrDefaultAsync(x => x.Id == id);

            var skills = _context.Categories.Get(x => x.Id, categoryId);

            var user = await userTask;

            foreach (var skill in skills)
            {
                if (!user.InterestedCategories.Contains(skill))
                {
                    throw new MultiLanguageException(nameof(Skill.Id), Errors.SkillIsAlreadyExist);
                }

                user.InterestedCategories.Remove(skill);
            }

            _context.SaveChanges();
        }

 
        #endregion

       */

    }
}
