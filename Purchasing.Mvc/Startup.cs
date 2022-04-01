// This Startup file is based on ASP.NET Core new project templates and is included
// as a starting point for DI registration and HTTP request processing pipeline configuration.
// This file will need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core startup files, see https://docs.microsoft.com/aspnet/core/fundamentals/startup

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using UCDArch.Data.NHibernate;
using Purchasing.Core.Domain;

namespace Purchasing.Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureContainer(IWindsorContainer container)
        {
            ModelBinders.Binders.DefaultBinder = new UCDArchModelBinder();

            NHibernateSessionConfiguration.Mappings.UseFluentMappings(typeof(Approval).Assembly);
            container.Install(
                new ComponentInstaller(),
                new AutoMapperInstaller());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options => { })
                .AddNewtonsoftJson(options =>
                {
                    options.UseMemberCasing();
                })
            // Allow standard Windsor behavior for services injected into controllers...
            .AddControllersAsServices();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            services.Configure<LDAPSettings>(Configuration);
            services.Configure<SendGridSettings>(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages(context =>
            {
                switch (context.HttpContext.Response.StatusCode)
                {
                    case StatusCodes.Status403Forbidden:
                        context.HttpContext.Response.Redirect("/Error/NotAuthorized");
                        break;
                    case StatusCodes.Status404NotFound:
                        context.HttpContext.Response.Redirect("/Error/Forbidden");
                        break;
                    default:
                        context.HttpContext.Response.Redirect("/Error/Index");
                        break;
                }
                return Task.CompletedTask;
            });

            app.UseStaticFiles();
            
            app.UseAuthentication();
            
            app.UseRouting();
            
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
