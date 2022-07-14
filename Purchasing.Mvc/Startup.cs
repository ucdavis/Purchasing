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
using Purchasing.Mvc.Logging;
using Serilog;
using AspNetCore.Security.CAS;
using Purchasing.Mvc.Handlers;
using Microsoft.AspNetCore.Authorization;
using CommonServiceLocator;
using UCDArch.Web.IoC;
using UCDArch.Web.ModelBinder;
using Purchasing.Mvc.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

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
            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
            NHibernateSessionConfiguration.Mappings.UseFluentMappings(typeof(Approval).Assembly);
            container.Install(
                new ComponentInstaller(),
                new AutoMapperInstaller());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add<SerilogControllerActionFilter>();
                // place EntityModelBinderProvider just before the ComplexObjectModelBinderProvider
                options.ModelBinderProviders.Insert(
                    options.ModelBinderProviders.IndexOf(options.ModelBinderProviders.OfType<ComplexObjectModelBinderProvider>().First()),
                    new EntityModelBinderProvider());
                options.ModelMetadataDetailsProviders.Add(new EmptyStringMetadataProvider());
            })
            .AddNewtonsoftJson(options =>
            {
                options.UseMemberCasing();
                options.SerializerSettings.Error += (sender, args) =>
                    {
                        Log.Logger.Warning(args.ErrorContext.Error, "JSON Serialization Error: {message}", args.ErrorContext.Error.Message);
                    };
                options.SerializerSettings.ContractResolver = new EntityJsonContractResolver();
            })
            // Allow standard Windsor behavior for services injected into controllers...
            .AddControllersAsServices();

            // add cas auth backed by a cookie signin scheme
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/LogOn");
                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                })
            .AddCAS(options =>
            {
                options.CasServerUrlBase = Configuration["CasUrl"];
            });
            services.AddAuthorization(options =>
            {
                // Assets can be managed by role or auth token (API)
                options.AddPolicy(Role.Codes.Admin, policy => policy.Requirements.Add(new RoleAccessRequirement(Role.Codes.Admin)));
                options.AddPolicy(Role.Codes.DepartmentalAdmin, policy => policy.Requirements.Add(new RoleAccessRequirement(Role.Codes.DepartmentalAdmin)));
                options.AddPolicy(Role.Codes.Requester, policy => policy.Requirements.Add(new RoleAccessRequirement(Role.Codes.Requester)));
                options.AddPolicy(Role.Codes.Approver, policy => policy.Requirements.Add(new RoleAccessRequirement(Role.Codes.Approver)));
                options.AddPolicy(Role.Codes.AccountManager, policy => policy.Requirements.Add(new RoleAccessRequirement(Role.Codes.AccountManager)));
                options.AddPolicy(Role.Codes.Purchaser, policy => policy.Requirements.Add(new RoleAccessRequirement(Role.Codes.Purchaser)));
                options.AddPolicy(Role.Codes.EmulationUser, policy => policy.Requirements.Add(new RoleAccessRequirement(Role.Codes.EmulationUser)));
                options.AddPolicy(Role.Codes.Reviewer, policy => policy.Requirements.Add(new RoleAccessRequirement(Role.Codes.Reviewer)));
                options.AddPolicy(Role.Codes.SscAdmin, policy => policy.Requirements.Add(new RoleAccessRequirement(Role.Codes.SscAdmin)));
                options.AddPolicy(Role.Codes.AdminWorkgroup, policy => policy.Requirements.Add(new RoleAccessRequirement(Role.Codes.AdminWorkgroup)));
                options.AddPolicy(Role.Codes.AdhocAccountManager, policy => policy.Requirements.Add(new RoleAccessRequirement(Role.Codes.AdhocAccountManager)));
            });

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddScoped<IAuthorizationHandler, VerifyRoleAccessHandler>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IReportService, ReportService>();

            services.Configure<LDAPSettings>(Configuration);
            services.Configure<SendGridSettings>(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/Index");
                app.UseStatusCodePages(context =>
                {
                    switch (context.HttpContext.Response.StatusCode)
                    {
                        case StatusCodes.Status401Unauthorized:
                        case StatusCodes.Status403Forbidden:
                            context.HttpContext.Response.Redirect("/Error/NotAuthorized");
                            break;
                        case StatusCodes.Status404NotFound:
                            context.HttpContext.Response.Redirect("/Error/FileNotFound");
                            break;
                        default:
                            context.HttpContext.Response.Redirect("/Error/Index");
                            break;
                    }
                    return Task.CompletedTask;
                });
            }

            app.UseStaticFiles();
            app.UseSerilogRequestLogging();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
