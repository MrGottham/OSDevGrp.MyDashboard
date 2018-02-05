using System;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Factories
{
    public class DashboardRedditContentBuilder : IDashboardRedditContentBuilder
    {
        #region Private variables

        private readonly IRedditLogic _redditLogic;
        private readonly IExceptionHandler _exceptionHandler;

        #endregion

        #region Constructor

        public DashboardRedditContentBuilder(IRedditLogic redditLogic, IExceptionHandler exceptionHandler)
        {
            if (redditLogic == null)
            {
                throw new ArgumentNullException(nameof(redditLogic));
            }
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(exceptionHandler));
            }

            _redditLogic = redditLogic;
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

            return dashboardSettings.UseReddit && dashboardSettings.RedditAccessToken != null;
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
                    IRedditAccessToken accessToken = dashboardSettings.RedditAccessToken;
                    if (DateTime.Now > accessToken.Expires)
                    {
                        // TODO: Renew our access token.
                    }

                    IRedditAuthenticatedUser authenticatedUser = await _redditLogic.GetAuthenticatedUserAsync(accessToken);
                    if (authenticatedUser == null)
                    {
                        return;
                    }

                    dashboard.Replace(authenticatedUser);
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
