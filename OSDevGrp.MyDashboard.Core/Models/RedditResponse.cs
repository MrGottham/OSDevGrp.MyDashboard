using System;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    public class RedditResponse<TRedditObject> : RedditObjectBase, IRedditResponse<TRedditObject> where TRedditObject : IRedditObject
    {
        #region Constructor

        public RedditResponse(int rateLimitUsed, int rateLimitRemaining, DateTime? rateLimitResetTime, DateTime receivedTime, TRedditObject data)
        {
            if (Equals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            RateLimitUsed = rateLimitUsed;
            RateLimitRemaining = rateLimitRemaining;
            RateLimitResetTime = rateLimitResetTime;
            ReceivedTime = receivedTime;
            Data = data;
        }

        #endregion

        #region Properties

        public int RateLimitUsed { get; private set; }

        public int RateLimitRemaining { get; private set; }

        public DateTime? RateLimitResetTime
        {
            get
            {
                return RateLimitResetUtcTime.HasValue ? RateLimitResetUtcTime.Value.ToLocalTime() : (DateTime?) null;
            }
            private set
            {
                RateLimitResetUtcTime = value.HasValue ? value.Value.ToUniversalTime() : (DateTime?) null;
            }
        }

        public DateTime? RateLimitResetUtcTime { get; private set; }

        public DateTime ReceivedTime
        {
            get
            {
                return ReceivedUtcTime.ToLocalTime();
            }
            private set
            {
                ReceivedUtcTime = value.ToUniversalTime();
            }
        }

        public DateTime ReceivedUtcTime
        {
            get;
            private set;
        }

        public TRedditObject Data { get; private set; }

        #endregion

        #region Methods

        public IRedditResponse<TTargetRedditObject> As<TTargetRedditObject>() where TTargetRedditObject : class, IRedditObject
        {
            return new RedditResponse<TTargetRedditObject>(
                RateLimitUsed,
                RateLimitRemaining,
                RateLimitResetTime,
                ReceivedTime,
                Data as TTargetRedditObject);
        }
        
        #endregion
    }
}