using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using AuthorizationServer.Models;
using AuthorizeTester.Model;
using Google.Apis.Auth;
using ManyForMany.Models.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiLanguage.Exception;
using MvcHelper;
using OpenIddict.Validation;

namespace AuthorizationServer.Controllers
{
    public class AccountController : Controller
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
        [HttpGet(nameof(UserInfo))]
        public async Task<object> UserInfo()
        {
            var user = await _userManager.GetUserAsync(User);

            return new
            {
                User = user,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }

        [AllowAnonymous]
        [MvcHelper.Attributes.HttpPost(nameof(Register), "{provider}")]
        public async Task Register(string provider, string jwtToken)
        {
            ApplicationUser user;

            try
            {
                switch (provider)
                {
                    case CustomGrantTypes.Google:
                        var payload = await GoogleJsonWebSignature.ValidateAsync(jwtToken);

                        user = await _userManager.FindByEmailAsync(payload.Email);

                        if (user != null)
                        {
                            throw new MultiLanguageException(Errors.UserIsExist, payload.Email);
                        }

                        user = new ApplicationUser(payload);
                        break;

                    default:
                        throw new MultiLanguageException(OpenIdConnectConstants.Errors.UnsupportedGrantType, provider);
                        break;
                }
            }
            catch (InvalidJwtException)
            {
                throw new MultiLanguageException(nameof(jwtToken), OpenIdConnectConstants.Errors.AccessDenied);
            }

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                var firstError = result.Errors.FirstOrDefault();
                throw new MultiLanguageException(firstError.Code, firstError.Description);
            }

            result = await _userManager.AddToRoleAsync(user, CustomRoles.BasicUser);

            if (!result.Succeeded)
            {
                var firstError = result.Errors.FirstOrDefault();
                throw new MultiLanguageException(firstError.Code, firstError.Description);
            }
        }

        [AllowAnonymous]
        [MvcHelper.Attributes.HttpGet(nameof(Providers))]
        public string[] Providers()
        {
            return CustomGrantTypes.All.ToArray();
        }

        #region Helpers

        // The following code creates the database and schema if they don't exist.
        // This is a temporary workaround since deploying database through EF migrations is
        // not yet supported in this release.
        // Please see this http://go.microsoft.com/fwlink/?LinkID=615859 for more information on how to do deploy the database
        // when publishing your application.
        private static void EnsureDatabaseCreated(Context context)
        {
            if (!_databaseChecked)
            {
                _databaseChecked = true;
                context.Database.EnsureCreated();
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #endregion
    }
}
