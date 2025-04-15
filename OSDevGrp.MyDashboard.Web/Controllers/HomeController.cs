using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Web.Controllers
{
    public class HomeController : Controller
    {
        #region Private variables

        private readonly IDashboardFactory _dashboardFactory;
        private readonly IViewModelBuilder<DashboardViewModel, IDashboard> _dashboardViewModelBuilder;
        private readonly IModelExporter<DashboardExportModel, IDashboard> _dashboardModelExporter;
        private readonly IRedditAccessTokenProviderFactory _redditAccessTokenProviderFactory;
        private readonly IContentHelper _contentHelper;
        private readonly ICookieHelper _cookieHelper;

        #endregion

        #region Constructor

        public HomeController(IDashboardFactory dashboardFactory, IViewModelBuilder<DashboardViewModel, IDashboard> dashboardViewModelBuilder, IModelExporter<DashboardExportModel, IDashboard> dashboardModelExporter, IRedditAccessTokenProviderFactory redditAccessTokenProviderFactory, IContentHelper contentHelper, ICookieHelper cookieHelper)
        {
            if (dashboardFactory == null)
            {
                throw new ArgumentNullException(nameof(dashboardFactory));
            }
            if (dashboardViewModelBuilder == null)
            {
                throw new ArgumentNullException(nameof(dashboardViewModelBuilder));
            }
            if (dashboardModelExporter == null)
            {
                throw new ArgumentNullException(nameof(dashboardModelExporter));
            }
            if (redditAccessTokenProviderFactory == null)
            {
                throw new ArgumentNullException(nameof(redditAccessTokenProviderFactory));
            }
            if (contentHelper == null)
            {
                throw new ArgumentNullException(nameof(contentHelper));
            }
            if (cookieHelper == null)
            {
                throw new ArgumentNullException(nameof(cookieHelper));
            }

            _dashboardFactory = dashboardFactory;
            _dashboardViewModelBuilder = dashboardViewModelBuilder;
            _dashboardModelExporter = dashboardModelExporter;
            _redditAccessTokenProviderFactory = redditAccessTokenProviderFactory;
            _contentHelper = contentHelper;
            _cookieHelper = cookieHelper;
        }

        #endregion

        #region Methods

        [HttpGet]
        public IActionResult Index()
        {
            DashboardSettingsViewModel defaultDashboardSettingsViewModel = new DashboardSettingsViewModel
            {
                NumberOfNews = 100,
                UseReddit = false,
                AllowNsfwContent = false,
                IncludeNsfwContent = false,
                OnlyNsfwContent = false,
                RedditAccessToken = null,
                ExportData = false
            };

            DashboardSettingsViewModel dashboardSettingsViewModel = _cookieHelper.ToDashboardSettingsViewModel();

            return View("Index", dashboardSettingsViewModel ?? defaultDashboardSettingsViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Build(string dashboardSettings)
        {
            if (string.IsNullOrWhiteSpace(dashboardSettings))
            {
                return BadRequest();
            }

            DashboardSettingsViewModel dashboardSettingsViewModel = _contentHelper.ToDashboardSettingsViewModel(dashboardSettings);
            if (dashboardSettingsViewModel == null)
            {
                return BadRequest();
            }

            IDashboard dashboard = await _dashboardFactory.BuildAsync(dashboardSettingsViewModel.ToDashboardSettings());
            DashboardViewModel dashboardViewModel = await _dashboardViewModelBuilder.BuildAsync(dashboard);

            return PartialView("_MainContentPartial", dashboardViewModel);
        }

        [HttpGet]
        public IActionResult TopContent()
        {
            DashboardViewModel dashboardViewModel = _cookieHelper.ToDashboardViewModel();
            if (dashboardViewModel == null)
            {
                return PartialView("_EmptyContentPartial");
            }

            return PartialView("_TopContentPartial", dashboardViewModel);
        }

        [HttpGet]
        public IActionResult SubContent()
        {
            DashboardViewModel dashboardViewModel = _cookieHelper.ToDashboardViewModel();
            if (dashboardViewModel == null)
            {
                return PartialView("_EmptyContentPartial");
            }

            return PartialView("_SubContentPartial", dashboardViewModel);
        }

        [HttpGet]
        public IActionResult Settings(string dashboardSettings)
        {
            if (string.IsNullOrWhiteSpace(dashboardSettings))
            {
                return BadRequest();
            }

            DashboardSettingsViewModel dashboardSettingsViewModel = _contentHelper.ToDashboardSettingsViewModel(dashboardSettings);
            if (dashboardSettingsViewModel == null)
            {
                return BadRequest();
            }

            return PartialView("_DashboardSettingsPartial", dashboardSettingsViewModel);
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
                return await AcquireRedditAuthorizationTokenAsync(dashboardSettingsViewModel);
            }

            dashboardSettingsViewModel.IncludeNsfwContent = dashboardSettingsViewModel.UseReddit ? dashboardSettingsViewModel.NotNullableIncludeNsfwContent : (bool?) null;
            dashboardSettingsViewModel.OnlyNsfwContent = dashboardSettingsViewModel.UseReddit ? dashboardSettingsViewModel.NotNullableOnlyNsfwContent : (bool?) null;

            return View("Index", dashboardSettingsViewModel);
        }

        [HttpGet]
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

            if (string.IsNullOrWhiteSpace(error) == false)
            {
                return Unauthorized($"Unable to get the access token from Reddit: {error}");
            }

            DashboardSettingsViewModel dashboardSettingsViewModel = _contentHelper.ToDashboardSettingsViewModel(state);
            if (dashboardSettingsViewModel == null)
            {
                return BadRequest();    
            }

            IRedditAccessToken redditAccessToken = await GetRedditAccessTokenAsync(code);
            if (redditAccessToken != null)
            {
                dashboardSettingsViewModel.RedditAccessToken = redditAccessToken.ToBase64();
                return View("Index", dashboardSettingsViewModel);
            }

            dashboardSettingsViewModel.UseReddit = false;
            dashboardSettingsViewModel.AllowNsfwContent = false;
            dashboardSettingsViewModel.IncludeNsfwContent = false;
            dashboardSettingsViewModel.OnlyNsfwContent = false;
            dashboardSettingsViewModel.RedditAccessToken = null;

            return View("Index", dashboardSettingsViewModel);
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

        [HttpGet]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        [HttpGet("/api/export")]
        public async Task<IActionResult> Export(int numberOfNews = 100, [FromHeader]string redditAccessToken = null, bool includeNsfwContent = false, bool onlyNsfwContent = false)
        {
            if (numberOfNews < 0)
            {
                return BadRequest($"The submitted value for '{nameof(numberOfNews)}' cannot be lower than 0.");
            }
            if (numberOfNews > 250)
            {
                return BadRequest($"The submitted value for '{nameof(numberOfNews)}' cannot be greater than 250.");
            }

            IRedditAccessToken token = null;
            if (string.IsNullOrWhiteSpace(redditAccessToken) == false)
            {
                try
                {
                    token = RedditAccessToken.Create(redditAccessToken);
                }
                catch (SerializationException)
                {
                    return BadRequest($"The submitted value for '{nameof(redditAccessToken)}' is invalid.");
                }
                catch (FormatException)
                {
                    return BadRequest($"The submitted value for '{nameof(redditAccessToken)}' is invalid.");
                }
            }

            DashboardSettingsViewModel dashboardSettingsViewModel = new DashboardSettingsViewModel
            {
                NumberOfNews = numberOfNews,
                UseReddit = token != null,
                AllowNsfwContent = token != null ? includeNsfwContent || onlyNsfwContent : false,
                IncludeNsfwContent = token != null ? includeNsfwContent : false,
                NotNullableIncludeNsfwContent = token != null ? includeNsfwContent : false,
                OnlyNsfwContent = token != null ? onlyNsfwContent : false,
                NotNullableOnlyNsfwContent = token != null ? onlyNsfwContent : false,
                RedditAccessToken = token?.ToBase64(),
                ExportData = true
            };
            IDashboard dashboard = await _dashboardFactory.BuildAsync(dashboardSettingsViewModel.ToDashboardSettings());

            DashboardExportModel dashboardExportModel = await _dashboardModelExporter.ExportAsync(dashboard);

            return Ok(dashboardExportModel);
        }

        private async Task<IActionResult> AcquireRedditAuthorizationTokenAsync(DashboardSettingsViewModel dashboardSettingsViewModel)
        {
            if (dashboardSettingsViewModel == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettingsViewModel));
            }

            string dashboardSettingsViewModelAsBase64 = _contentHelper.ToBase64String(dashboardSettingsViewModel);
            Uri redirectUrl = new Uri(_contentHelper.AbsoluteUrl("RedditCallback", "Home"));

            Uri acquireRedditAuthorizationTokenUrl = await _redditAccessTokenProviderFactory.AcquireRedditAuthorizationTokenAsync(dashboardSettingsViewModelAsBase64, redirectUrl);

            return Redirect(acquireRedditAuthorizationTokenUrl.AbsoluteUri);
        }

        private async Task<IRedditAccessToken> GetRedditAccessTokenAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            Uri redirectUrl = new Uri(_contentHelper.AbsoluteUrl("RedditCallback", "Home"));

            return await _redditAccessTokenProviderFactory.GetRedditAccessTokenAsync(code, redirectUrl);
        }

        #endregion
    }
}