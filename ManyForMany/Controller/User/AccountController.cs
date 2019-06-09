using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizeTester.Model;
using AuthorizeTester.Model.Error;
using AuthorizeTester.ViewModel.User;
using ManyForMany.Model.Entity;
using ManyForMany.Model.Entity.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiLanguage.Exception;
using MvcHelper;

namespace ManyForMany.Controller.User
{
    [ApiController]
    [Authorize]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class AccountController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Context _context;

        public AccountController(UserManager<ApplicationUser> userManager, Context context, SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _roleManager = roleManager;

        }


        [HttpGet(nameof(UserInfo))]
        public async Task<object> UserInfo()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                return new
                {
                    User = user,
                    Roles = await _userManager.GetRolesAsync(user)
                };
            }

            return false;
        }

        [HttpPost(nameof(LogOff))]
        public async Task LogOff()
        {
            await _signInManager.SignOutAsync();
        }

        [HttpGet(nameof(GetAllUsers))]
        [Authorize(Roles = CustomRoles.Admin)]
        public ApplicationUser[] GetAllUsers()
        {
            return _userManager.Users.ToArray();
        }

        #region ExternalLogin

        //
        // POST: /Account/ExternalLogin
        [MvcHelper.Attributes.HttpGet(nameof(ExternalLogin), "{provider}")]
        [AllowAnonymous]
        public ChallengeResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), nameof(AccountController).GetControllerName(),
                new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        // GET: /Account/ExternalLoginCallback
        [HttpGet(nameof(ExternalLoginCallback))]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null) throw new MultiLanguageException(nameof(info), Error.UserNotLogged);

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

            if (result.Succeeded)
            {
                return string.IsNullOrEmpty(returnUrl) ? Redirect(nameof(UserInfo)) : Redirect(returnUrl);
            }

            // If the user does not have an account, then ask the user to create an account.
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            await ExternalLoginConfirmation(new ExternalLoginConfirmationViewModel { Email = email });


            return Redirect(nameof(UserInfo));
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost(nameof(ExternalLoginConfirmation))]
        [AllowAnonymous]
        public async Task ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();

                if (info == null) throw new MultiLanguageException(nameof(info), Error.UserNotLogged);

                var user = new ApplicationUser(_userManager,_context)
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return;
                    }
                }

                AddErrors(result);
            }
        }

        [AllowAnonymous]
        [HttpGet(nameof(Providers))]
        public async Task<string[]> Providers()
        {
            return (await _signInManager.GetExternalAuthenticationSchemesAsync()).Select(x => x.DisplayName).ToArray();
        }

        #endregion
        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        }

        #endregion
    }
}