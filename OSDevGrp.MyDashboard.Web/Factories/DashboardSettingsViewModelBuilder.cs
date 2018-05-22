using System;
using Microsoft.AspNetCore.Http;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class DashboardSettingsViewModelBuilder : ViewModelBuilderBase<DashboardSettingsViewModel, IDashboardSettings>
    {
        #region Private variables

        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Constructor

        public DashboardSettingsViewModelBuilder(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            _httpContextAccessor = httpContextAccessor;
        }

        #endregion 

        #region Methods

        protected override DashboardSettingsViewModel Build(IDashboardSettings dashboardSettings)
        {
            DateTime cookieExpireTime = DateTime.Now.AddMinutes(15);

            IRedditAccessToken redditAccessToken = dashboardSettings.RedditAccessToken;
            if (redditAccessToken != null)
            {
                DateTime redditAccessTokenExpireTime = redditAccessToken.Expires;
                if (redditAccessTokenExpireTime < cookieExpireTime)
                {
                    cookieExpireTime = redditAccessTokenExpireTime;
                }
            }

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

            dashboardSettingsViewModel.ToCookie(_httpContextAccessor.HttpContext, cookieExpireTime);

            return dashboardSettingsViewModel;
        }

        #endregion
    }
}
