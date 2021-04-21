using System;
using System.Collections.Generic;
using GraphQlHelper;
using Microsoft.AspNetCore.Identity;
using Google.Apis.Auth;
using TODOIT.ViewModel.User;

namespace TODOIT.Model.Entity.User
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser, IBaseElement<string>
    {
        public ApplicationUser()
        {
        }

        public ApplicationUser(GoogleJsonWebSignature.Payload payload)
        {
            Name = payload.GivenName;
            Surrname = payload.FamilyName;
            Email = payload.Email;
            Picture = payload.Picture;
            UserName = payload.Email;
        }

        public ApplicationUser(ExternalLoginViewModel model) : this((UserViewModel)model)
        {
            Id = model.Id;
        }

        public ApplicationUser(UserViewModel model)
        {
            this.Assign(model);
        }

        #region properties

        public string Name { get; set; }

        public string Surrname { get; set; }

        public string Picture { get; set; }
        
        
        #endregion
    }
}