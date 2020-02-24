using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTrackerProject.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using BugTrackerProject.Security;
using NETCore.MailKit.Infrastructure.Internal;
using NETCore.MailKit.Extensions;

namespace BugTrackerProject
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(
            //IConfiguration configuration
            IWebHostEnvironment env
            )
        {
            //Configuration = configuration;
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(env.ContentRootPath);
            builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            // This one needs to be last
            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
                config.EnableEndpointRouting = false;
            }).AddXmlSerializerFormatters();

            var mailKitOptions = Configuration.GetSection("Email").Get<MailKitOptions>();
            services.AddMailKit(options => options.UseMailKit(mailKitOptions));

            //var dbstring = Environment.GetEnvironmentVariable("BugTrackerDBConnection");

            //services.AddMailKit(optionBuilder =>
            //{
            //    optionBuilder.UseMailKit(new MailKitOptions()
            //    {
            //        Server = Environment.GetEnvironmentVariable("SMTPServer"),
            //        Port = Convert.ToInt32(Environment.GetEnvironmentVariable("SMTPPort")),
            //        SenderName = Environment.GetEnvironmentVariable("SMTPSenderName"),
            //        SenderEmail = Environment.GetEnvironmentVariable("SMTPSenderEmail"),
            //        Account = Environment.GetEnvironmentVariable("SMTPAccount"),
            //        Password = Environment.GetEnvironmentVariable("SMTPPassword"),
            //        Security = false
            //    });
            //});

            services.AddDbContextPool<AppDbContext>(
            options => options.UseSqlServer(Configuration.GetConnectionString("BugTrackerDBConnection")));
            //options => options.UseSqlServer(Environment.GetEnvironmentVariable("BugTrackerDBConnection")));
            services.AddControllersWithViews();
            //services.AddSingleton<IBugRepository, MockBugRepository>();
            //services.AddSingleton<IProjectRepository, MockProjectRepository>();

            services.AddScoped<IBugRepository, SqlBugRepository>();
            services.AddScoped<IProjectRepository, SqlProjectRepository>();

            services.AddIdentity<IdentityUser, IdentityRole>(options => {
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;
                options.Password.RequireNonAlphanumeric = true;
                options.SignIn.RequireConfirmedAccount = true;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            services.AddAuthorization(options =>
                {
                    options.AddPolicy("UserPolicy",
                       policy => policy.AddRequirements(new UserClaimsReqirement()));
                    options.AddPolicy("DeveloperPolicy",
                       policy => policy.AddRequirements(new DeveloperClaimsRequirement()));
                    options.AddPolicy("ManagerPolicy",
                       policy => policy.AddRequirements(new ManagerClaimsRequirement()));
                    options.AddPolicy("AdminPolicy",
                       policy => policy.AddRequirements(new AdminClaimsRequirement()));
                    
                });
         

            services.AddTransient<IAuthorizationHandler, UserLevel>();
            services.AddTransient<IAuthorizationHandler, DeveloperLevel>();
            services.AddTransient<IAuthorizationHandler, ManagerLevel>();
            services.AddTransient<IAuthorizationHandler, AdministratorLevel>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
