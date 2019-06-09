using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using AuthorizeTester.Model;
using AuthorizeTester.Model.Error;
using ManyForMany.Model.Entity.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiLanguage.Exception;
using Newtonsoft.Json.Linq;
using OpenIddict.Abstractions;

namespace ManyForMany.Controller.User
{
    public class UserinfoController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserinfoController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        //[Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
        [HttpGet(AuthorizationHelper.AbsolutePath + AuthorizationHelper.UserInfoEndPoint)]
        //[Produces(ControllerHelper.JsonFormat)]
        public async Task<JsonResult> UserInfo()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) throw new MultiLanguageException(nameof(User), Error.ElementDoseNotExist);

            var claims = new JObject
            {
                [OpenIdConnectConstants.Claims.Subject] = await _userManager.GetUserIdAsync(user)
            };
            claims.Add(user);
            claims.Add(user);
            claims.Add(user);

            // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.

            if (User.HasClaim(OpenIdConnectConstants.Claims.Scope, OpenIdConnectConstants.Scopes.Email))
            {
                claims[OpenIdConnectConstants.Claims.Email] = await _userManager.GetEmailAsync(user);
                claims[OpenIdConnectConstants.Claims.EmailVerified] = await _userManager.IsEmailConfirmedAsync(user);
            }

            if (User.HasClaim(OpenIdConnectConstants.Claims.Scope, OpenIdConnectConstants.Scopes.Phone))
            {
                claims[OpenIdConnectConstants.Claims.PhoneNumber] = await _userManager.GetPhoneNumberAsync(user);
                claims[OpenIdConnectConstants.Claims.PhoneNumberVerified] = await _userManager.IsPhoneNumberConfirmedAsync(user);
            }

            if (User.HasClaim(OpenIdConnectConstants.Claims.Scope, OpenIddictConstants.Scopes.Roles))
                claims["roles"] = JArray.FromObject(await _userManager.GetRolesAsync(user));

            // Note: the complete list of standard claims supported by the OpenId Connect specification
            // can be found here: http://openId.net/specs/openId-connect-core-1_0.html#StandardClaims

            return Json(claims);
        }
    }
}