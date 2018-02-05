using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Adds dependencies for the infrastructure. 
            services.AddTransient<IExceptionHandler, ExceptionHandler>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // Adds dependencies for the repositories.
            services.AddTransient<IDataProviderFactory, DataProviderFactory>();
            services.AddTransient<INewsRepository, NewsRepository>();
            services.AddTransient<IRedditRepository, RedditRepository>();
            services.AddSingleton<IExceptionRepository, ExceptionRepository>();
            // Adds dependencies for the logic.
            services.AddTransient<INewsLogic, NewsLogic>();
            services.AddSingleton<IRedditRateLimitLogic, RedditRateLimitLogic>();
            services.AddTransient<IRedditLogic, RedditLogic>();
            services.AddTransient<ISystemErrorLogic, SystemErrorLogic>();
            // Adds dependencies for the dashboard content builders.
            services.AddTransient<IDashboardContentBuilder, DashboardNewsBuilder>();
            services.AddTransient<IDashboardContentBuilder, DashboardRedditContentBuilder>();
            // Adds dependencies for the dashboard factory.
            services.AddTransient<IDashboardFactory, DashboardFactory>();
            // Adds dependencies for the view model builders.
            services.AddSingleton<IHtmlHelper, HtmlHelper>();
            services.AddTransient<IViewModelBuilder<InformationViewModel, INews>, NewsToInformationViewModelBuilder>();
            services.AddTransient<IViewModelBuilder<SystemErrorViewModel, ISystemError>, SystemErrorViewModelBuilder>();
            services.AddTransient<IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings>, DashboardSettingsViewModelBuilder>();
            services.AddTransient<IViewModelBuilder<DashboardViewModel, IDashboard>, DashboardViewModelBuilder>();
            
            // Adds other services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
