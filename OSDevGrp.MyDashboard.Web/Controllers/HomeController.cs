using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
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
        private readonly IRedditAccessTokenProviderFactory _redditAccessTokenProviderFactory;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Constructor

        public HomeController(IDashboardFactory dashboardFactory, IViewModelBuilder<DashboardViewModel, IDashboard> dashboardViewModelBuilder, IRedditAccessTokenProviderFactory redditAccessTokenProviderFactory, IExceptionHandler exceptionHandler, IHttpContextAccessor httpContextAccessor)
        {
            if (dashboardFactory == null)
            {
                throw new ArgumentNullException(nameof(dashboardFactory));
            }
            if (dashboardViewModelBuilder == null)
            {
                throw new ArgumentNullException(nameof(dashboardViewModelBuilder));
            }
            if (redditAccessTokenProviderFactory == null)
            {
                throw new ArgumentNullException(nameof(redditAccessTokenProviderFactory));
            }
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(exceptionHandler));
            }
            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            _dashboardFactory = dashboardFactory;
            _dashboardViewModelBuilder = dashboardViewModelBuilder;
            _redditAccessTokenProviderFactory = redditAccessTokenProviderFactory;
            _exceptionHandler = exceptionHandler;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Index()
        {
            IDashboardSettings defaultDashboardSettings = new DashboardSettings
            {
                NumberOfNews = 100,
                UseReddit = false,
                RedditAccessToken = null,
                IncludeNsfwContent = false,
                OnlyNsfwContent = false
            };

            DashboardSettingsViewModel dashboardSettingsViewModel = DashboardSettingsViewModel.Create(_httpContextAccessor.HttpContext);
            
            return await GenerateDashboardViewAsync(dashboardSettingsViewModel == null ? defaultDashboardSettings : dashboardSettingsViewModel.ToDashboardSettings());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Commit(DashboardSettingsViewModel dashboardSettingsViewModel)
        {
            if (dashboardSettingsViewModel == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettingsViewModel));
            }

            if (ModelState.IsValid == false)
            {
                return RedirectToAction("Index", "HomeController");
            }

            if (dashboardSettingsViewModel.UseReddit && string.IsNullOrWhiteSpace(dashboardSettingsViewModel.RedditAccessToken))
            {
                return await AcquireRedditAuthorizationTokenAsync(dashboardSettingsViewModel, _httpContextAccessor.HttpContext);
            }
            
            return await GenerateDashboardViewAsync(dashboardSettingsViewModel.ToDashboardSettings());
        }

        public async Task<IActionResult> RedditCallback(string code, string state, string error = null)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }
            if (string.IsNullOrWhiteSpace(state))
            {
                throw new ArgumentNullException(nameof(state));
            }

            DashboardSettingsViewModel dashboardSettingsViewModel = DashboardSettingsViewModel.Create(state);
            IDashboardSettings dashboardSettings = dashboardSettingsViewModel.ToDashboardSettings();

            if (string.IsNullOrWhiteSpace(error) == false)
            {
                return await HandleErrorFromRedditAsync(error, dashboardSettings);
            }

            IRedditAccessToken redditAccessToken = await GetRedditAccessTokenAsync(code, _httpContextAccessor.HttpContext);
            dashboardSettings.UseReddit = redditAccessToken != null;
            dashboardSettings.RedditAccessToken = redditAccessToken;

            return await GenerateDashboardViewAsync(dashboardSettings);
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

        private async Task<IActionResult> GenerateDashboardViewAsync(IDashboardSettings dashboardSettings)
        {
            if (dashboardSettings == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettings));
            }

            try
            {
                IDashboard dashboard = await _dashboardFactory.BuildAsync(dashboardSettings);

                DashboardViewModel dashboardViewModel = await _dashboardViewModelBuilder.BuildAsync(dashboard);

                return View("Index", dashboardViewModel);
            }
            catch (AggregateException aggregateException)
            {
                await _exceptionHandler.HandleAsync(aggregateException);
                throw;
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
                throw;
            }
        }

        private async Task<IActionResult> AcquireRedditAuthorizationTokenAsync(DashboardSettingsViewModel dashboardSettingsViewModel, HttpContext httpContext)
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
                string dashboardSettingsViewModelAsBase64 = dashboardSettingsViewModel.ToBase64();
                Uri redirectUrl = GetRedditCallbackUri(httpContext);

                Uri acquireRedditAccessToken = await _redditAccessTokenProviderFactory.AcquireRedditAuthorizationTokenAsync(dashboardSettingsViewModelAsBase64, redirectUrl);
                
                return Redirect(acquireRedditAccessToken.AbsoluteUri);
            }
            catch (AggregateException aggregateException)
            {
                await _exceptionHandler.HandleAsync(aggregateException);
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            dashboardSettingsViewModel.UseReddit = false;
            dashboardSettingsViewModel.RedditAccessToken = null;
            return await GenerateDashboardViewAsync(dashboardSettingsViewModel.ToDashboardSettings());
        }

        private async Task<IRedditAccessToken> GetRedditAccessTokenAsync(string code, HttpContext httpContext)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            
            try
            {
                Uri redirectUrl = GetRedditCallbackUri(httpContext);

                IRedditAccessToken getRedditAccessToken = await _redditAccessTokenProviderFactory.GetRedditAccessTokenAsync(code, redirectUrl);

                return getRedditAccessToken;
            }
            catch (AggregateException aggregateException)
            {
                await _exceptionHandler.HandleAsync(aggregateException);
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            return null;
        }

        private async Task<IActionResult> HandleErrorFromRedditAsync(string error, IDashboardSettings dashboardSettings)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                throw new ArgumentNullException(nameof(error));
            }
            if (dashboardSettings == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettings));
            }
            
            try
            {
                throw new Exception($"Unable to get the access token from Reddit: {error}");
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }

            dashboardSettings.UseReddit = false;
            dashboardSettings.RedditAccessToken = null;
            return await GenerateDashboardViewAsync(dashboardSettings);
        }

        private Uri GetRedditCallbackUri(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            return new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}/Home/RedditCallback");
        }

        #endregion
    }
}
