/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        #region PasswordTemp

        /*
        [Obsolete]
        [AllowAnonymous]
        [MvcHelper.Attributes.HttpPost(nameof(Register))]
        public async Task Register(PasswordViewModel model)
        {
            await _userManager.Register(model);
        }
        */

        #endregion

        #region ExternalLogin
            
        /*

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
        [HttpGet(nameof(ExternalLoginCallback))]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return LocalRedirect(returnUrl);
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return LocalRedirect(returnUrl);
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

            return LocalRedirect(returnUrl ?? string.Empty);
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

       
        */

        #endregion

        //
        // POST: /Account/LogOff
        [HttpPost(nameof(LogOff))]
        public async Task LogOff()
        {
            await _signInManager.SignOutAsync();
        }
    }

    public static class AuthorizationExtension
    {
        public static async Task<ApplicationUser> Register(this UserManager<ApplicationUser> userManager, GoogleJsonWebSignature.Payload payload)
        {
            var user = new ApplicationUser(payload);

            var result = await userManager.CreateAsync(user);

            await Register(userManager, user, result);

            return user;
        }

        public static async Task<ApplicationUser> Register(this UserManager<ApplicationUser> userManager, PasswordViewModel model)
        {
            var user = new ApplicationUser(model);

            var result = await userManager.CreateAsync(user, model.Password);

            await Register(userManager, user, result);

            return user;
        }
        
        private static async Task Register(this UserManager<ApplicationUser> userManager, ApplicationUser user, IdentityResult result)
        {

            if (!result.Succeeded)
            {
                var firstError = result.Errors.FirstOrDefault();
                throw new MultiLanguageException(firstError.Code, firstError.Description);
            }

            result = await userManager.AddToRoleAsync(user, CustomRoles.BasicUser);

            if (!result.Succeeded)
            {
                var firstError = result.Errors.FirstOrDefault();
                throw new MultiLanguageException(firstError.Code, firstError.Description);
            }
        }

        public static async Task<IActionResult> Password(this ControllerBase controller  , UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, OpenIdConnectRequest request)
        {
            var user = await userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                var properties = new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIdConnectConstants.Properties.Error] = OpenIdConnectConstants.Errors.InvalidGrant,
                    [OpenIdConnectConstants.Properties.ErrorDescription] = "The username/password couple is invalid."
                });

                return controller.Forbid(properties, OpenIddictServerDefaults.AuthenticationScheme);
            }

            // Validate the username/password parameters and ensure the account is not locked out.
            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                var properties = new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIdConnectConstants.Properties.Error] = OpenIdConnectConstants.Errors.InvalidGrant,
                    [OpenIdConnectConstants.Properties.ErrorDescription] = "The username/password couple is invalid."
                });

                return controller.Forbid(properties, OpenIddictServerDefaults.AuthenticationScheme);
            }

            // Create a new ClaimsPrincipal containing the claims that
            // will be used to create an id_token, a token or a code.
            var principal = await signInManager.CreateUserPrincipalAsync(user);

            return await controller.CreateTicketAsync(principal, request);
        }
        
        #region token

        public static async Task<IActionResult> CreateTicketAsync(this ControllerBase controller, SignInManager<ApplicationUser> signInManager, ApplicationUser user, OpenIdConnectRequest request)
        {
            // Create a new ClaimsPrincipal containing the claims that
            // will be used to create an id_token, a token or a code.
            var principal = await signInManager.CreateUserPrincipalAsync(user);

            return await controller.CreateTicketAsync(principal, request);

        }
       
        public static async Task<IActionResult> CreateTicketAsync(this ControllerBase controller, ClaimsPrincipal principal, OpenIdConnectRequest request)
        {
            // Create a new authentication ticket holding the user identity.
            var ticket = new AuthenticationTicket(principal,
                new AuthenticationProperties(),
                OpenIddictServerDefaults.AuthenticationScheme);

            // Set the list of scopes granted to the client application.
            ticket.SetScopes(new[]
            {
                OpenIdConnectConstants.Scopes.OpenId,
                OpenIdConnectConstants.Scopes.Email,
                OpenIdConnectConstants.Scopes.Profile,
                OpenIddictConstants.Scopes.Roles
            }.Intersect(request.GetScopes()));

            ticket.SetResources("resource-server");

            foreach (var claim in ticket.Principal.Claims)
            {
                claim.SetDestinations(GetDestinations(claim, ticket));
            }

            return controller.SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
        }
        
        private static IEnumerable<string> GetDestinations(Claim claim, AuthenticationTicket ticket)
        {
            // Note: by default, claims are NOT automatically included in the access and identity tokens.
            // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
            // whether they should be included in access tokens, in identity tokens or in both.

            switch (claim.Type)
            {
                case OpenIdConnectConstants.Claims.Name:
                    yield return OpenIdConnectConstants.Destinations.AccessToken;

                    if (ticket.HasScope(OpenIdConnectConstants.Scopes.Profile))
                        yield return OpenIdConnectConstants.Destinations.IdentityToken;

                    yield break;

                case OpenIdConnectConstants.Claims.Email:
                    yield return OpenIdConnectConstants.Destinations.AccessToken;

                    if (ticket.HasScope(OpenIdConnectConstants.Scopes.Email))
                        yield return OpenIdConnectConstants.Destinations.IdentityToken;

                    yield break;

                case OpenIdConnectConstants.Claims.Role:
                    yield return OpenIdConnectConstants.Destinations.AccessToken;

                    if (ticket.HasScope(OpenIddictConstants.Scopes.Roles))
                        yield return OpenIdConnectConstants.Destinations.IdentityToken;

                    yield break;

                // Never include the security stamp in the access and identity tokens, as it's a secret value.
                case "AspNet.Identity.SecurityStamp": yield break;

                default:
                    yield return OpenIdConnectConstants.Destinations.AccessToken;
                    yield break;
            }
        }

        #endregion


    }
}