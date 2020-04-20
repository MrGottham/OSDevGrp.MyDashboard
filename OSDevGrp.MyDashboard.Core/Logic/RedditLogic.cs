using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;

namespace OSDevGrp.MyDashboard.Core.Logic
{
    public class RedditLogic : IRedditLogic
    {
        #region Private variables

        private readonly IDataProviderFactory _dataProviderFactory;
        private readonly IRedditAccessTokenProviderFactory _redditAccessTokenProviderFactory;
        private readonly IRedditRepository _redditRepository;
        private readonly IRedditRateLimitLogic _redditRateLimitLogic;
        private readonly IRedditFilterLogic _redditFilterLogic;
        private readonly IExceptionHandler _exceptionHandler;
        
        #endregion

        #region Constructor

        public RedditLogic(IDataProviderFactory dataProviderFactory, IRedditAccessTokenProviderFactory redditAccessTokenProviderFactory, IRedditRepository redditRepository, IRedditRateLimitLogic redditRateLimitLogic, IRedditFilterLogic redditFilterLogic, IExceptionHandler exceptionHandler)
        {
            if (dataProviderFactory == null)
            {
                throw new ArgumentNullException(nameof(dataProviderFactory));
            }
            if (redditAccessTokenProviderFactory == null)
            {
                throw new ArgumentNullException(nameof(redditAccessTokenProviderFactory));
            }
            if (redditRepository == null)
            {
                throw new ArgumentNullException(nameof(redditRepository));
            }
            if (redditRateLimitLogic == null)
            {
                throw new ArgumentNullException(nameof(redditRateLimitLogic));
            }
            if (redditFilterLogic == null)
            {
                throw new ArgumentNullException(nameof(redditFilterLogic));
            }
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(exceptionHandler));
            }

