using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Controllers
{
    public class HomeController : Controller
    {
        #region Private variables

        private readonly IDashboardFactory _dashboardFactory;
        private readonly IViewModelBuilder<DashboardViewModel, IDashboard> _dashboardViewModelBuilder;
        private readonly IDataProviderFactory _dataProviderFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Constructor

        public HomeController(IDashboardFactory dashboardFactory, IViewModelBuilder<DashboardViewModel, IDashboard> dashboardViewModelBuilder, IDataProviderFactory dataProviderFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            if (dashboardFactory == null)
            {
                throw new ArgumentNullException(nameof(dashboardFactory));
            }
            if (dashboardViewModelBuilder == null)
            {
                throw new ArgumentNullException(nameof(dashboardViewModelBuilder));
            }
            if (dataProviderFactory == null)
            {
                throw new ArgumentNullException(nameof(dataProviderFactory));
            }
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            _dashboardFactory = dashboardFactory;
            _dashboardViewModelBuilder = dashboardViewModelBuilder;
            _dataProviderFactory = dataProviderFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Methods

        public IActionResult Index()
        {
            IDashboardSettings dashboardSettings = new DashboardSettings
            {
                NumberOfNews = 100,
                UseReddit = false
            };
            return GenerateDashboardView(dashboardSettings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Commit(DashboardSettingsViewModel dashboardSettingsViewModel)
        {
            if (dashboardSettingsViewModel == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettingsViewModel));
            }

            if (ModelState.IsValid == false)
            {
                return RedirectToAction("Index", "HomeController");
            }

            if (dashboardSettingsViewModel.UseReddit)
            {
                return AcquireRedditAccessToken(dashboardSettingsViewModel, _httpContextAccessor.HttpContext);
            }
            
            return GenerateDashboardView(dashboardSettingsViewModel.ToDashboardSettings());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SystemError(SystemErrorViewModel systemErrorViewModel)
        {
            if (systemErrorViewModel == null)
            {
                throw new ArgumentNullException(nameof(systemErrorViewModel));

            }

            return View("SystemError", systemErrorViewModel);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IActionResult GenerateDashboardView(IDashboardSettings dashboardSettings)
        {
            if (dashboardSettings == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettings));
            }

            try
            {
                Task<IDashboard> buildDashboardTask = _dashboardFactory.BuildAsync(dashboardSettings);
                buildDashboardTask.Wait();

                Task<DashboardViewModel> buildDashboardViewModelTask = _dashboardViewModelBuilder.BuildAsync(buildDashboardTask.Result);
                buildDashboardViewModelTask.Wait();

                return View("Index", buildDashboardViewModelTask.Result);
            }
            catch (AggregateException aggregateException)
            {
                aggregateException.Handle(ex => true);
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IActionResult AcquireRedditAccessToken(DashboardSettingsViewModel dashboardSettingsViewModel, HttpContext httpContext)
        {
            if (dashboardSettingsViewModel == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettingsViewModel));
            }
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            try
            {
                string clientId = _configuration["Authentication:Reddit:ClientId"];
                string dashboardSettingsViewModelAsBase64 = dashboardSettingsViewModel.ToBase64();
                Uri redirectUrl = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}/Home/RedditCallback");

                Task<Uri> acquireRedditAccessTokenTask = _dataProviderFactory.AcquireRedditAccessTokenAsync(clientId, dashboardSettingsViewModelAsBase64, redirectUrl);
                acquireRedditAccessTokenTask.Wait();
                
                return Redirect(acquireRedditAccessTokenTask.Result.AbsoluteUri);
            }
            catch (AggregateException aggregateException)
            {
                aggregateException.Handle(ex => true);
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
