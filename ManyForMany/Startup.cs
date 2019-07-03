using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using AuthorizationServer.Models;
using AuthorizeTester.Model;
using ManyForMany.Models.Configuration;
using ManyForMany.Models.Entity.Order;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.Validation;
using Swashbuckle.AspNetCore.Swagger;
using SignalRChat;

namespace AuthorizationServer
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            configuration.Bind(Configur);
        }

        public Configuration Configur { get; set; } = new Configuration();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy(Configur.Policy, builder =>
            {
                builder
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
            services.AddSignalR(x =>
            {
                x.EnableDetailedErrors = true;
            });

            services.AddCors();
            services.AddMvc();

            services.AddDbContext<Context>(options =>
            {
                options.UseSqlServer(Configur.ConnectionStrings.DefaultConnection);
                options.UseOpenIddict();
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<Context>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });

            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                           .UseDbContext<Context>();
                })

                .AddServer(options =>
                {
                    options.UseMvc();
                    options.EnableTokenEndpoint(AuthorizationHelper.TokenEndPoint);

                    options.AcceptAnonymousClients();
                    options.DisableHttpsRequirement();
                    options.AddCustomGrantTypes();
                })
                .AddValidation();

            services
                .AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = OpenIddictValidationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = OpenIddictValidationDefaults.AuthenticationScheme;
                    })
                ;

            Swagger(services);
        }

        private void Swagger(IServiceCollection services)
        {
            //swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info()
                {
                    Version = "v1",
                    Title = "API",
                    Description = "Test API with ASP.NET Core 3.0"
                });


                var xmlPath = Path.Combine(AppContext.BaseDirectory, SwaggerHelper.XmlPath);
                c.IncludeXmlComments(xmlPath);
            });
        }


        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();

            Swagger(app);


            app.UseCors(Configur.Policy);

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>($"{MvcHelper.AttributeHelper.AddressPathSeparator}{nameof(ChatHub)}",
                    x => { });
            });

            InitializeAsync(app.ApplicationServices).GetAwaiter().GetResult();
        }

        private void Swagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(SwaggerHelper.JsonPath, "Test API V1");
            });
        }

        private async Task InitializeAsync(IServiceProvider services)
        {
            // Create a new service scope to ensure the database context is correctly disposed when this methods returns.
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Context>();
                await context.Database.EnsureCreatedAsync();


                var manager = scope.ServiceProvider.GetRequiredService<OpenIddictApplicationManager<OpenIddictApplication>>();

                var clientId = Configur.ClientId;
                var client = await manager.FindByClientIdAsync(clientId);

                if (client == null)
                {
                    var descriptor = new OpenIddictApplicationDescriptor
                    {
                        ClientId = clientId,
                        Permissions =
                        {
                            OpenIddictConstants.Permissions.Endpoints.Token,

                            OpenIddictConstants.Permissions.Scopes.Email,
                            OpenIddictConstants.Permissions.Scopes.Roles,

                            CustomGrantTypes.Google.ToPermission()
                        },
                    };

                    await manager.CreateAsync(descriptor);
                }

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                foreach (var role in CustomRoles.All)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }


                //CreateSkills(context);
            }

        }



        private async Task CreateSkills(Context context)
        {
            var rows =
                File.ReadAllLines(@"C:\Users\RWSwiss\source\repos\ManyForMany\ManyForMany\Data\Skills.txt");

            var skills = rows.Select(x => new Skill(x)).ToArray();

            var errors = new List<Skill>();

            foreach (var skill in skills)
            {
                try
                {
                    context.Skills.Add(skill);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    errors.Add(skill);
                   //var a =  context.Skills.Count();
                }
            }


        }
    }
}
