using AuthorizeTester.Model;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AuthorizationServer.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
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

        #endregion

    }
}