using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Logic
{
    public class RedditFilterLogic : IRedditFilterLogic
    {
        #region Methods

        public Task<IEnumerable<IRedditSubreddit>> RemoveUserBannedContentAsync(IEnumerable<IRedditSubreddit> subredditCollection)
        {
            if (subredditCollection == null)
            {
                throw new ArgumentNullException(nameof(subredditCollection));
            }

            return Task.Run<IEnumerable<IRedditSubreddit>>(() => subredditCollection.Where(subreddit => subreddit.UserIsBanned == false).ToList());
        }

        public Task<IEnumerable<IRedditSubreddit>> RemoveNsfwContentAsync(IEnumerable<IRedditSubreddit> subredditCollection)
        {
            if (subredditCollection == null)
            {
                throw new ArgumentNullException(nameof(subredditCollection));
            }

            return Task.Run<IEnumerable<IRedditSubreddit>>(() => subredditCollection.Where(subreddit => subreddit.Over18 == false).ToList());
        }

        public Task<IEnumerable<IRedditSubreddit>> RemoveNoneNsfwContentAsync(IEnumerable<IRedditSubreddit> subredditCollection)
        {
            if (subredditCollection == null)
            {
                throw new ArgumentNullException(nameof(subredditCollection));
            }

            return Task.Run<IEnumerable<IRedditSubreddit>>(() => subredditCollection.Where(subreddit => subreddit.Over18).ToList());
        }

        #endregion
    }
}