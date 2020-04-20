using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;
using OSDevGrp.MyDashboard.Core.Factories;
using OSDevGrp.MyDashboard.Core.Infrastructure;
using OSDevGrp.MyDashboard.Core.Logic;
using OSDevGrp.MyDashboard.Core.Repositories;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Factories;
using OSDevGrp.MyDashboard.Web.Helpers;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(opt =>
            {
                opt.CheckConsentNeeded = context => true;
                opt.MinimumSameSitePolicy = SameSiteMode.None;
                opt.Secure = CookieSecurePolicy.Always;
            });

            services.AddDataProtection()
                .SetApplicationName("OSDevGrp.MyDashboard.Web")
                .SetDefaultKeyLifetime(new TimeSpan(30, 0, 0, 0));

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddHealthChecks();

            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory => 
            {
                IActionContextAccessor actionContextAccessor = factory.GetRequiredService<IActionContextAccessor>();
                return factory.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(actionContextAccessor.ActionContext);
            });
            services.AddMemoryCache();

            // Adds dependencies for the infrastructure. 
            services.AddScoped<IExceptionHandler, ExceptionHandler>();
            services.AddSingleton<ISeedGenerator, SeedGenerator>();
            services.AddSingleton<IRandomizer, Randomizer>();
            // Adds dependencies for the repositories.
            services.AddTransient<IDataProviderFactory, DataProviderFactory>();
            services.AddTransient<IRedditAccessTokenProviderFactory, RedditAccessTokenProviderFactory>();
            services.AddTransient<INewsRepository, NewsRepository>();
            services.AddTransient<IRedditRepository, RedditRepository>();
            services.AddScoped<IExceptionRepository, ExceptionRepository>();
            // Adds dependencies for the logic.
            services.AddTransient<INewsLogic, NewsLogic>();
            services.AddScoped<IRedditRateLimitLogic, RedditRateLimitLogic>();
            services.AddTransient<IRedditFilterLogic, RedditFilterLogic>();
            services.AddTransient<IRedditLogic, RedditLogic>();
            services.AddTransient<ISystemErrorLogic, SystemErrorLogic>();
            // Adds dependencies for the dashboard content builders.
            services.AddTransient<IDashboardContentBuilder, DashboardNewsBuilder>();
            services.AddTransient<IDashboardContentBuilder, DashboardRedditContentBuilder>();
            // Adds dependencies for the dashboard factory.
            services.AddTransient<IDashboardFactory, DashboardFactory>();
            // Adds dependencies for the view model builders.
            services.AddSingleton<IHtmlHelper, HtmlHelper>();
            services.AddSingleton<IHttpHelper, HttpHelper>();
            services.AddScoped<IContentHelper, ContentHelper>();
            services.AddScoped<ICookieHelper, CookieHelper>();
            services.AddTransient<IViewModelBuilder, NewsToInformationViewModelBuilder>();
            services.AddTransient<IViewModelBuilder, RedditAuthenticatedUserToObjectViewModelBuilder>();
            services.AddTransient<IViewModelBuilder, RedditSubredditToObjectViewModelBuilder>();
            services.AddTransient<IViewModelBuilder, RedditLinkToInformationViewModelBuilder>();
            services.AddTransient<IViewModelBuilder, SystemErrorViewModelBuilder>();
            services.AddTransient<IViewModelBuilder, DashboardSettingsViewModelBuilder>();
            services.AddTransient<IViewModelBuilder<DashboardViewModel, IDashboard>, DashboardViewModelBuilder>();
            services.AddTransient<IModelExporter, NewsModelExporter>();
            services.AddTransient<IModelExporter, RedditAuthenticatedUserModelExporter>();
            services.AddTransient<IModelExporter, RedditSubredditModelExporter>();
            services.AddTransient<IModelExporter, RedditLinkModelExporter>();
            services.AddTransient<IModelExporter<DashboardExportModel, IDashboard>, DashboardModelExporter>(); 
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseCookiePolicy();

            app.UseCors("default");

            app.UseEndpoints(endpoints => 
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}