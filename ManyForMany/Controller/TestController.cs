using System.Threading.Tasks;
using AuthorizeTester.Model;
using ManyForMany.Model.Entity;
using ManyForMany.Model.Entity.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ManyForMany.Controller
{
    [ApiController]
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    public class TestController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Context _context;

        public TestController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, Context context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet(nameof(Test))]
        public void Test()
        {
            
        }
        
        [AllowAnonymous]
        [HttpGet(nameof(ConfigureRoles))]
        public async Task ConfigureRoles()
        {
            foreach (var roleName in CustomRoles.All)
            {
                var x = await _roleManager.RoleExistsAsync(roleName);

                if (roleName == CustomRoles.BasicUser)
                {
                    // first we create Admin rool    


                    //Here we create a Admin super user who will maintain the website                   

                    var user = await _userManager.GetUserAsync(User);

                    var result1 = await _userManager.AddToRoleAsync(user, CustomRoles.BasicUser);

                    _context.SaveChanges();
                }


                if (!x)
                {
                    var role = new IdentityRole
                    {
                        Name = roleName
                    };
                    await _roleManager.CreateAsync(role);

                    _context.SaveChanges();

                    if (roleName == CustomRoles.BasicUser)
                    {
                        // first we create Admin rool    
                        

                        //Here we create a Admin super user who will maintain the website                   

                        var user = await _userManager.GetUserAsync(User);

                        var result1 = await _userManager.AddToRoleAsync(user, CustomRoles.BasicUser);

                        _context.SaveChanges();
                    }


                }
            }
        }
    }
}