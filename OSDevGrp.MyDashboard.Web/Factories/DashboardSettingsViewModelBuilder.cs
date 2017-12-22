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
            IRedditAccessToken redditAccessToken = dashboardSettings.RedditAccessToken;

            return new DashboardSettingsViewModel
            {
                NumberOfNews = dashboardSettings.NumberOfNews,
                UseReddit = dashboardSettings.UseReddit,
                RedditAccessToken = redditAccessToken != null ? redditAccessToken.ToBase64() : null
            };
        }

        #endregion
    }
}
