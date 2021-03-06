using System.Collections.Generic;

namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IDashboard
    {
        IEnumerable<INews> News { get; }

        IRedditAuthenticatedUser RedditAuthenticatedUser { get; }

        IEnumerable<IRedditSubreddit> RedditSubreddits { get; }

        IEnumerable<IRedditLink> RedditLinks { get; }

        IEnumerable<ISystemError> SystemErrors { get; }

        IDashboardSettings Settings { get; }

        IDashboardRules Rules { get; }

        void Replace(IEnumerable<INews> news);

        void Replace(IRedditAuthenticatedUser redditAuthenticatedUser);

        void Replace(IEnumerable<IRedditSubreddit> redditSubreddits);

        void Replace(IEnumerable<IRedditLink> redditLinks);

        void Replace(IEnumerable<ISystemError> systemErrors);

        void Replace(IDashboardSettings settings);
    }
}