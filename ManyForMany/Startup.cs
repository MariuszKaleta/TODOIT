using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Execution;
using GraphQL.Http;
using GraphQL.Language.AST;
using GraphQL.Server;
using GraphQL.Server.Authorization.AspNetCore;
using GraphQL.Server.Internal;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Server.Ui.Playground;
using GraphQL.Tests.Subscription;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.Validation;
using TODOIT.GraphQl;
using TODOIT.GraphQl.Queries;
using TODOIT.GraphQl.Schema;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.Chat;
using TODOIT.Model.Entity.Skill;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories;
using TODOIT.Repositories.Contracts;

namespace TODOIT
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            configuration.Bind(Config);
        }

        public static readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Configuration Config { get; set; } = new Configuration();

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore(x => x.EnableEndpointRouting = false)
                .AddApiExplorer()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddCors(
                    options =>
                    {
                        options.AddPolicy(MyAllowSpecificOrigins,
                            builder =>
                            {

                                builder
                                    .AllowAnyOrigin()
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    ;
                            });
                    })
                ;

            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
            // If using IIS:
            services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });

            services.AddDbContext<Context>(options =>
            {
                options.UseSqlServer(Config.ConnectionStrings.DefaultConnection);

                options.UseOpenIddict();
            }, ServiceLifetime.Transient, ServiceLifetime.Transient);

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<Context>()
                .AddDefaultTokenProviders();


            services.OpenIddict();
            services.Authenticate(Config);
            services.Swagger();
            services.GraphQl();
            services.Chat();
        }

        public async void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            /*
            var a =
            {
                AuthenticationScheme = "LinkedIn",
                CallbackPath = new PathString("/signin-linkedin"),
                ClientId = Configuration["Authentication:LinkedIn:ClientId"],
                ClientSecret = Configuration["Authentication:LinkedIn:ClientSecret"],
                Scope = {LinkedInScopes.BasicProfile, LinkedInScopes.EmailAddress}
            };
           
            app.UseLinkedInAccountAuthentication(new LinkedInAccountOptions(LinkedInFields.All)
            {
                ClientId = Config.Authentication.LinkedIn.ClientId,
                ClientSecret = Config.Authentication.LinkedIn.ClientSecret,
            });

             */

            //app.UseBrowserLink();

            app.Swagger();
            app.GraphQl();
            app.Chat();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseMvcWithDefaultRoute();

            await app.ApplicationServices.InitializeRolesAsync();
        }
    }

    public static class StartupExtension
    {
        public static void OpenIddict(this IServiceCollection services)
        {
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
                    // Enable the token endpoint.
                    options.EnableTokenEndpoint(AuthorizationHelper.TokenEndPoint);

                    options.AddCustomGrantTypes();

                    // Accept anonymous clients (i.e clients that don't send a client_id).
                    options.AcceptAnonymousClients();

                    // During development, you can disable the HTTPS requirement.
                    options.DisableHttpsRequirement();

                })

                // Register the OpenIddict validation handler.
                // Note: the OpenIddict validation handler is only compatible with the
                // default token format or with reference tokens and cannot be used with
                // JWT tokens. For JWT tokens, use the Microsoft JWT bearer handler.
                .AddValidation();
        }

        public static void Swagger(this IServiceCollection services)
        {
            //swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "API",
                    Description = "Test API with ASP.NET Core 3.0"
                });

                var xmlPath = Path.Combine(AppContext.BaseDirectory, SwaggerHelper.XmlPath);
                c.IncludeXmlComments(xmlPath);
            });
        }

        public static void GraphQl(this IServiceCollection services)
        {
            //services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            services.AddSingleton<IDocumentExecuter, EfDocumentExecuter>();

            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ISkillRepository, SkillRepository>();
            services.AddTransient<IOpinionRepository, OpinionRepository>();
            services.AddTransient<IChatRepository, ChatRepository>();
            services.AddTransient<IMessageRepository, MessageRepository>();
            services.AddTransient<ISchema, AppSchema>();

            services.AddSingleton<AppSchema>();

            services
                .AddGraphQL()
                .AddUserContextBuilder(ctx =>
                {
                    var result = ctx.AuthenticateAsync(OpenIddictValidationDefaults.AuthenticationScheme).Result;

                    if (result.Succeeded)
                    {
                        return result.Principal.Claims.ToDictionary(x => x.Type, x => (object)x.Value);
                    }

                    return null;
                })

                .AddGraphQLAuthorization(x =>
                {
                    x.AddPolicy(Startup.MyAllowSpecificOrigins, x =>
                    {
                        x.AddAuthenticationSchemes(OpenIddictValidationDefaults.AuthenticationScheme);
                        //x.RequireAuthenticatedUser()
                        x.RequireClaim(OpenIddictConstants.Claims.Subject);
                    });
                })

                .AddDataLoader()
                .AddGraphTypes(ServiceLifetime.Transient)

                ;

        }

        public static void Chat(this IServiceCollection app)
        {
            app.AddSignalR();
        }

        public static void Authenticate(this IServiceCollection services, Configuration Config)
        {

            var scopes = new string[]
            {
                //OpenIddictConstants.Scopes.Phone
            };


            services
                 .AddAuthentication(x =>
                 {

                     x.DefaultAuthenticateScheme = OpenIddictValidationDefaults.AuthenticationScheme;

                 })
                 /*     .AddGoogle(options =>
                         {
        
                             options.ClientId =  Config.Authentication.Google.ClientId;
                             options.ClientSecret = Config.Authentication.Google.ClientSecret;
                             foreach (var scope in scopes)
                             {
                                 options.Scope.Add(scope);
                             }
                         })
                         .AddFacebook(options =>
                         {
                             options.ClientId = Config.Authentication.Facebook.AppId;
                             options.ClientSecret = Config.Authentication.Facebook.AppSecret;
                             foreach (var scope in scopes)
                             {
                                 options.Scope.Add(scope);
                             }
                         })
                         .AddLinkedIn(options =>
                         {
                             options.ClientId = Config.Authentication.LinkedIn.ClientId;
                             options.ClientSecret = Config.Authentication.LinkedIn.ClientSecret;
                         })
                    .AddOAuthValidation()*/
                 ;
            /*
            services.AddAuthorization(x =>
            {
                x.AddPolicy(Startup.MyAllowSpecificOrigins, x=>
                {
                    x.AuthenticationSchemes.Add(OpenIddictValidationDefaults.AuthenticationScheme);
                    x.RequireAuthenticatedUser();
                });
            });
            */
        }

        public static void Swagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(SwaggerHelper.JsonPath, "Test API V1");
            });
        }

        public static void GraphQl(this IApplicationBuilder app)
        {
            /*
            app.UseMiddleware<GraphQLMiddleware>(new GraphQLSettings
            {
                BuildUserContext = ctx =>
                {
                    var result = ctx.AuthenticateAsync(OpenIddictValidationDefaults.AuthenticationScheme).Result;

                    if (result.Succeeded)
                    {
                        return result.Principal.Claims.ToDictionary(x => x.Type, x => (object)x.Value);
                    }

                    return null;
                },
                EnableMetrics = true,
                ExposeExceptions = true   
            });

    */
            app.UseGraphQL<AppSchema>();
            app.UseGraphQLPlayground(options: new GraphQLPlaygroundOptions());
        }

        public static void Chat(this IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(x =>
            {
                x.MapHub<ChatHub>("/chat");
            });
        }
        public static IGraphQLBuilder AddGraphQLAuthorization(
            this IGraphQLBuilder builder,
            Action<AuthorizationOptions> options)
        {
            builder.Services.AddHttpContextAccessor().AddTransient<IValidationRule, AuthorizationValidationRule>().AddAuthorization(options);

            return builder;
        }

        public static async Task InitializeRolesAsync(this IServiceProvider services)
        {
            using var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in CustomRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
