using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizeTester.Model;
using Google.Apis.Auth;
using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Models.Configuration;
using ManyForMany.ViewModel.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using MvcHelper.Entity;

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

        public string Name { get; private set; }
        public string SurName { get; private set; }

        public List<Order> InterestedOrders { get; private set; }

        public List<Order> RejectedOrders { get; private set; }

        public List<Order> OwnOrders { get; private set; }

        public List<Order> MemberOfOrders { get; private set; }

        #endregion

    }


    public static class UserExtension
    {
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
            foreach (var order in user.MemberOfOrders)
            {
                order.ActualTeam.Remove(user);
            }

            foreach (var order in user.OwnOrders)
            {
                order.Remove(context);
            }
        }

        public static IEnumerable<Order> WatchedAndUserOrdersId(this ApplicationUser user)
        {
            foreach (var order in user.WatchedOrdersId())
            {
                yield return order;
            }

            foreach (var order in user.OwnOrders)
            {
                yield return order;
            }
        }

        public static IEnumerable<Order> WatchedOrdersId(this ApplicationUser user)
        {
            foreach (var order in user.InterestedOrders)
            {
                yield return order;
            }

            foreach (var order in user.RejectedOrders)
            {
                yield return order;
            }
        }

        public static UserInformationViewModel ToUserInformation(this ApplicationUser user)
        {
            return  new UserInformationViewModel(user);
        }
        public static UserThumbnailViewModel ToUserThumbnail(this ApplicationUser user)
        {
            return  new UserThumbnailViewModel(user);
        }

        public static async Task<ApplicationUser> Get<T>(this IQueryable<ApplicationUser> users, string id, ILogger<T> logger)
        {
            return await users.Get(id, Errors.UserIsNotExist, logger);
        }
        public static ApplicationUser Get<T>(this IEnumerable<ApplicationUser> users, string id, ILogger<T> logger)
        {
            return users.Get(id, Errors.UserIsNotExist, logger);
        }
    }
}