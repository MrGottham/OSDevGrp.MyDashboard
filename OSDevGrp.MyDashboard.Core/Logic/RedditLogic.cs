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
        
        public Task<IRedditAccessToken> RenewAccessTokenAsync(IRedditAccessToken accessToken)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            return Task.Run<IRedditAccessToken>(() => 
            {
                try
                {
                    if (DateTime.Now <= accessToken.Expires)
                    {
                        return accessToken;
                    }

                    Task<IRedditAccessToken> renewRedditAccessTokenTask = _redditAccessTokenProviderFactory.RenewRedditAccessTokenAsync(accessToken.RefreshToken);
                    renewRedditAccessTokenTask.Wait();

                    return renewRedditAccessTokenTask.Result;
                }
                catch (AggregateException ex)
                {
                    _exceptionHandler.HandleAsync(ex);
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex);
                }
                return null;
            });
        }

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
                    if (response == null)
                    {
                        return null;
                    }

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
                    if (response == null)
                    {
                        return new List<IRedditSubreddit>(0);
                    }

                    Task enforceRateLimitTask = _redditRateLimitLogic.EnforceRateLimitAsync(response.RateLimitUsed, response.RateLimitRemaining, response.RateLimitResetTime, response.ReceivedTime);
                    enforceRateLimitTask.Wait();

                    return ApplyFilters(response.Data, includeNsfwContent, onlyNsfwContent)
                        .OrderByDescending(subreddit => subreddit.Subscribers)
                        .ToList();
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
                try
                {
                    if (_redditRateLimitLogic.WillExceedRateLimit(1))
                    {
                        return null;
                    }

                    Task<IRedditResponse<IRedditSubreddit>> getSpecificSubredditTask = _redditRepository.GetSpecificSubredditAsync(accessToken, knownSubreddit);
                    getSpecificSubredditTask.Wait();

                    IRedditResponse<IRedditSubreddit> response = getSpecificSubredditTask.Result;
                    if (response == null)
                    {
                        return null;
                    }

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

                    Task<IRedditSubreddit>[] getSpecificSubredditTaskArray = knownNsfwSubreddits.Take(numberOfSubredditsToGet)
                        .Select(knownNsfwSubreddit => GetSpecificSubredditAsync(accessToken, knownNsfwSubreddit))
                        .ToArray();
                    Task.WaitAll(getSpecificSubredditTaskArray);

                    return getSpecificSubredditTaskArray
                        .Where(getSpecificSubredditTask => getSpecificSubredditTask.IsCompleted && getSpecificSubredditTask.IsFaulted == false && getKnownNsfwSubredditsTask.IsCanceled == false)
                        .Select(getSpecificSubredditTask => getSpecificSubredditTask.Result)
                        .Where(subreddit => subreddit != null)
                        .OrderByDescending(subreddit => subreddit.Subscribers)
                        .ToList();
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

        public Task<IEnumerable<IRedditLink>> GetLinksAsync(IRedditAccessToken accessToken, IRedditSubreddit subreddit, bool includeNsfwContent, bool onlyNsfwContent)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            if (subreddit == null)
            {
                throw new ArgumentNullException(nameof(subreddit));
            }

            return Task.Run<IEnumerable<IRedditLink>>(() =>
            {
                try
                {
                    if (_redditRateLimitLogic.WillExceedRateLimit(1))
                    {
                        return new List<IRedditLink>(0);
                    }

                    Task<IRedditResponse<IRedditList<IRedditLink>>> getLinksTask = _redditRepository.GetLinksAsync(accessToken, subreddit);
                    getLinksTask.Wait();

                    IRedditResponse<IRedditList<IRedditLink>> response = getLinksTask.Result;
                    if (response == null)
                    {
                        return new List<IRedditLink>(0);
                    }

                    Task enforceRateLimitTask = _redditRateLimitLogic.EnforceRateLimitAsync(response.RateLimitUsed, response.RateLimitRemaining, response.RateLimitResetTime, response.ReceivedTime);
                    enforceRateLimitTask.Wait();

                    return ApplyFilters(response.Data, includeNsfwContent, onlyNsfwContent)
                        .OrderByDescending(link => link.CreatedTime)
                        .ToList();
                }
                catch (AggregateException ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                return new List<IRedditLink>(0);
            });
        }

        public Task<IEnumerable<IRedditLink>> GetLinksAsync(IRedditAccessToken accessToken, IEnumerable<IRedditSubreddit> subredditCollection, bool includeNsfwContent, bool onlyNsfwContent)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            if (subredditCollection == null)
            {
                throw new ArgumentNullException(nameof(subredditCollection));
            }

            return Task.Run<IEnumerable<IRedditLink>>(() =>
            {
                try
                {
                    if (_redditRateLimitLogic.WillExceedRateLimit(subredditCollection.Count()))
                    {
                        return new List<IRedditLink>(0);
                    }

                    Task<IEnumerable<IRedditLink>>[] getLinksTaskArray = subredditCollection
                        .Select(subreddit => GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent))
                        .ToArray();
                    Task.WaitAll(getLinksTaskArray);

                    Task<IRedditThingComparer<IRedditLink>> createComparerTask = _redditFilterLogic.CreateComparerAsync<IRedditLink>();
                    createComparerTask.Wait();

                    return getLinksTaskArray
                        .Where(getLinksTask => getLinksTask.IsCompleted && getLinksTask.IsFaulted == false && getLinksTask.IsCanceled == false)
                        .SelectMany(getLinksTask => getLinksTask.Result)
                        .Distinct(createComparerTask.Result)
                        .OrderByDescending(link => link.CreatedTime)
                        .ToList();
                }
                catch (AggregateException ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                return new List<IRedditLink>(0);
            });
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

            Task<IEnumerable<T>> filterTask = filterTaskGetter(filterableCollection);
            filterTask.Wait();

            return filterTask.Result;
        }

        #endregion
    }
}