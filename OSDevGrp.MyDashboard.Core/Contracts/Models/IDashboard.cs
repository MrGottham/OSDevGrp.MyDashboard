using System.Collections.Generic;

namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IDashboard
    {
        IEnumerable<INews> News { get; }

        IRedditAuthenticatedUser RedditAuthenticatedUser { get; }

        IEnumerable<ISystemError> SystemErrors { get; }

        IDashboardSettings Settings { get; }

        void Replace(IEnumerable<INews> news);

        void Replace(IRedditAuthenticatedUser redditAuthenticatedUser);

        void Replace(IEnumerable<ISystemError> systemErrors);

        void Replace(IDashboardSettings settings);
    }
}