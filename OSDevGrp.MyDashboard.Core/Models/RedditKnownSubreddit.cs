using System;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    public class RedditKnownSubreddit : RedditObjectBase, IRedditKnownSubreddit
    {
        #region Constructor

        public RedditKnownSubreddit(string name, int rank)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Rank = rank;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public int Rank { get; private set; }

        #endregion
    }
}