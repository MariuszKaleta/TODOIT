using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using AuthorizationServer.Models;
using AuthorizeTester.Model;
using ManyForMany.Models.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.Validation;
using Swashbuckle.AspNetCore.Swagger;

namespace AuthorizationServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            configuration.Bind(Configuration);
        }

        public Configuration Configuration { get; set; } = new Configuration();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc();

            services.AddDbContext<Context>(options =>
            {
                options.UseSqlServer(Configuration.ConnectionStrings.DefaultConnection);
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

                    options.AllowPasswordFlow();
                    options.AcceptAnonymousClients();
                    options.DisableHttpsRequirement();
                    options.AddCustomGrantTypes();
                })
                .AddValidation();

            services
                .AddAuthentication()
                .AddGoogle(o =>
                    {
                        o.ClientId = Configuration.Authentication.Google.ClientId;
                        o.ClientSecret = Configuration.Authentication.Google.ClientSecret;
                    }
                )
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

                var clientId = Configuration.ClientId;
                var client = await manager.FindByClientIdAsync(clientId);

                if (client == null)
                {
                    var descriptor = new OpenIddictApplicationDescriptor
                    {
                        ClientId = clientId,
                        Permissions =
                        {
                            OpenIddictConstants.Permissions.Endpoints.Authorization,
                            OpenIddictConstants.Permissions.Endpoints.Logout,
                            OpenIddictConstants.Permissions.Endpoints.Token,

                            OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                            OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                            OpenIddictConstants.Permissions.GrantTypes.Implicit,
                            OpenIddictConstants.Permissions.GrantTypes.Password,
                            OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                            OpenIddictConstants.Permissions.Scopes.Email,

                            CustomGrantTypes.Google.ToPermission()
                        },
                    };

                    await manager.CreateAsync(descriptor);
                }
            }
        }

    }
}
