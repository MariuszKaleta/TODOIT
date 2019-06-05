using System.Threading.Tasks;
using AuthorizeTester.Model;
using ManyForMany.Model.Entity.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ManyForMany.Controller
{
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class TestController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public TestController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpGet(nameof(AddToAdminRole))]
        public async Task AddToAdminRole()
        {
            var x = await _roleManager.RoleExistsAsync(CustomRoles.Admin);
            if (!x)
            {
                // first we create Admin rool    
                var role = new IdentityRole
                {
                    Name = CustomRoles.Admin
                };
                await _roleManager.CreateAsync(role);

                //Here we create a Admin super user who will maintain the website                   

                var user = await _userManager.GetUserAsync(User);

                var result1 = await _userManager.AddToRoleAsync(user, CustomRoles.Admin);
            }
        }
    }
}