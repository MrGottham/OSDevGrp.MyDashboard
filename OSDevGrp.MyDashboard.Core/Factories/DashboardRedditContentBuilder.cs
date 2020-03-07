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

        public async Task BuildAsync(IDashboardSettings dashboardSettings, IDashboard dashboard)
        {
            if (dashboardSettings == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettings));
            }
            if (dashboard == null)
            {
                throw new ArgumentNullException(nameof(dashboard));
            }

            try
            {
                IRedditAccessToken accessToken = dashboardSettings.RedditAccessToken;
                if (DateTime.Now > accessToken.Expires)
                {
                    accessToken = await _redditLogic.RenewAccessTokenAsync(accessToken);
                    if (accessToken == null)
                    {
                        return;
                    }
                    dashboardSettings.RedditAccessToken = accessToken;
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

                IEnumerable<IRedditLink> links = await GetLinksAsync(accessToken, subreddits, over18, includeNsfwContent, onlyNsfwContent);
                dashboard.Replace(links);
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
        }

        private async Task<IEnumerable<IRedditSubreddit>> GetSubredditsAsync(IRedditAccessToken accessToken, bool over18, bool includeNsfwContent, bool onlyNsfwContent)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            try
            {
                IList<Task<IEnumerable<IRedditSubreddit>>> getSubredditTaskCollection = new List<Task<IEnumerable<IRedditSubreddit>>>
                {
                    _redditLogic.GetSubredditsForAuthenticatedUserAsync(accessToken, over18 && includeNsfwContent, over18 && onlyNsfwContent)
                };
                if (over18 && (includeNsfwContent || onlyNsfwContent))
                {
                    getSubredditTaskCollection.Add(_redditLogic.GetNsfwSubredditsAsync(accessToken, 4));
                }
                await Task.WhenAll(getSubredditTaskCollection.ToArray());

                return getSubredditTaskCollection
                    .Where(getsubredditTask => getsubredditTask.IsCompleted && getsubredditTask.IsFaulted == false && getsubredditTask.IsCanceled == false)
                    .SelectMany(getsubredditTask => getsubredditTask.Result)
                    .OrderByDescending(subreddit => subreddit.Subscribers)
                    .ToList();
            }
            catch (AggregateException ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            return new List<IRedditSubreddit>(0);
        }

        private async Task<IEnumerable<IRedditLink>> GetLinksAsync(IRedditAccessToken accessToken, IEnumerable<IRedditSubreddit> subreddits, bool over18, bool includeNsfwContent, bool onlyNsfwContent)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            if (subreddits == null)
            {
                throw new ArgumentNullException(nameof(subreddits));
            }

            try
            {
                return await _redditLogic.GetLinksAsync(accessToken, subreddits, over18 && includeNsfwContent, over18 && onlyNsfwContent);
            }
            catch (AggregateException ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            return new List<IRedditLink>(0);
        }

        #endregion 
    }
}