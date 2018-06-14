using System;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Logic
{
    public class RedditThingComparer<T> : IRedditThingComparer<T> where T : IRedditThing
    {
        #region Methods

        public bool Equals(T x, T y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }

            return string.Compare(x.FullName, y.FullName, true) == 0;
        }

        public int GetHashCode(T redditThing)
        {
            if (redditThing == null)
            {
                throw new ArgumentNullException(nameof(redditThing));
            }

            string fullName = redditThing.FullName;
            if (fullName == null)
            {
                throw new ArgumentException("The full name for the Reddit thing has not been initialized.", nameof(redditThing.FullName));
            }

            return fullName.GetHashCode();
        }

        #endregion
    }
}