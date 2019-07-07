using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth;
using ManyForMany.Models.Configuration;
using ManyForMany.ViewModel.Team;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MvcHelper.Entity;

namespace ManyForMany.Models.Entity.User
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser, IId<string>
    {
        private ApplicationUser()
        {

        }

        public ApplicationUser(GoogleJsonWebSignature.Payload model)
        {
            EmailConfirmed = model.EmailVerified;
            Picture = model.Picture;
            Email = model.Email;
            UserName = model.Email;
            Name = model.Name;
            SurName = model.FamilyName;
        }



        #region properties

        public string Picture { get; private set; }

        public string Name { get; set; }

        public string SurName { get; set; }

        #region Skills

        //TODO wywlic to z sta i zrobić to biernie w skilach
        public List<Skill.Skill> Skills { get; private set; }

        #endregion

        #region Team

        public List<ApplicationUser> InterestedByOtherUsers { get; private set; }

        public List<ApplicationUser> RejectedByOtherUsers { get; private set; }

        #endregion
        
        #endregion
    }


    public static class UserExtension
    {
        public static string FirstName(this ApplicationUser user)
        {
            return user.Name.Replace(user.SurName, string.Empty);
        }

        public static UserViewModel ToViewModel(this ApplicationUser user)
        {
            return new UserViewModel()
            {
                Name =  user.Name,
                SurName =  user.SurName,
                UserName =  user.UserName
            };

        }

        public static UserInformationViewModel ToUserInformation(this ApplicationUser user)
        {
            return  new UserInformationViewModel(user);
        }
        public static ThumbnailUserViewModel ToUserThumbnail(this ApplicationUser user)
        {
            return  new ThumbnailUserViewModel(user);
        }

        public static async Task<ApplicationUser> Get(this IQueryable<ApplicationUser> users, string id, ILogger logger)
        {
            return await users.Get(id, Errors.UserIsNotExist, logger);
        }

        public static IQueryable<ApplicationUser> Get<T>(this IQueryable<ApplicationUser> users, IEnumerable<string> id)
        {
            return  users.Get(id);
        }
    }
}