using Microsoft.Extensions.Options;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Options;
using System;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class RedditAccessTokenProviderFactory : IRedditAccessTokenProviderFactory
    {
        #region Private variables

        private readonly IOptions<RedditOptions> _redditOptions;
        private readonly IDataProviderFactory _dataProviderFactory;

        #endregion

        #region Constructor

        public RedditAccessTokenProviderFactory(IOptions<RedditOptions> redditOptions, IDataProviderFactory dataProviderFactory)
        {
            if (redditOptions == null)
            {
                throw new ArgumentNullException(nameof(redditOptions));
            }
            if (dataProviderFactory == null)
            {
                throw new ArgumentNullException(nameof(dataProviderFactory));
            }

            _redditOptions = redditOptions;
            _dataProviderFactory = dataProviderFactory;
        }

        #endregion

        #region Properties

        private string ClientId => _redditOptions.Value.ClientId;

        private string ClientSecret => _redditOptions.Value.ClientSecret;

        #endregion

        #region Methods

        public Task<Uri> AcquireRedditAuthorizationTokenAsync(string state, Uri redirectUri)
        {
            if (string.IsNullOrWhiteSpace(state))
            {
                throw new ArgumentNullException(nameof(state));
            }
            if (redirectUri == null)
            {
                throw new ArgumentNullException(nameof(redirectUri));
            }

            return _dataProviderFactory.AcquireRedditAuthorizationTokenAsync(ClientId, state, redirectUri);
        }

        public Task<IRedditAccessToken> GetRedditAccessTokenAsync(string code, Uri redirectUri)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }
            if (redirectUri == null)
            {
                throw new ArgumentNullException(nameof(redirectUri));
            }

            return _dataProviderFactory.GetRedditAccessTokenAsync(ClientId, ClientSecret, code, redirectUri);
        }

        public Task<IRedditAccessToken> RenewRedditAccessTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentNullException(nameof(refreshToken));
            }

            return _dataProviderFactory.RenewRedditAccessTokenAsync(ClientId, ClientSecret, refreshToken);
        }

        #endregion
    }
}