using System;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Core.Contracts.Logic
{
    public interface IRedditRateLimitLogic
    {
        int Used { get; }

        int Remaining { get; }

        DateTime ResetTime { get; }

        DateTime ResetUtcTime { get; }

        bool WillExceedRateLimit(int expectedCalls);

        Task EnforceRateLimitAsync(int used, int remaining, DateTime? resetTime, DateTime receivedTime);
    }
}