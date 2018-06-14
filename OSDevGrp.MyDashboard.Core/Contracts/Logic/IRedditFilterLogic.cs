using System.Collections.Generic;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Contracts.Logic
{
    public interface IRedditFilterLogic
    {
        Task<IEnumerable<T>> RemoveUserBannedContentAsync<T>(IEnumerable<T> filterableCollection) where T : IRedditFilterable;

        Task<IEnumerable<T>> RemoveNsfwContentAsync<T>(IEnumerable<T> filterableCollection) where T : IRedditFilterable;

        Task<IEnumerable<T>> RemoveNoneNsfwContentAsync<T>(IEnumerable<T> subredditCollection) where T : IRedditFilterable;

        Task<IRedditThingComparer<T>> CreateComparerAsync<T>() where T : IRedditThing;
    }
}