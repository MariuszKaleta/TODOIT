using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Tests.Subscription;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.Validation;
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
            }, ServiceLifetime.Transient);

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

            // app.UseWelcomePage();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors(MyAllowSpecificOrigins);


            app.UseMvcWithDefaultRoute();


            /*
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>($"{MvcHelper.AttributeHelper.AddressPathSeparator}{nameof(ChatHub)}",
                    x => { });
            });
            */
            //InitializeAsync(app.ApplicationServices).GetAwaiter().GetResult();
            await app.ApplicationServices.InitializeRolesAsync();
        }

       
        /*
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

        */
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

                    // Enable the password flow.
                    options.AllowPasswordFlow();

                    // Accept anonymous clients (i.e clients that don't send a client_id).
                    options.AcceptAnonymousClients();

                    // During development, you can disable the HTTPS requirement.
                    options.DisableHttpsRequirement();

                    // Note: to use JWT access tokens instead of the default
                    // encrypted format, the following lines are required:
                    //
                    // options.UseJsonWebTokens();
                    // options.AddEphemeralSigningKey();
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
            services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            services.AddSingleton<IDocumentExecuter, EfDocumentExecuter>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<IOpinionRepository, OpinionRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            

            //services.AddScoped<AppQuery>();
            // services.AddScoped<AppMutation>();
            //services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddScoped<AppSchema>();

            services.AddGraphQL(o =>
                {
                    o.ExposeExceptions = true;
                    o.EnableMetrics = true;
                })
                .AddGraphTypes(ServiceLifetime.Scoped)
                .AddDataLoader()
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
                 .AddAuthentication(x=>x.DefaultAuthenticateScheme = OpenIddictValidationDefaults.AuthenticationScheme)
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

        public static async Task InitializeClientAsync(this IServiceProvider services, Configuration Config)
        {
            // Create a new service scope to ensure the database context is correctly disposed when this methods returns.
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Context>();
                await context.Database.EnsureCreatedAsync();

                var manager = scope.ServiceProvider.GetRequiredService<OpenIddictApplicationManager<OpenIddictApplication>>();

                var clientId = Config.ClientId;
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
                //CreateSkills(context);
            }

        }
    }
}
