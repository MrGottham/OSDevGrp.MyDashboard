using System;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class DashboardSettingsViewModelBuilder : ViewModelBuilderBase<DashboardSettingsViewModel, IDashboardSettings>
    {
        #region Methods

        protected override DashboardSettingsViewModel Build(IDashboardSettings dashboardSettings)
        {
            return new DashboardSettingsViewModel
            {
                NumberOfNews = dashboardSettings.NumberOfNews,
                UseReddit = dashboardSettings.UseReddit
            };
        }

        #endregion
    }
}
