using System;

namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IRedditCreatedThing : IRedditThing
    {
        DateTime CreatedTime { get; }

        DateTime CreatedUtcTime { get; }
    }
}