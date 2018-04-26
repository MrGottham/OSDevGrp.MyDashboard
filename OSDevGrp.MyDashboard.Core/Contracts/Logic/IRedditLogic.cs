using System.Collections.Generic;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Contracts.Logic
{
    public interface IRedditLogic
    {
        Task<IRedditAuthenticatedUser> GetAuthenticatedUserAsync(IRedditAccessToken accessToken);

        Task<IEnumerable<IRedditSubreddit>> GetSubredditsForAuthenticatedUserAsync(IRedditAccessToken accessToken, bool includeNsfwContent, bool onlyNsfwContent);

        Task<IRedditSubreddit> GetSpecificSubredditAsync(IRedditAccessToken accessToken, IRedditKnownSubreddit knownSubreddit);

        Task<IEnumerable<IRedditSubreddit>> GetNsfwSubredditsAsync(IRedditAccessToken accessToken, int numberOfSubreddits);
    }
}