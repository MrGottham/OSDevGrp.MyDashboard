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
        private readonly IRedditRepository _redditRepository;
        private readonly IRedditRateLimitLogic _redditRateLimitLogic;
        private readonly IRedditFilterLogic _redditFilterLogic;
        private readonly IExceptionHandler _exceptionHandler;
        
        #endregion

        #region Constructor

        public RedditLogic(IDataProviderFactory dataProviderFactory, IRedditRepository redditRepository, IRedditRateLimitLogic redditRateLimitLogic, IRedditFilterLogic redditFilterLogic, IExceptionHandler exceptionHandler)
        {
            if (dataProviderFactory == null)
            {
                throw new ArgumentNullException(nameof(dataProviderFactory));
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
            _redditRepository = redditRepository;
            _redditRateLimitLogic = redditRateLimitLogic;
            _redditFilterLogic = redditFilterLogic;
            _exceptionHandler = exceptionHandler;
        }

        #endregion

        #region Methods
        
        public Task<IRedditAuthenticatedUser> GetAuthenticatedUserAsync(IRedditAccessToken accessToken)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            return Task.Run<IRedditAuthenticatedUser>(() => 
            {
                try
                {
                    if (_redditRateLimitLogic.WillExceedRateLimit(1))
                    {
                        return null;
                    }

                    Task<IRedditResponse<IRedditAuthenticatedUser>> getAuthenticatedUserTask = _redditRepository.GetAuthenticatedUserAsync(accessToken);
                    getAuthenticatedUserTask.Wait();

                    IRedditResponse<IRedditAuthenticatedUser> response = getAuthenticatedUserTask.Result;

                    Task enforceRateLimitTask = _redditRateLimitLogic.EnforceRateLimitAsync(response.RateLimitUsed, response.RateLimitRemaining, response.RateLimitResetTime, response.ReceivedTime);
                    enforceRateLimitTask.Wait();

                    return response.Data;
                }
                catch (AggregateException ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                return null;
            });
        }

        public Task<IEnumerable<IRedditSubreddit>> GetSubredditsForAuthenticatedUserAsync(IRedditAccessToken accessToken, bool includeNsfwContent, bool onlyNsfwContent)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            return Task.Run<IEnumerable<IRedditSubreddit>>(() =>
            {
                try
                {
                    if (_redditRateLimitLogic.WillExceedRateLimit(1))
                    {
                        return new List<IRedditSubreddit>(0);
                    }

                    Task<IRedditResponse<IRedditList<IRedditSubreddit>>> getSubredditsForAuthenticatedUserTask = _redditRepository.GetSubredditsForAuthenticatedUserAsync(accessToken);
                    getSubredditsForAuthenticatedUserTask.Wait();

                    IRedditResponse<IRedditList<IRedditSubreddit>> response = getSubredditsForAuthenticatedUserTask.Result;

                    Task enforceRateLimitTask = _redditRateLimitLogic.EnforceRateLimitAsync(response.RateLimitUsed, response.RateLimitRemaining, response.RateLimitResetTime, response.ReceivedTime);
                    enforceRateLimitTask.Wait();

                    IEnumerable<IRedditSubreddit> filteredSubredditCollection = ApplyFilter(response.Data, subredditCollection => _redditFilterLogic.RemoveUserBannedContentAsync(subredditCollection));
                    if (includeNsfwContent == false)
                    {
                        filteredSubredditCollection = ApplyFilter(filteredSubredditCollection, subredditCollection => _redditFilterLogic.RemoveNsfwContentAsync(subredditCollection));
                    }
                    if (onlyNsfwContent)
                    {
                        filteredSubredditCollection = ApplyFilter(filteredSubredditCollection, subredditCollection => _redditFilterLogic.RemoveNoneNsfwContentAsync(subredditCollection));
                    }

                    return filteredSubredditCollection.OrderByDescending(m => m.Subscribers).ToList();
                }
                catch (AggregateException ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                return new List<IRedditSubreddit>(0);
            });
        }

        public Task<IRedditSubreddit> GetSpecificSubredditAsync(IRedditAccessToken accessToken, IRedditKnownSubreddit knownSubreddit)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            if (knownSubreddit == null)
            {
                throw new ArgumentNullException(nameof(knownSubreddit));
            }

            return Task.Run<IRedditSubreddit>(() => 
            {
                IRedditSubreddit s = null;
                try
                {
                    if (_redditRateLimitLogic.WillExceedRateLimit(1))
                    {
                        return null;
                    }

                    Task<IRedditResponse<IRedditSubreddit>> getSpecificSubredditTask = _redditRepository.GetSpecificSubredditAsync(accessToken, knownSubreddit);
                    getSpecificSubredditTask.Wait();
                }
                catch (AggregateException ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
               return s;
            });
        }

        public Task<IEnumerable<IRedditSubreddit>> GetNsfwSubredditsAsync(IRedditAccessToken accessToken, int numberOfSubreddits)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            return Task.Run<IEnumerable<IRedditSubreddit>>(() =>
            {
                try
                {
                    Task<IEnumerable<IRedditKnownSubreddit>> getKnownNsfwSubredditsTask = _dataProviderFactory.GetKnownNsfwSubredditsAsync();
                    getKnownNsfwSubredditsTask.Wait();

                    List<IRedditKnownSubreddit> knownNsfwSubreddits = getKnownNsfwSubredditsTask.Result
                        .OrderBy(m => m.Rank)
                        .ThenBy(m => m.Name)
                        .ToList();

                    int numberOfSubredditsToGet = Math.Min(numberOfSubreddits, knownNsfwSubreddits.Count);
                    if (_redditRateLimitLogic.WillExceedRateLimit(numberOfSubredditsToGet))
                    {
                        return new List<IRedditSubreddit>(0);
                    }

                    Task<IRedditSubreddit>[] getSpecificSubredditArray = knownNsfwSubreddits.Take(numberOfSubredditsToGet)
                        .Select(knownNsfwSubreddit => GetSpecificSubredditAsync(accessToken, knownNsfwSubreddit))
                        .ToArray();
                    Task.WaitAll(getSpecificSubredditArray);
                }
                catch (AggregateException ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                return new List<IRedditSubreddit>(0);
            });
        }

        private IEnumerable<IRedditSubreddit> ApplyFilter(IEnumerable<IRedditSubreddit> subredditCollection, Func<IEnumerable<IRedditSubreddit>, Task<IEnumerable<IRedditSubreddit>>> filterTaskGetter)
        {
            if (subredditCollection == null)
            {
                throw new ArgumentNullException(nameof(subredditCollection));
            }
            if (filterTaskGetter == null)
            {
                throw new ArgumentNullException(nameof(filterTaskGetter));
            }

            Task<IEnumerable<IRedditSubreddit>> filterTask = filterTaskGetter(subredditCollection);
            filterTask.Wait();

            return filterTask.Result;
        }

        #endregion
    }
}