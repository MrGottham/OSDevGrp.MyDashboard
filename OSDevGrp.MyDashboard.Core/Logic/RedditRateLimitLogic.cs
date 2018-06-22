using System;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;

namespace OSDevGrp.MyDashboard.Core.Logic
{
    public class RedditRateLimitLogic : IRedditRateLimitLogic
    {
        #region Private constants

        private const int LimitRequestsPrMinute = 60;

        #endregion

        #region Private variables

        private readonly IExceptionHandler _exceptionHandler;
        private readonly object SyncRoot = new object();

        #endregion

        #region Constructor

        public RedditRateLimitLogic(IExceptionHandler exceptionHandler)
        {
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(exceptionHandler));
            }

            _exceptionHandler = exceptionHandler;

            Used = 0;
            Remaining = LimitRequestsPrMinute;
            ResetUtcTime = DateTime.UtcNow.AddSeconds(60);
        }

        #endregion

        #region Properties

        public int Used { get; protected set; }

        public int Remaining { get; protected set; }

        public DateTime ResetTime
        {
            get
            {
                return ResetUtcTime.ToLocalTime();
            }
            protected set
            {
                ResetUtcTime = value.ToUniversalTime();
            }
        }

        public DateTime ResetUtcTime { get; protected set; }

        protected DateTime? LatestReceivedTime
        {
            get
            {
                return LatestReceivedUtcTime.HasValue ? LatestReceivedUtcTime.Value.ToLocalTime() : (DateTime?) null;
            }
            set
            {
                LatestReceivedUtcTime = value.HasValue ? value.Value.ToUniversalTime() : (DateTime?) null;
            }
        }

        protected DateTime? LatestReceivedUtcTime { get; set; }

        #endregion

        #region Methods

        public bool WillExceedRateLimit(int expectedCalls)
        {
            lock (SyncRoot)
            {
                return DateTime.UtcNow < ResetUtcTime && expectedCalls > Remaining;
            }
        } 

        public Task EnforceRateLimitAsync(int used, int remaining, DateTime? resetTime, DateTime receivedTime)
        {
            return Task.Run(() =>
            {
                try
                {
                    lock (SyncRoot)
                    {
                        if (LatestReceivedUtcTime.HasValue == false || LatestReceivedUtcTime < receivedTime.ToUniversalTime())
                        {
                            if (resetTime.HasValue && resetTime.Value.ToUniversalTime() > ResetUtcTime)
                            {
                                Used = used;
                                Remaining = remaining;
                                ResetUtcTime = resetTime.Value.ToUniversalTime();
                            }
                            if (Used < used)
                            {
                                Used = used;
                            }
                            if (Remaining > remaining)
                            {
                                Remaining = remaining;
                            }
                            LatestReceivedUtcTime = receivedTime.ToUniversalTime();
                        }
                    }
                }
                catch (AggregateException ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
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