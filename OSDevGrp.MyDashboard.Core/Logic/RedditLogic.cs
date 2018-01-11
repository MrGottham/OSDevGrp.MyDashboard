using System;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;

namespace OSDevGrp.MyDashboard.Core.Logic
{
    public class RedditLogic : IRedditLogic
    {
        #region Private variables

        private readonly IRedditRepository _redditRepository;
        private readonly IRedditRateLimitLogic _redditRateLimitLogic;
        private readonly IExceptionHandler _exceptionHandler;
        
        #endregion

        #region Constructor

        public RedditLogic(IRedditRepository redditRepository, IRedditRateLimitLogic redditRateLimitLogic, IExceptionHandler exceptionHandler)
        {
            if (redditRepository == null)
            {
                throw new ArgumentNullException(nameof(redditRepository));
            }
            if (redditRateLimitLogic == null)
            {
                throw new ArgumentNullException(nameof(redditRateLimitLogic));
            }
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(exceptionHandler));
            }

            _redditRepository = redditRepository;
            _redditRateLimitLogic = redditRateLimitLogic;
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
        
        #endregion
    }
}