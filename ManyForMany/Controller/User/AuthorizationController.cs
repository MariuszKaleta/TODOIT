/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using Google.Apis.Auth;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity.User;
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
using AuthenticationProperties = Microsoft.AspNetCore.Authentication.AuthenticationProperties;

namespace ManyForMany.Controller.User
{
    public class AuthorizationController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthorizationController(
            IOptions<IdentityOptions> identityOptions,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _identityOptions = identityOptions;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost(AuthorizationHelper.AbsolutePath + AuthorizationHelper.TokenEndPoint)]
        [Produces(Produces.Json)]
        public async Task<IActionResult> Exchange([ModelBinder(typeof(OpenIddictMvcBinder))] OpenIdConnectRequest request)
        {
            if (request.GrantType == CustomGrantTypes.Google)
            {
                // Reject the request if the "assertion" parameter is missing.
                if (string.IsNullOrEmpty(request.Token))
                    throw new MultiLanguageException(nameof(request.Token), Error.ElementDoseNotExist);

                try
                {
                    // Validate the access token using Google's token validation
                    var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token);

                    var user = await _userManager.FindByEmailAsync(payload.Email);

                    if (user == null)
                    {
                        user = await Register(payload);
                    }

                    if (!await _signInManager.CanSignInAsync(user))
                        throw new MultiLanguageException(nameof(user), Error.NotAllowedToSignIn);
                    // Create a new authentication ticket and return sign in
                    var ticket = await CreateTicketAsync(request, user);

                    return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
                }
                catch (InvalidJwtException)
                {
                    throw new MultiLanguageException(nameof(request.Token), OpenIdConnectConstants.Errors.AccessDenied);
                }
            }

            throw new MultiLanguageException(nameof(request.GrantType),
                OpenIdConnectConstants.Errors.UnsupportedGrantType);
        }

        [AllowAnonymous]
        [MvcHelper.Attributes.HttpGet(nameof(GrantTypes))]
        public string[] GrantTypes()
        {
            return CustomGrantTypes.All.ToArray();
        }



        #region Helpers

        private async Task<AuthenticationTicket> CreateTicketAsync(OpenIdConnectRequest request, ApplicationUser user)
        {
            // Create a new ClaimsPrincipal containing the claims that
            // will be used to create an id_token, a token or a code.
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            // Create a new authentication ticket holding the user identity.
            var ticket = new AuthenticationTicket(principal,
                new AuthenticationProperties(),
                OpenIddictServerDefaults.AuthenticationScheme);

            // Set the list of scopes granted to the client application.
            ticket.SetScopes(new[]
            {
                                OpenIddictConstants.Scopes.Email,
                OpenIddictConstants.Scopes.OfflineAccess,
                OpenIddictConstants.Scopes.OpenId,
                OpenIddictConstants.Scopes.Address,
                OpenIddictConstants.Scopes.Phone,
                OpenIddictConstants.Scopes.Profile,
                OpenIddictConstants.Scopes.Roles,

            }.Intersect(request.GetScopes()));

            ticket.SetResources("resource-server");

            // Note: by default, claims are NOT automatically included in the access and identity tokens.
            // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
            // whether they should be included in access tokens, in identity tokens or in both.

            foreach (var claim in ticket.Principal.Claims)
            {
                // Never include the security stamp in the access and identity tokens, as it's a secret value.
                if (claim.Type == _identityOptions.Value.ClaimsIdentity.SecurityStampClaimType)
                {
                    continue;
                }

                var destinations = new List<string>
                {
                    OpenIdConnectConstants.Destinations.AccessToken
                };

                // Only add the iterated claim to the id_token if the corresponding scope was granted to the client application.
                // The other claims will only be added to the access_token, which is encrypted when using the default format.
                if ((claim.Type == OpenIdConnectConstants.Claims.Name && ticket.HasScope(OpenIdConnectConstants.Scopes.Profile)) ||
                    (claim.Type == OpenIdConnectConstants.Claims.Email && ticket.HasScope(OpenIdConnectConstants.Scopes.Email)) ||
                    (claim.Type == OpenIdConnectConstants.Claims.Role && ticket.HasScope(OpenIddictConstants.Claims.Roles)))
                {
                    destinations.Add(OpenIdConnectConstants.Destinations.IdentityToken);
                }

                claim.SetDestinations(destinations);
            }

            return ticket;
        }

        private async Task<ApplicationUser> Register(GoogleJsonWebSignature.Payload payload)
        {
            var user = new ApplicationUser(payload);

            await Register(user);

            return user;
        }

        private async Task Register(ApplicationUser user)
        {
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

        #endregion

    }
}