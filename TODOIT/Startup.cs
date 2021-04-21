using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using GraphQL.DataLoader;
using GraphQL.Execution;
using GraphQL.Language.AST;
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
using Newtonsoft.Json;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using TODOIT.GraphQl;
using TODOIT.GraphQl.Queries;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity;
using TODOIT.Model.Entity.Skill;
using TODOIT.Model.Entity.User;

namespace TODOIT
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            configuration.Bind(Config);
        }

        public static readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public static Configuration Config { get; set; } = new Configuration();

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
                                    .WithOrigins("http://192.168.1.115:3000 ")
                                    //.AllowAnyOrigin()
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials()
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
}
