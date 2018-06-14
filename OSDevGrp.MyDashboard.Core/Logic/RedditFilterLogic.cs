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

        public Task<IEnumerable<T>> RemoveUserBannedContentAsync<T>(IEnumerable<T> filterableCollection) where T : IRedditFilterable
        {
            if (filterableCollection == null)
            {
                throw new ArgumentNullException(nameof(filterableCollection));
            }

            return Task.Run<IEnumerable<T>>(() => filterableCollection.Where(filterable => filterable.UserBanned == false).ToList());
        }

        public Task<IEnumerable<T>> RemoveNsfwContentAsync<T>(IEnumerable<T> filterableCollection) where T : IRedditFilterable
        {
            if (filterableCollection == null)
            {
                throw new ArgumentNullException(nameof(filterableCollection));
            }

            return Task.Run<IEnumerable<T>>(() => filterableCollection.Where(filterable => filterable.Over18 == false).ToList());
        }

        public Task<IEnumerable<T>> RemoveNoneNsfwContentAsync<T>(IEnumerable<T> filterableCollection) where T : IRedditFilterable
        {
            if (filterableCollection == null)
            {
                throw new ArgumentNullException(nameof(filterableCollection));
            }

            return Task.Run<IEnumerable<T>>(() => filterableCollection.Where(filterable => filterable.Over18).ToList());
        }

        public Task<IRedditThingComparer<T>> CreateComparerAsync<T>() where T : IRedditThing
        {
            return Task.Run<IRedditThingComparer<T>>(() => new RedditThingComparer<T>());
        }

        #endregion
    }
}