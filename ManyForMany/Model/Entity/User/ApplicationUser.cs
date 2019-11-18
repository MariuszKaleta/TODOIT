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

        /*
        public ApplicationUser(GoogleJsonWebSignature.Payload model)
        {
            EmailConfirmed = model.EmailVerified;
            Picture = model.Picture;
            Email = model.Email;
            UserName = model.Email;
            Name = model.Name;
            SurName = model.FamilyName;
        }
        */
        #region properties

        //public string Picture { get; set; }

        public string Name { get; set; }

        public string Surrname { get; set; }

        public string Picture { get; set; }

 
        #region Skills

        //TODO wywlic to z sta i zrobić to biernie w skilach
        //public List<Skill.Skill> Skills { get; private set; }

        //public List<Category.Category> InterestedCategories { get; private set; }

        #endregion

        #region Team

       // public List<ApplicationUser> InterestedByOtherUsers { get; private set; }

        //public List<ApplicationUser> RejectedByOtherUsers { get; private set; }

        #endregion
        
        #endregion
    }
}