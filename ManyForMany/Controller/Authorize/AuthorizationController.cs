/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openIddict/openIddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using AuthorizeTester.Model;
using AuthorizeTester.Model.Error;
using Google.Apis.Auth;
using ManyForMany.Model.Entity.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultiLanguage.Exception;
using MvcHelper;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.Mvc.Internal;
using OpenIddict.Server;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ManyForMany.Controller.User
{
    public class AuthorizationController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly OpenIddictApplicationManager<OpenIddictApplication> _applicationManager;
        private readonly IOptions<IdentityOptions> _IdentityOptions;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthorizationController(
            OpenIddictApplicationManager<OpenIddictApplication> applicationManager,
            IOptions<IdentityOptions> IdentityOptions,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _applicationManager = applicationManager;
            _IdentityOptions = IdentityOptions;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet(AuthorizationHelper.AbsolutePath + AuthorizationHelper.AuthorizeEndPoint)]
        public async Task Authorize([ModelBinder(typeof(OpenIddictMvcBinder))]
            OpenIdConnectRequest request)
        {
            // Retrieve the application details from the database.
            var application =
                await _applicationManager.FindByClientIdAsync(request.ClientId, HttpContext.RequestAborted);
            if (application == null)
                throw new MultiLanguageException(nameof(request.ClientId), OpenIdConnectConstants.Errors.InvalidClient);
        }
        
        [HttpPost(AuthorizationHelper.AbsolutePath + AuthorizationHelper.TokenEndPoint)]
        [Produces(Produces.Json)]
        public async Task<IActionResult> Exchange([ModelBinder(typeof(OpenIddictMvcBinder))]
            OpenIdConnectRequest request)
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

                    if (user != null)
                    {
                        if (!await _signInManager.CanSignInAsync(user))
                            throw new MultiLanguageException(nameof(user), Error.NotAllowedToSignIn);
                        // Create a new authentication ticket and return sign in
                        var ticket = await CreateTicketAsync(request.GetScopes(), user);
                        return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
                    }
                    else
                    {
                        // Create a new ClaimsIdentity containing the claims that
                        // will be used to create an id_token and/or an access token.
                        var identity = new ClaimsIdentity(
                            OpenIdConnectServerDefaults.AuthenticationScheme,
                            OpenIdConnectConstants.Claims.Name,
                            OpenIdConnectConstants.Claims.Role);

                        // Note: the "sub" claim is mandatory and an
                        // exception is thrown if this claim is missing.
                        
                        identity.AddClaim(new Claim(OpenIdConnectConstants.Claims.Subject, payload.Subject)
                                .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken,
                                    OpenIdConnectConstants.Destinations.IdentityToken));

                        identity.AddClaim(
                            new Claim(OpenIdConnectConstants.Claims.Name, payload.Name)
                                .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken,
                                    OpenIdConnectConstants.Destinations.IdentityToken));

                        identity.AddClaim(
                            new Claim(OpenIdConnectConstants.Claims.Email, payload.Email)
                                .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken,
                                    OpenIdConnectConstants.Destinations.IdentityToken));
                        
                        // Create a new authentication ticket holding the user identity.
                        var ticket = new AuthenticationTicket(
                            new ClaimsPrincipal(identity),
                            new AuthenticationProperties(),
                            OpenIdConnectServerDefaults.AuthenticationScheme);

                        ticket.SetScopes(
                            OpenIdConnectConstants.Scopes.OpenId,
                            OpenIdConnectConstants.Scopes.OfflineAccess);

                        return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
                    }
                }
                catch (InvalidJwtException)
                {
                    throw new MultiLanguageException(nameof(request.Token), OpenIdConnectConstants.Errors.AccessDenied);
                }
            }

            throw new MultiLanguageException(nameof(request.GrantType),
                OpenIdConnectConstants.Errors.UnsupportedGrantType);
        }

        private async Task<AuthenticationTicket> CreateTicketAsync(IEnumerable<string> scopes, ApplicationUser user)
        {
            // Create a new ClaimsPrincipal containing the claims that
            // will be used to create an Id_token, a token or a code.
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            // Create a new authentication ticket holding the user Identity.
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
            }.Intersect(scopes));

            ticket.SetResources("resource-server");

            // Note: by default, claims are NOT automatically included in the access and Identity tokens.
            // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
            // whether they should be included in access tokens, in Identity tokens or in both.

            foreach (var claim in ticket.Principal.Claims)
            {
                // Never include the security stamp in the access and Identity tokens, as it's a secret value.
                if (claim.Type == _IdentityOptions.Value.ClaimsIdentity.SecurityStampClaimType) continue;

                var destinations = new List<string>
                {
                    OpenIdConnectConstants.Destinations.AccessToken
                };

                // Only add the iterated claim to the Id_token if the corresponding scope was granted to the client application.
                // The other claims will only be added to the access_token, which is encrypted when using the default format.
                if (claim.Type == OpenIdConnectConstants.Claims.Name &&
                    ticket.HasScope(OpenIdConnectConstants.Scopes.Profile) ||
                    claim.Type == OpenIdConnectConstants.Claims.Email &&
                    ticket.HasScope(OpenIdConnectConstants.Scopes.Email) ||
                    claim.Type == OpenIdConnectConstants.Claims.Role &&
                    ticket.HasScope(OpenIddictConstants.Claims.Roles))
                    destinations.Add(OpenIdConnectConstants.Destinations.IdentityToken);

                claim.SetDestinations(destinations);
            }

            return ticket;
        }
    }

    public class Authorizer
    {
        public string GrantType { get; set; }

        public string Token { get; set; }

        public string[] Scopes { get; set; }
        public static Authorizer Create(HttpContext httpContext)
        {
            var header = httpContext.Request.Form;

            return new Authorizer()
            {
                GrantType = header[nameof(GrantType)],
                Token = header[nameof(Token)],
                Scopes = header[nameof(Scopes)]
            };
        }
    }
}