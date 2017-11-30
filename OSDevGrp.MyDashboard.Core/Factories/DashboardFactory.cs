using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;

namespace OSDevGrp.MyDashboard.Core.Factories
{
    public class DashboardFactory : IDashboardFactory
    {
        #region Private variables

        private readonly IEnumerable<IDashboardContentBuilder> _dashboardContentBuilderCollection;
        private readonly ISystemErrorLogic _systemErrorLogic;
        private readonly IExceptionHandler _exceptionHandler;

        #endregion

        #region Constructor

        public DashboardFactory(IEnumerable<IDashboardContentBuilder> dashboardContentBuilderCollection, ISystemErrorLogic systemErrorLogic, IExceptionHandler exceptionHandler)
        {
            if (dashboardContentBuilderCollection == null)
            {
                throw new ArgumentNullException(nameof(dashboardContentBuilderCollection));
            }
            if (systemErrorLogic == null)
            {
                throw new ArgumentNullException(nameof(systemErrorLogic));
            }
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(exceptionHandler));
            }

            _dashboardContentBuilderCollection = dashboardContentBuilderCollection;
            _systemErrorLogic = systemErrorLogic;
            _exceptionHandler = exceptionHandler;
        }

        #endregion

        #region Methods

        public Task<IDashboard> BuildAsync(IDashboardSettings dashboardSettings)
        {
            if (dashboardSettings == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettings));
            }

            return Task.Run<IDashboard>(async () =>
            {
                IDashboard dashboard = new Dashboard();
                try
                {
                    Task[] dashboardContentBuilderTasks = _dashboardContentBuilderCollection
                        .Where(dashboardContentBuilder => dashboardContentBuilder.ShouldBuild(dashboardSettings))
                        .Select(dashboardContentBuilder => dashboardContentBuilder.BuildAsync(dashboardSettings, dashboard))
                        .ToArray();
                    Task.WaitAll(dashboardContentBuilderTasks);
                }
                catch (AggregateException ex)
                {
                    await _exceptionHandler.HandleAsync(ex);
                }
                catch (Exception ex)
                {
                    await _exceptionHandler.HandleAsync(ex);
                }
                finally
                {
                    dashboard.Replace(dashboardSettings);
                    
                    IEnumerable<ISystemError> systemErrors = await _systemErrorLogic.GetSystemErrorsAsync();
                    dashboard.Replace(systemErrors.OrderByDescending(systemError => systemError.Timestamp));
                }
                return dashboard;
            });
        }

        #endregion 
    }
}