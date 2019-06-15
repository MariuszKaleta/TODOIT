using System;
using System.IO;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using AuthorizeTester.Model;
using ManyForMany.Model.Entity;
using ManyForMany.Model.Entity.User;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace ManyForMany
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Authentication = Configuration.GetSection(nameof(Authentication));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<Context>(options =>
            {
                string defaultConnection = null;

                defaultConnection = Configuration.GetConnectionString(nameof(defaultConnection));

                // Configure the context to use Microsoft SQL Server.

                options.UseSqlServer(
                    defaultConnection
                    //Configuration.GetConnectionString("LocalConnection")
                );

                options.UseOpenIddict();
            });

            OpenIddict(services);
            Swagger(services);

            //services.AddMultiLanguageValidation();
        }

        private void OpenIddict(IServiceCollection services)
        {
            // Register the Identity services.
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores< Context>()
                .AddDefaultTokenProviders();

            // Configure Identity to use the same JWT claims as OpenIddict instead
            // of the legacy WS-Federation claims it uses by default (ClaimTypes),
            // which saves you from doing the mapping in your authorization controller.
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });

            services.AddOpenIddict()


                // Register the OpenIddict core services.
                .AddCore(options =>
                {
                    // Register the Entity Framework stores and models.
                    options.UseEntityFrameworkCore()
                        .UseDbContext<Context>();
                })

                // Register the OpenIddict server handler.
                .AddServer(options =>
                {
                    // Register the ASP.NET Core MVC binder used by OpenIddict.
                    // Note: if you don't call this method, you won't be able to
                    // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
                    options.UseMvc();


                    // Enable the authorization, logout, userinfo, and introspection endpoints.
                    options
                        .EnableAuthorizationEndpoint(AuthorizationHelper.AuthorizeEndPoint)
                        .EnableLogoutEndpoint(AuthorizationHelper.LogoutEndPoint)
                        .EnableTokenEndpoint(AuthorizationHelper.TokenEndPoint)
                        .EnableUserinfoEndpoint(AuthorizationHelper.UserInfoEndPoint)
                        ;


                    options.RegisterScopes(
                        OpenIdConnectConstants.Scopes.Email,
                        OpenIdConnectConstants.Scopes.Profile,
                        OpenIddictConstants.Scopes.Roles
                        
                    );

                    options.AllowAuthorizationCodeFlow();
                    options.EnableRequestCaching();
                    options.UseJsonWebTokens();
                    options.AddEphemeralSigningKey();

                    options.AllowClientCredentialsFlow();
                    options.AllowRefreshTokenFlow();
                    options.AddCustomGrantTypes();
                    options.DisableHttpsRequirement();
                    options.AllowImplicitFlow();

                    options.IgnoreScopePermissions();
                    options.IgnoreGrantTypePermissions();
                    options.IgnoreEndpointPermissions();
                })
                .AddValidation()
                ;


            IConfigurationSection google;
            google = Authentication.GetSection(nameof(google));

            var googleId = google.GetSection(nameof(GoogleOptions.ClientId)).Value;
            var googleSecret = google.GetSection(nameof(GoogleOptions.ClientSecret)).Value;

            services
                .AddAuthentication()
                .AddGoogle(o =>
                    {
                        o.ClientId = googleId;
                        o.ClientSecret = googleSecret;
                    }
                )
                .AddFacebook(o =>
                    {
                        IConfigurationSection facebook;
                        facebook = Authentication.GetSection(nameof(facebook));

                        o.AppId = facebook.GetSection(nameof(o.AppId)).Value;
                        o.AppSecret = facebook.GetSection(nameof(o.AppSecret)).Value;
                    }
                )
                .AddOpenIdConnect(x=>
                    {
                        x.ClientId = googleId;
                        x.ClientSecret = googleSecret;
                        x.Authority = "https://accounts.google.com";
                        x.ResponseType = OpenIdConnectResponseType.Code;
                        x.GetClaimsFromUserInfoEndpoint = true;
                        x.SaveTokens = true;
                        x.Events = new OpenIdConnectEvents()
                        {
                            OnRedirectToIdentityProvider = (context) =>
                            {
                                if (context.Request.Path != "/account/external")
                                {
                                    context.Response.Redirect("/account/login");
                                    context.HandleResponse();
                                }

                                return Task.FromResult(0);
                            }
                        };
                    }
                    )
                ;

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

            app.UseMvc();

            //app.UseWelcomePage();
            Swagger(app);

            // app.AddMultiLanguageExceptionHandler().
            //    AddErrorEnum<GeoCoderStatusCode>().
            //   AddErrorEnum<Error>().
            //  AddErrorClassWithConstProperties(typeof(OpenIddictConstants.Errors));
            //  AddErrorClassWithConstProperties(typeof(OpenIdConnectConstants.Errors));

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

        #region Propertis

        public static IConfiguration Configuration { get; private set; }

        public static IConfigurationSection Authentication { get; private set; }

        #endregion

        private async Task InitializeAsync(IServiceProvider services)
        {
            // Create a new service scope to ensure the database context is correctly disposed when this methods returns.
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Context>();
                await context.Database.EnsureCreatedAsync();

                
                var manager = scope.ServiceProvider.GetRequiredService<OpenIddictApplicationManager<OpenIddictApplication>>();

                var clientId = Configuration.GetSection(nameof(OpenIddictApplicationDescriptor.ClientId)).Get<string>();

                var client = await manager.FindByClientIdAsync(clientId);
                await manager.DeleteAsync(client);
                client = await manager.FindByClientIdAsync(clientId);
                
                if (client  == null)
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