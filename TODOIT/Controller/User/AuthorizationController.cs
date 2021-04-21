using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using Facebook;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultiLanguage.Exception;
using MvcHelper;
using OpenIddict.Abstractions;
using OpenIddict.Mvc.Internal;
using OpenIddict.Server;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity.User;
using TODOIT.ViewModel.User;

namespace TODOIT.Controller.User
{
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class AuthorizationController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthorizationController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

       
        [HttpGet("Test")]
        public async Task<string> Test()
        {
           var user = await _userManager.GetUserAsync(User);


            return user.Name;
        }

        /*


        [HttpPost(AuthorizationHelper.AbsolutePath + AuthorizationHelper.TokenEndPoint)]
        [Produces(Produces.Json)]
        public async Task<IActionResult> Exchange([ModelBinder(typeof(OpenIddictMvcBinder))] OpenIdConnectRequest request)
        {
            //throw new NotImplementedException();
            switch (request.GrantType)
            {
                case CustomGrantTypes.Google:
                    {
                        var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token);

                        var user = await _userManager.FindByEmailAsync(payload.Email);

                        if (user == null)
                        {
                            user = await _userManager.Register(payload);
                        }

                        return await this.CreateTicketAsync(_signInManager, user, request);
                    }

                case CustomGrantTypes.Linkedin:
                    {
                        throw new NotImplementedException();

                    }

                case CustomGrantTypes.Facebook:
                    {

                        var payload = await FacebookJsonWebSignature.ValidateAsync(request.Token, Startup.Config.Authentication.Facebook.AppId);


                        throw new NotImplementedException();
                    }

                default:
                    throw new MultiLanguageException(nameof(request.GrantType),
                        OpenIdConnectConstants.Errors.UnsupportedGrantType);
            }
        }


        [AllowAnonymous]
        [MvcHelper.Attributes.HttpGet(nameof(GrantTypes))]
        public string[] GrantTypes()
        {
            //throw new NotImplementedException();
            return CustomGrantTypes.All.ToArray();
        }
        
            */
        #region ExternalLogin



        [AllowAnonymous]
        [Produces("application/json")]
        [HttpGet(nameof(ExternalProviders))]
        public async Task<string[]> ExternalProviders()
        {
            var elements = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Select(x => x.Name).ToArray();

            return elements;
        }

        //
        // Get: /Account/ExternalLogin
        [MvcHelper.Attributes.HttpGet(nameof(ExternalLogin), "{provider}")]
        [AllowAnonymous]
        public IActionResult ExternalLogin(
            string provider,
            [FromQuery] string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback),
                nameof(AuthorizationController).Replace(nameof(Controller), string.Empty), new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [MvcHelper.Attributes.HttpGet(nameof(ExternalLoginCallback))]
        //[Produces(Produces.Json)]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            returnUrl = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "http://192.168.1.115:3000";

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return Redirect(returnUrl);
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return Redirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                var surrname = info.Principal.FindFirstValue(ClaimTypes.Surname);
                var id = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                await ExternalLoginConfirmation(new ExternalLoginViewModel
                {
                    Email = email,
                    Name = name,
                    Surrname = surrname,
                    Id = id
                });
            }



            return Redirect(returnUrl);
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost(nameof(ExternalLoginConfirmation))]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return;
                }

                var user = new ApplicationUser(model);

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);

                    if (result.Succeeded)
                    {
                        result = await _userManager.AddToRoleAsync(user, CustomRoles.BasicUser);

                        await _signInManager.SignInAsync(user, false);
                        return;
                    }
                }

                AddErrors(result);
            }
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        }

        #endregion

        //
        // POST: /Account/LogOff
        [HttpPost(nameof(LogOff))]
        public async Task LogOff()
        {
            await _signInManager.SignOutAsync();
        }
    }
}