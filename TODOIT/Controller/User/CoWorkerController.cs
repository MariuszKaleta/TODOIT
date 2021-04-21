using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.User;

namespace TODOIT.Controller.User
{
    [Obsolete]
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class CoWorkerController : Microsoft.AspNetCore.Mvc.Controller
    {
        public CoWorkerController( Context context, UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            _context = context;
        }

        #region Properties

        public UserManager<ApplicationUser> UserManager { get; }
        private ILogger _logger;
        private readonly Context _context;

        #endregion

      //  #region Get
        /*
       [AllowAnonymous]
       [MvcHelper.Attributes.HttpGet("{userId}")]
       public async Task<PublicUserViewModel> Get(string userId)
       {
           var user = await _context.Users
               .Include(x => x.Skills)
               .Get(userId);

           return user.ToViewModel(_context);
       }


       [Authorize]
       [MvcHelper.Attributes.HttpGet(nameof(LookNew))]
       public async Task<PublicUserViewModel[]> LookNew([FromQuery] int? start, [FromQuery] int? count, [FromQuery] int[] skills, [FromQuery] int? howMatchSkillsShouldByIncludeInOrder)
       {
           var userId = UserManager.GetUserId(User);

           var orders = _context.Users
               .Where(x => x.RejectedByOtherUsers.All(y => y.Id != userId)
                           && x.InterestedByOtherUsers.All(y => y.Id != userId)
                           && x.Id != userId
               );

           if (skills != null)
           {
               var findSkills = _context.Skills.Get(x => x.Id, skills);

               orders = orders.Filter(x => x.Skills, findSkills, howMatchSkillsShouldByIncludeInOrder);
           }

           return
               orders
                   .TryTake(start, count)
                   .Select(x=>x.ToViewModel(_context))
                   .ToArray();
       }


       [Authorize]
       [MvcHelper.Attributes.HttpGet(nameof(Watched))]
       public async Task<PublicUserViewModel[]> Watched([FromQuery] int? start, [FromQuery] int? count, [FromQuery] int[] skills, [FromQuery] int? howMatchSkillsShouldByIncludeInOrder)
       {
           var userId = UserManager.GetUserId(User);

           var orders = _context.Users.Where(x =>
               x.InterestedByOtherUsers.Any(y => y.Id == userId)
               || x.RejectedByOtherUsers.Any(y => y.Id == userId)
           );

           if (skills != null)
           {
               var findSkills = _context.Skills.Get(x => x.Id, skills);

               orders = orders.Filter(x => x.Skills, findSkills, howMatchSkillsShouldByIncludeInOrder);
           }

           return orders
               .TryTake(start, count)
               .Select(x => x.ToViewModel(_context))
               .ToArray();
       }

       [Authorize]
       [MvcHelper.Attributes.HttpGet(nameof(Interested))]
       public async Task<PublicUserViewModel[]> Interested([FromQuery] int? start, [FromQuery] int? count, [FromQuery] int[] skills, [FromQuery] int? howMatchSkillsShouldByIncludeInOrder)
       {
           var userId = UserManager.GetUserId(User);

           var orders = _context.Users
               .Where(x => x.InterestedByOtherUsers.Any(y => y.Id == userId));

           if (skills != null)
           {
               var findSkills = _context.Skills.Get(x => x.Id, skills);

               orders = orders.Filter(x => x.Skills, findSkills, howMatchSkillsShouldByIncludeInOrder);
           }


           return orders 
               .TryTake(start, count)
               .TryTake(start, count)
               .Select(x => x.ToViewModel(_context))
               .ToArray();
       }

       [Authorize]
       [MvcHelper.Attributes.HttpGet(nameof(Rejected))]
       public async Task<PublicUserViewModel[]> Rejected([FromQuery] int? start, [FromQuery] int? count, [FromQuery] int[] skills, [FromQuery] int? howMatchSkillsShouldByIncludeInOrder)
       {
           var userId = UserManager.GetUserId(User);

           var orders = _context.Users
               .Where(x => x.RejectedByOtherUsers.Any(y => y.Id == userId));

           if (skills != null)
           {
               var findSkills = _context.Skills.Get(x => x.Id, skills);

               orders = orders.Filter(x => x.Skills, findSkills, howMatchSkillsShouldByIncludeInOrder);
           }

           return orders
               .TryTake(start, count)
               .Select(x => x.ToViewModel(_context)).ToArray();
       }
       


        [Authorize]
        [MvcHelper.Attributes.HttpPost(nameof(Decide), "{userId}", "{decision}")]
        public async Task Decide(string userId, bool decision)
        {
            var user = await UserManager.GetUserAsync(User);

            await Decide(user, userId, decision);

            await _context.SaveChangesAsync();
        }

        [Authorize]
        [MvcHelper.Attributes.HttpPost(nameof(Decide))]
        public async Task Decide(DecideViewModel[] elements)
        {
            var user = await UserManager.GetUserAsync(User);

            var tasks = elements.Select(x => Decide(user, x.ElementId, x.Decision));

            await Task.WhenAll(tasks);

            await _context.SaveChangesAsync();
        }

        [Authorize]
        [MvcHelper.Attributes.HttpPost(nameof(Matched))]
        public PublicUserViewModel[] Matched([FromQuery] int? start, [FromQuery] int? count)
        {
            var userId = UserManager.GetUserId(User);


            return _context.MathcedCoWorkerses
                .Where(x => x.Persons.Any(y => y.Id == userId))
                .Select(x => x.Persons.FirstOrDefault(y => y.Id != userId))
                .TryTake(start, count)
                .Select(x => x.ToViewModel(_context))
                .ToArray();
        }


        #endregion
        /*
        #region Helper

        private async Task Decide(ApplicationUser UserWhichMakeChoose, string userId, bool decide)
        {
            var user = await _context.Users
                .Include(x => x.InterestedByOtherUsers)
                .Include(x => x.RejectedByOtherUsers)
                .Get(userId);

            if (user == UserWhichMakeChoose)
            {
                throw new MultiLanguageException(nameof(UserWhichMakeChoose), Errors.YouCantAddToTeamYourself);
            }

            var match = _context.MathcedCoWorkerses.FirstOrDefault(x =>
                x.Persons.Contains(UserWhichMakeChoose) && x.Persons.Contains(user));

            if (decide)
            {
                if (match != null)
                {
                    throw new MultiLanguageException(nameof(MathcedCoWorkers), Errors.MatchAlredyExist);
                }

                user.InterestedByOtherUsers.Add(UserWhichMakeChoose);
                user.RejectedByOtherUsers.Remove(UserWhichMakeChoose);

                if (UserWhichMakeChoose.InterestedByOtherUsers.Contains(user))
                {
                    match = new MathcedCoWorkers(UserWhichMakeChoose, user);

                    var chat = new SingleChat(UserWhichMakeChoose, user);

                    _context.SingleChats.Add(chat);

                    _context.MathcedCoWorkerses.Add(match);
                }
            }
            else
            {
                if (match != null)
                {
                    _context.Remove(match);
                }

                user.RejectedByOtherUsers.Add(UserWhichMakeChoose);
                user.InterestedByOtherUsers.Remove(UserWhichMakeChoose);
            }
        }

        #endregion

        */
    }
}