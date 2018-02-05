using System;

namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IRedditCreatedThing : IRedditThing, IDashboardItem
    {
        DateTime CreatedTime { get; }

        DateTime CreatedUtcTime { get; }
    }
}