            _dataProviderFactory = dataProviderFactory;
            _redditAccessTokenProviderFactory = redditAccessTokenProviderFactory;
            _redditRepository = redditRepository;
            _redditRateLimitLogic = redditRateLimitLogic;
            _redditFilterLogic = redditFilterLogic;
            _exceptionHandler = exceptionHandler;
        }

        #endregion

        #region Methods
        
        public async Task<IRedditAccessToken> RenewAccessTokenAsync(IRedditAccessToken accessToken)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            try
            {
                if (DateTime.Now <= accessToken.Expires)
                {
                    return accessToken;
                }

                return await _redditAccessTokenProviderFactory.RenewRedditAccessTokenAsync(accessToken.RefreshToken);
            }
            catch (AggregateException ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            return null;
        }

        public async Task<IRedditAuthenticatedUser> GetAuthenticatedUserAsync(IRedditAccessToken accessToken)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            try
            {
                if (_redditRateLimitLogic.WillExceedRateLimit(1))
                {
                    return null;
                }

                IRedditResponse<IRedditAuthenticatedUser> response =  await _redditRepository.GetAuthenticatedUserAsync(accessToken);
                if (response == null)
                {
                    return null;
                }

                await _redditRateLimitLogic.EnforceRateLimitAsync(response.RateLimitUsed, response.RateLimitRemaining, response.RateLimitResetTime, response.ReceivedTime);

                return response.Data;
            }
            catch (AggregateException ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            return null;
        }

        public async Task<IEnumerable<IRedditSubreddit>> GetSubredditsForAuthenticatedUserAsync(IRedditAccessToken accessToken, bool includeNsfwContent, bool onlyNsfwContent)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            try
            {
                if (_redditRateLimitLogic.WillExceedRateLimit(1))
                {
                    return new List<IRedditSubreddit>(0);
                }

                IRedditResponse<IRedditList<IRedditSubreddit>> response = await _redditRepository.GetSubredditsForAuthenticatedUserAsync(accessToken);
                if (response == null)
                {
                    return new List<IRedditSubreddit>(0);
                }

                await _redditRateLimitLogic.EnforceRateLimitAsync(response.RateLimitUsed, response.RateLimitRemaining, response.RateLimitResetTime, response.ReceivedTime);

                return ApplyFilters(response.Data, includeNsfwContent, onlyNsfwContent)
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

        public async Task<IRedditSubreddit> GetSpecificSubredditAsync(IRedditAccessToken accessToken, IRedditKnownSubreddit knownSubreddit)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            if (knownSubreddit == null)
            {
                throw new ArgumentNullException(nameof(knownSubreddit));
            }

            try
            {
                if (_redditRateLimitLogic.WillExceedRateLimit(1))
                {
                    return null;
                }

                IRedditResponse<IRedditSubreddit> response = await _redditRepository.GetSpecificSubredditAsync(accessToken, knownSubreddit);
                if (response == null)
                {
                    return null;
                }

                await _redditRateLimitLogic.EnforceRateLimitAsync(response.RateLimitUsed, response.RateLimitRemaining, response.RateLimitResetTime, response.ReceivedTime);

                return response.Data;
            }
            catch (AggregateException ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            return null;
        }

        public async Task<IEnumerable<IRedditSubreddit>> GetNsfwSubredditsAsync(IRedditAccessToken accessToken, int numberOfSubreddits)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            try
            {
                List<IRedditKnownSubreddit> knownNsfwSubreddits = (await _dataProviderFactory.GetKnownNsfwSubredditsAsync())
                    .OrderBy(m => m.Rank)
                    .ThenBy(m => m.Name)
                    .ToList();

                int numberOfSubredditsToGet = Math.Min(numberOfSubreddits, knownNsfwSubreddits.Count);
                if (_redditRateLimitLogic.WillExceedRateLimit(numberOfSubredditsToGet))
                {
                    return new List<IRedditSubreddit>(0);
                }

                Task<IRedditSubreddit>[] getSpecificSubredditTaskArray = knownNsfwSubreddits.Take(numberOfSubredditsToGet)
                    .Select(knownNsfwSubreddit => GetSpecificSubredditAsync(accessToken, knownNsfwSubreddit))
                    .ToArray();
                await Task.WhenAll(getSpecificSubredditTaskArray);

                return getSpecificSubredditTaskArray
                    .Where(getSpecificSubredditTask => getSpecificSubredditTask.IsCompleted && getSpecificSubredditTask.IsFaulted == false)
                    .Select(getSpecificSubredditTask => getSpecificSubredditTask.Result)
                    .Where(subreddit => subreddit != null)
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

        public async Task<IEnumerable<IRedditLink>> GetLinksAsync(IRedditAccessToken accessToken, IRedditSubreddit subreddit, bool includeNsfwContent, bool onlyNsfwContent)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            if (subreddit == null)
            {
                throw new ArgumentNullException(nameof(subreddit));
            }

            try
            {
                if (_redditRateLimitLogic.WillExceedRateLimit(1))
                {
                    return new List<IRedditLink>(0);
                }

                IRedditResponse<IRedditList<IRedditLink>> response = await _redditRepository.GetLinksAsync(accessToken, subreddit);
                if (response == null)
                {
                    return new List<IRedditLink>(0);
                }

                await _redditRateLimitLogic.EnforceRateLimitAsync(response.RateLimitUsed, response.RateLimitRemaining, response.RateLimitResetTime, response.ReceivedTime);

                return ApplyFilters(response.Data, includeNsfwContent, onlyNsfwContent)
                    .OrderByDescending(link => link.CreatedTime)
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
            return new List<IRedditLink>(0);
        }

        public async Task<IEnumerable<IRedditLink>> GetLinksAsync(IRedditAccessToken accessToken, IEnumerable<IRedditSubreddit> subredditCollection, bool includeNsfwContent, bool onlyNsfwContent)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            if (subredditCollection == null)
            {
                throw new ArgumentNullException(nameof(subredditCollection));
            }

            try
            {
                if (_redditRateLimitLogic.WillExceedRateLimit(subredditCollection.Count()))
                {
                    return new List<IRedditLink>(0);
                }

                Task<IEnumerable<IRedditLink>>[] getLinksTaskArray = subredditCollection
                    .Select(subreddit => GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent))
                    .ToArray();
                await Task.WhenAll(getLinksTaskArray);

                IRedditThingComparer<IRedditLink> createComparer = await _redditFilterLogic.CreateComparerAsync<IRedditLink>();

                return getLinksTaskArray
                    .Where(getLinksTask => getLinksTask.IsCompleted && getLinksTask.IsFaulted == false && getLinksTask.IsCanceled == false)
                    .SelectMany(getLinksTask => getLinksTask.Result)
                    .Distinct(createComparer)
                    .OrderByDescending(link => link.CreatedTime)
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
            return new List<IRedditLink>(0);
        }

        private IEnumerable<T> ApplyFilters<T>(IEnumerable<T> filterableCollection, bool includeNsfwContent, bool onlyNsfwContent) where T : IRedditFilterable
        {
            if (filterableCollection == null)
            {
                throw new ArgumentNullException(nameof(filterableCollection));
            }

            IEnumerable<T> resultCollection = ApplyFilter(filterableCollection, collection => _redditFilterLogic.RemoveUserBannedContentAsync(collection));
            if (includeNsfwContent == false)
            {
                resultCollection = ApplyFilter(resultCollection, collection => _redditFilterLogic.RemoveNsfwContentAsync(collection));
            }
            if (onlyNsfwContent)
            {
                resultCollection = ApplyFilter(resultCollection, collection => _redditFilterLogic.RemoveNoneNsfwContentAsync(collection));
            }
            return resultCollection;
        }

        private IEnumerable<T> ApplyFilter<T>(IEnumerable<T> filterableCollection, Func<IEnumerable<T>, Task<IEnumerable<T>>> filterTaskGetter) where T : IRedditFilterable
        {
            if (filterableCollection == null)
            {
                throw new ArgumentNullException(nameof(filterableCollection));
            }
            if (filterTaskGetter == null)
            {
                throw new ArgumentNullException(nameof(filterTaskGetter));
            }

            return filterTaskGetter(filterableCollection).GetAwaiter().GetResult();
        }

        #endregion
    }
}