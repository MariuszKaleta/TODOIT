using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Authorization.AspNetCore;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Server.Transports.AspNetCore.Common;
using GraphQL.Server.Transports.AspNetCore.SystemTextJson;
using GraphQL.Server.Ui.Altair;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQlHelper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using OpenIddict.Validation;
using TODOIT.GraphQl.Schema;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.Chat;
using TODOIT.Repositories;
using TODOIT.Repositories.Contracts;

namespace TODOIT
{
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
                  //  options
                  //      .EnableTokenEndpoint(AuthorizationHelper.TokenEndPoint);
                     //   .EnableAuthorizationEndpoint("/connect/authorize");

                  //  options.EnableTokenEndpoint(AuthorizationHelper.ExternalLogin);

                      options.AddCustomGrantTypes();
                  //  options.AllowAuthorizationCodeFlow();

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
            services.AddTransient<IGraphQLRequestDeserializer, GraphQLRequestDeserializer>();

            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IOrderMembersRepository, OrderMembersRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ISkillRepository, SkillRepository>();
            services.AddTransient<IOpinionRepository, OpinionRepository>();
            services.AddTransient<IChatRepository, ChatRepository>();
            services.AddTransient<IMessageRepository, MessageRepository>();
            services.AddTransient<ISchema, AppSchema>();
            services.AddSingleton<AppSchema>();

            services
                .AddGraphQL(x =>
                {
                    x.ExposeExceptions = true;
                    x.EnableMetrics = true;
                })
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
            services
                .AddAuthentication(x =>
                {
                    //todo
                 //   x.DefaultAuthenticateScheme = OpenIddictValidationDefaults.AuthenticationScheme;
                })
                ;

            services.AddAuthorization(x =>
            {
                x.AddPolicy(Startup.MyAllowSpecificOrigins, x =>
                {
                    x.AuthenticationSchemes.Add(OpenIddictValidationDefaults.AuthenticationScheme);
                    x.RequireAuthenticatedUser();
                });
            });

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = Config.Authentication.Google.ClientId;
                    options.ClientSecret = Config.Authentication.Google.ClientSecret;
                });
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
            app.UseGraphQL<AppSchema>();
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions()
            {
                Path = "/ui/Playground",
                GraphQLEndPoint = "/graphql",
            });

            app.UseGraphQLAltair(new GraphQLAltairOptions
            {
                Path = "/ui/altair",
                GraphQLEndPoint = "/graphql",
            });
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