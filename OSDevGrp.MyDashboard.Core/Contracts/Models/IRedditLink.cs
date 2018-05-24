using System;

namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IRedditLink : IRedditCreatedThing
    {
        IRedditSubreddit Subreddit { get; }

        string Title { get; }

        string SelftextAsText { get; }

        string SelftextAsHtml { get; }

        string Author { get; }

        Uri ThumbnailUrl { get; }

        bool Over18 { get; }
 
        Uri Url { get; }
    }
}