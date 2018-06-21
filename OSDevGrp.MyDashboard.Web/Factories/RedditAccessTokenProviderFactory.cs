using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class RedditAccessTokenProviderFactory : IRedditAccessTokenProviderFactory
    {
        #region Private variables

        private readonly IConfiguration _configuration;
        private readonly IDataProviderFactory _dataProviderFactory;

        #endregion

        #region Constructor

        public RedditAccessTokenProviderFactory(IConfiguration configuration, IDataProviderFactory dataProviderFactory)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (dataProviderFactory == null)
            {
                throw new ArgumentNullException(nameof(dataProviderFactory));
            }

            _configuration = configuration;
            _dataProviderFactory = dataProviderFactory;
        }

        #endregion

        #region Properties

        private string ClientId
        {
            get
            {
                return _configuration["Authentication:Reddit:ClientId"];
            }
        }

        private string ClientSecret
        {
            get
            {
                return _configuration["Authentication:Reddit:ClientSecret"];
            }
        }

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