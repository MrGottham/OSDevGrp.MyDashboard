using System;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class DashboardSettingsViewModelBuilder : ViewModelBuilderBase<DashboardSettingsViewModel, IDashboardSettings>
    {
        #region Private variables

        private readonly ICookieHelper _cookieHelper;

        #endregion

        #region Constructor

        public DashboardSettingsViewModelBuilder(ICookieHelper cookieHelper)
        {
            if (cookieHelper == null)
            {
                throw new ArgumentNullException(nameof(cookieHelper));
            }

            _cookieHelper = cookieHelper;
        }

        #endregion 

        #region Methods

        protected override DashboardSettingsViewModel Build(IDashboardSettings dashboardSettings)
        {
            IRedditAccessToken redditAccessToken = dashboardSettings.RedditAccessToken;

            bool includeNsfwContent = dashboardSettings.IncludeNsfwContent;
            bool onlyNsfwContent = dashboardSettings.OnlyNsfwContent;

            DashboardSettingsViewModel dashboardSettingsViewModel = new DashboardSettingsViewModel
            {
                NumberOfNews = dashboardSettings.NumberOfNews,
                UseReddit = dashboardSettings.UseReddit,
                IncludeNsfwContent = includeNsfwContent ? true : (bool?) null,
                OnlyNsfwContent = onlyNsfwContent ? true : (bool?) null,
                RedditAccessToken = redditAccessToken != null ? redditAccessToken.ToBase64() : null
            };

            _cookieHelper.ToCookie(dashboardSettingsViewModel);

            return dashboardSettingsViewModel;
        }

        #endregion
    }
}