using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using AuthorizationServer.ViewModels.Account;
using AuthorizeTester.Model;
using ManyForMany.Models.Configuration;
using ManyForMany.ViewModel.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiLanguage.Exception;
using MvcHelper;
using OpenIddict.Validation;

namespace AuthorizationServer.Controllers
{
    [Authorize]
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

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            EnsureDatabaseCreated(_context);
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    return StatusCode(StatusCodes.Status409Conflict);
                }

                user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, CustomRoles.BasicUser).GetAwaiter().GetResult();
                    _context.SaveChanges();

                    return Ok();
                }
                AddErrors(result);
            }

            // If we got this far, something failed.
            return BadRequest(ModelState);
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
                return string.IsNullOrEmpty(returnUrl) ? Redirect("api/message") : Redirect(returnUrl);
            }

            // If the user does not have an account, then ask the user to create an account.
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            await ExternalLoginConfirmation(new ExternalLoginConfirmationViewModel { Email = email });


            return Redirect("api/message");
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

                var user = new ApplicationUser()
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, CustomRoles.BasicUser).GetAwaiter().GetResult();
                    _context.SaveChanges();


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
