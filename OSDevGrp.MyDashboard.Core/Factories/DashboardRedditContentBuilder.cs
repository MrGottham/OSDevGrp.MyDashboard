using System;
using System.Collections.Generic;
using System.Linq;
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

                    bool over18 = authenticatedUser.Over18;
                    bool includeNsfwContent = dashboardSettings.IncludeNsfwContent;
                    bool onlyNsfwContent = dashboardSettings.OnlyNsfwContent;
                    IEnumerable<IRedditSubreddit> subreddits = await GetSubredditsAsync(accessToken, over18, includeNsfwContent, onlyNsfwContent);
                    dashboard.Replace(subreddits);
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
            });
        }

        private Task<IEnumerable<IRedditSubreddit>> GetSubredditsAsync(IRedditAccessToken accessToken, bool over18, bool includeNsfwContent, bool onlyNsfwContent)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            return Task.Run<IEnumerable<IRedditSubreddit>>(() =>
            {
                try
                {
                    Task<IEnumerable<IRedditSubreddit>>[] getSubredditCollectionTasks = new[] 
                    {
                        _redditLogic.GetSubredditsForAuthenticatedUserAsync(accessToken, over18 && includeNsfwContent, over18 && onlyNsfwContent)
                    };
                    Task.WaitAll(getSubredditCollectionTasks);

                    return getSubredditCollectionTasks
                        .Where(task => task.IsCompleted && task.IsFaulted == false)
                        .SelectMany(task => task.Result)
                        .OrderByDescending(subreddit => subreddit.Subscribers)
                        .ToList();
                }
                catch (AggregateException ex)
                {
                    _exceptionHandler.HandleAsync(ex);
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex);
                }
                return new List<IRedditSubreddit>(0);
            });
        }

        #endregion 
    }
}