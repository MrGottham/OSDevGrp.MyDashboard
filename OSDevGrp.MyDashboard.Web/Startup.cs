using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
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
using OSDevGrp.MyDashboard.Web.Options;
using System;

namespace OSDevGrp.MyDashboard.Web
{
    public class Startup
    {
        private const string DotnetRunningInContainerEnvironmentVariable = "DOTNET_RUNNING_IN_CONTAINER";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(opt => 
            {
                opt.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto;
                opt.KnownNetworks.Clear();
                opt.KnownProxies.Clear();
            });

            services.Configure<CookiePolicyOptions>(opt =>
            {
                opt.CheckConsentNeeded = _ => true;
                opt.ConsentCookie.Name = $"{GetType().Namespace!}.Consent";
                opt.MinimumSameSitePolicy = SameSiteMode.Lax;
                opt.Secure = CookieSecurePolicy.Always;
            });

            services.ConfigureApplicationCookie(opt =>
            {
                opt.Cookie.SameSite = SameSiteMode.Strict;
                opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                opt.Cookie.Name = $"{GetType().Namespace}.Application";
                opt.DataProtectionProvider = DataProtectionProvider.Create(GetType().Namespace!);
            });

            services.AddAntiforgery(opt =>
            {
                opt.FormFieldName = "__CSRF";
                opt.HeaderName = $"X-{GetType().Namespace!}-CSRF-TOKEN";
                opt.Cookie.SameSite = SameSiteMode.Strict;
                opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                opt.Cookie.Name = $"{GetType().Namespace!}.Antiforgery";
            });

            services.AddDataProtection()
                .SetApplicationName("OSDevGrp.MyDashboard.Web")
                .UseEphemeralDataProtectionProvider()
                .SetDefaultKeyLifetime(new TimeSpan(30, 0, 0, 0));

            services.AddRazorPages();
            services.AddControllersWithViews();

            services.AddHealthChecks()
                .AddCheck<RedditOptionsHealthCheck>(nameof(RedditOptions));

            services.AddHttpContextAccessor()
                .AddMemoryCache();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped(factory => 
            {
                IActionContextAccessor actionContextAccessor = factory.GetRequiredService<IActionContextAccessor>();
                return factory.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(actionContextAccessor.ActionContext!);
            });

            // Adds dependencies for the infrastructure. 
            services.AddScoped<IExceptionHandler, ExceptionHandler>();
            services.AddSingleton<ISeedGenerator, SeedGenerator>();
            services.AddSingleton<IRandomizer, Randomizer>();
            // Adds dependencies for the repositories.
            services.Configure<RedditOptions>(Configuration.GetSection($"{ConfigurationKeys.AuthenticationSectionName}:{ConfigurationKeys.RedditSectionName}"));
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
        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            if (RunningInDocker() == false)
            {
                app.UseHttpsRedirection();
            }

            app.UseDefaultFiles();
            app.MapStaticAssets();

            app.UseCookiePolicy();

            app.UseRouting();

            app.UseCors("default");

            app.MapDefaultControllerRoute().WithStaticAssets();
            app.MapRazorPages().WithStaticAssets();
            app.MapHealthChecks("/health");
        }

        private static bool RunningInDocker()
        {
            return RunningInDocker(Environment.GetEnvironmentVariable(DotnetRunningInContainerEnvironmentVariable));
        }

        private static bool RunningInDocker(string environmentVariable)
        {
            if (string.IsNullOrWhiteSpace(environmentVariable) || bool.TryParse(environmentVariable, out bool result) == false)
            {
                return false;
            }

            return result;
        }
    }
}