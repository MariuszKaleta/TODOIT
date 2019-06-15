using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizeTester.Model;
using AuthorizeTester.Model.Error;
using ManyForMany.Controller;
using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Model.Entity.Orders;
using ManyForMany.ViewModel.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiLanguage.Exception;
using StringHelper;

namespace ManyForMany.Model.Entity.User
{
    public class ApplicationUser : IdentityUser
    {
        private ApplicationUser()
        {

        }

        public ApplicationUser(UserManager<ApplicationUser> userManager, DbContext context)
        {
            userManager.AddToRoleAsync(this, CustomRoles.BasicUser).GetAwaiter().GetResult();
            context.SaveChanges();
        }

        #region Proeprties

        public bool ShowPublic { get; set; } = true;

        public List<ApplicationUser> InterestedCooperators { get; private set; }

        public List<ApplicationUser> RejectedCooperators { get; private set; }

        public List<Order>  InterestedOrders { get; private set; }

        public List<Order>  RejectedOrders { get; private set; }
        
        public List<Order> OwnOrders { get; private set; }

        #endregion
    }

    public static class UserExtension
    {
        public static IEnumerable<Order> WatchedAndUserOrders(this ApplicationUser user)
        {
            foreach (var order in user.WatchedOrders())
            {
                yield return order;
            }

            foreach (var order in user.OwnOrders)
            {
                yield return order;
            }
        }

        public static IEnumerable<Order> WatchedOrders(this ApplicationUser user)
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

        public static IEnumerable<ApplicationUser> WatchedUsers(this ApplicationUser user)
        {
            foreach (var cooperator in user.InterestedCooperators)
            {
                yield return cooperator;
            }

            foreach (var cooperator in user.RejectedCooperators)
            {
                yield return cooperator;
            }
        }

        public static async Task<ApplicationUser> GetUser<T>(this UserManager<ApplicationUser> userManager, ClaimsPrincipal userClaimsPrincipal, ILogger<T> logger)
        {
            var user = await userManager.GetUserAsync(userClaimsPrincipal);

            if (null == user)
            {
                logger.LogError(nameof(User), Error.ElementDoseNotExist.ToString(), userClaimsPrincipal);
                throw new MultiLanguageException(nameof(User), Error.ElementDoseNotExist);
            }

            return user;
        }

        public static UserViewModel ToUserInformation(this ApplicationUser user)
        {
            return new UserViewModel(user);
        }

        public static void Edit(this ApplicationUser user, UserViewModel model)
        {
            if (user.Email != model.Email)
            {
                user.Email = model.Email;
            }

            if (user.UserName != model.UserName)
            {
                user.UserName = model.UserName;
            }

            if (user.PhoneNumber != model.PhoneNumber)
            {
                user.PhoneNumber = model.PhoneNumber;
            }

            if (user.ShowPublic != model.ShowPublic)
            {
                user.ShowPublic = model.ShowPublic;
            }
        }
    }
}