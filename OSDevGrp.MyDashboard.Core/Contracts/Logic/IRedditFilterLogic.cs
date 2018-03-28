using System.Collections.Generic;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Contracts.Logic
{
    public interface IRedditFilterLogic
    {
        Task<IEnumerable<IRedditSubreddit>> RemoveUserBannedContentAsync(IEnumerable<IRedditSubreddit> subredditCollection);

        Task<IEnumerable<IRedditSubreddit>> RemoveNsfwContentAsync(IEnumerable<IRedditSubreddit> subredditCollection);

        Task<IEnumerable<IRedditSubreddit>> RemoveNoneNsfwContentAsync(IEnumerable<IRedditSubreddit> subredditCollection);
    }
}