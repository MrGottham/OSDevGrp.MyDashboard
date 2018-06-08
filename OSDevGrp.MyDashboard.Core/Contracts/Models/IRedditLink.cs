using System;
using System.Collections.Generic;

namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IRedditLink : IRedditCreatedThing, IRedditFilterable
    {
        IRedditSubreddit Subreddit { get; }

        string Title { get; }

        string SelftextAsText { get; }

        string SelftextAsHtml { get; }

        string Author { get; }

        Uri ThumbnailUrl { get; }

        IEnumerable<IRedditImage> Images { get; }

        string BannedBy { get; }

        DateTime? BannedAtTime { get; }

        DateTime? BannedAtUtcTime { get; }

        Uri Url { get; }
    }
}