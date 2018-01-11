using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Contracts.Logic
{
    public interface IRedditLogic
    {
        Task<IRedditAuthenticatedUser> GetAuthenticatedUserAsync(IRedditAccessToken accessToken);
    }
}