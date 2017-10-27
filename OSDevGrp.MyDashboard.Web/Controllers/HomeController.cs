using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Controllers
{
    public class HomeController : Controller
    {
        #region Private variables

        private readonly IDashboardFactory _dashboardFactory;

        #endregion

        #region Constructor

        public HomeController(IDashboardFactory dashboardFactory)
        {
            if (dashboardFactory == null)
            {
                throw new ArgumentNullException(nameof(dashboardFactory));
            }

            _dashboardFactory = dashboardFactory;
        }

        #endregion

        #region Methods

        public IActionResult Index()
        {
            try
            {
                IDashboardSettings dashboardSettings = new DashboardSettings
                {
                    NumberOfNews = 100
                };
                Task<IDashboard> buildDashboardTask = _dashboardFactory.BuildAsync(dashboardSettings);
                buildDashboardTask.Wait();

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

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion
    }
}
