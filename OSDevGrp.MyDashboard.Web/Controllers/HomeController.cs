using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IConfiguration _configuration;

        #endregion

        #region Constructor

        public HomeController(IDashboardFactory dashboardFactory, IViewModelBuilder<DashboardViewModel, IDashboard> dashboardViewModelBuilder, IConfiguration configuration)
        {
            if (dashboardFactory == null)
            {
                throw new ArgumentNullException(nameof(dashboardFactory));
            }
            if (dashboardViewModelBuilder == null)
            {
                throw new ArgumentNullException(nameof(dashboardViewModelBuilder));
            }
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _dashboardFactory = dashboardFactory;
            _dashboardViewModelBuilder = dashboardViewModelBuilder;
            _configuration = configuration;
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
                return null;
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

        #endregion
    }
}
