using System;

namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IRedditResponse<TRedditObject> : IRedditObject where TRedditObject : IRedditObject
    {
        int RateLimitUsed { get; }

        int RateLimitRemaining { get; }

        DateTime? RateLimitResetTime { get; }

        DateTime? RateLimitResetUtcTime { get; }

        DateTime ReceivedTime { get; }

        DateTime ReceivedUtcTime { get; }

        TRedditObject Data { get; }

        IRedditResponse<TTargetRedditObject> As<TTargetRedditObject>() where TTargetRedditObject : class, IRedditObject;
    }
}