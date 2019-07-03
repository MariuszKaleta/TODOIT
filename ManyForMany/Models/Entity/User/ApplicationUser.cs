using Google.Apis.Auth;
using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity.Chat;
using ManyForMany.Models.Entity.Order;
using ManyForMany.Models.Entity.Rate;
using ManyForMany.ViewModel.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MvcHelper.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer.Models
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

        public List<Skill> Skills { get; private set; }

        #endregion

        #region Team

        public List<Chat> Chats { get; private set; }

        #endregion


        #region Opininons
        
        public List<Opinion> OpinionsAboutMe { get; private set; }

        
        #endregion


        #endregion

    }


    public static class UserExtension
    {
        public static string FirstName(this ApplicationUser user)
        {
            return user.Name.Replace(user.SurName, string.Empty);
        }

        public static void Remove(this ApplicationUser user, Context context)
        {
            context.Users.Remove(user);
            /*
            foreach (var chat in user.Chats)
            {
                if (chat.AdminId == user.Id)
                {
                    chat.AdminId = chat.Members.FirstOrDefault()?.Id;
                }
                else
                {
                    chat.Members.Remove(user);
                }
            }
            */


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