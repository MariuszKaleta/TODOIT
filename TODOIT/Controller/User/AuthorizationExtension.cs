using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiLanguage.Exception;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity.User;
using TODOIT.ViewModel.User;

namespace TODOIT.Controller.User
{
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


        public static async Task<AuthenticationTicket> CreateSpecifiedTicketAsync(this  ClaimsPrincipal principal, OpenIdConnectRequest request, AuthenticationProperties properties = null)
        {

            // Create a new authentication ticket holding the user identity.
            var ticket = new AuthenticationTicket(principal, properties,
                OpenIdConnectServerDefaults.AuthenticationScheme);

            if (!request.IsRefreshTokenGrantType())
            {
                //TODO : // Include resources and scopes, **as APPROPRIATE**
                // Set the list of scopes granted to the client application.
                // Note: the offline_access scope must be granted
                // to allow OpenIddict to return a refresh token.
                ticket.SetScopes(new[]
                {
                    /* openid: */ OpenIdConnectConstants.Scopes.OpenId,
                    /* email: */ OpenIdConnectConstants.Scopes.Email,
                    /* profile: */ OpenIdConnectConstants.Scopes.Profile,
                    /* offline_access: */ OpenIdConnectConstants.Scopes.OfflineAccess,
                    /* roles: */ OpenIddictConstants.Scopes.Roles
                }.Intersect(request.GetScopes()));
            }
            return ticket;
        }



        public static IEnumerable<string> GetDestinations(Claim claim, AuthenticationTicket ticket)
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