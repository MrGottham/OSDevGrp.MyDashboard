using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Contracts.Repositories
{
    public interface IRedditRepository
    {
        Task<IRedditResponse<IRedditAuthenticatedUser>> GetAuthenticatedUserAsync(IRedditAccessToken accessToken);

        Task<IRedditResponse<IRedditList<IRedditSubreddit>>> GetSubredditsForAuthenticatedUserAsync(IRedditAccessToken accessToken);

        Task<IRedditResponse<IRedditSubreddit>> GetSpecificSubredditAsync(IRedditAccessToken accessToken, IRedditKnownSubreddit knownSubreddit);
    }
}