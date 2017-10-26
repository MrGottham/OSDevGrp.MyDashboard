using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Factories
{
    public class DashboardNewsBuilder : IDashboardNewsBuilder
    {
        #region Private variables

        private readonly INewsLogic _newLogic;
        private readonly IExceptionHandler _exceptionHandler;

        #endregion

        #region Constructor

        public DashboardNewsBuilder(INewsLogic newsLogic, IExceptionHandler exceptionHandler)
        {
            if (newsLogic == null) throw new ArgumentNullException(nameof(newsLogic));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));

            _newLogic = newsLogic;
            _exceptionHandler = exceptionHandler;
        }

        #endregion

        #region Methods

        public bool ShouldBuild(IDashboardSettings dashboardSettings)
        {
            if (dashboardSettings == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettings));
            }

            return dashboardSettings.NumberOfNews > 0;
        }

        public Task BuildAsync(IDashboardSettings dashboardSettings, IDashboard dashboard)
        {
            if (dashboardSettings == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettings));
            }
            if (dashboard == null)
            {
                throw new ArgumentNullException(nameof(dashboard));
            }

            return Task.Run(async () => 
            {
                try
                {
                    IEnumerable<INews> news = await _newLogic.GetNewsAsync(dashboardSettings.NumberOfNews);

                    dashboard.Replace(news);
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
            });
        }

        #endregion 
    }
}
