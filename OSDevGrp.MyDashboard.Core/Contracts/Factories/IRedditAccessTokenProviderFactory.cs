using System;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Contracts.Factories
{
    public interface IRedditAccessTokenProviderFactory
    {
        Task<Uri> AcquireRedditAuthorizationTokenAsync(string state, Uri redirectUri);

        Task<IRedditAccessToken> GetRedditAccessTokenAsync(string code, Uri redirectUri);

        Task<IRedditAccessToken> RenewRedditAccessTokenAsync(string refreshToken);
    }
}