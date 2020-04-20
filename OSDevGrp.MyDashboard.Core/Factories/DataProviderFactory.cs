using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Core.Repositories;

namespace OSDevGrp.MyDashboard.Core.Factories
{
    public class DataProviderFactory : IDataProviderFactory
    {
        #region Private variables

        private readonly IRandomizer _randomizer;

        #endregion

        #region Constructor

        public DataProviderFactory(IRandomizer randomizer)
        {
            if (randomizer == null)
            {
                throw new ArgumentNullException(nameof(randomizer));
            }

            _randomizer = randomizer;
        }

        #endregion
        
        #region Methods

        public Task<IEnumerable<INewsProvider>> BuildNewsProvidersAsync()
        {
            IEnumerable<INewsProvider> newsProviders = new List<INewsProvider>
            {
                new NewsProvider("DR", new Uri("https://www.dr.dk/nyheder/service/feeds/allenyheder")),
                new NewsProvider("TV 2 Lorry", new Uri("https://www.tv2lorry.dk/rss")),
                new NewsProvider("Børsen", new Uri("https://borsen.dk/rss")),
                new NewsProvider("Computerworld", new Uri("https://www.computerworld.dk/rss/all")),
                new NewsProvider("Version2", new Uri("https://www.version2.dk/it-nyheder/rss"))
            };

            return Task.Run(() => newsProviders);
        }

        public Task<Uri> AcquireRedditAuthorizationTokenAsync(string clientId, string state, Uri redirectUri)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            if (string.IsNullOrWhiteSpace(state))
            {
                throw new ArgumentNullException(nameof(state));
            }
            if (redirectUri == null)
            {
                throw new ArgumentNullException(nameof(redirectUri));
            }

            return Task.Run(() => 
            {
                const string scope = "identity privatemessages mysubreddits read";
                return new Uri($"https://www.reddit.com/api/v1/authorize?client_id={Uri.EscapeDataString(clientId)}&response_type=code&state={Uri.EscapeDataString(state)}&redirect_uri={redirectUri.AbsoluteUri}&duration=permanent&scope={Uri.EscapeDataString(scope)}");
            });
        }

        public Task<IRedditAccessToken> GetRedditAccessTokenAsync(string clientId, string clientSecret, string code, Uri redirectUri)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }
            if (redirectUri == null)
            {
                throw new ArgumentNullException(nameof(redirectUri));
            }

            return Task.Run<IRedditAccessToken>(async () => 
            {
                IDictionary<string, string> formFields = new Dictionary<string, string>
                {
                    {"grant_type", "authorization_code"},
                    {"code", code},
                    {"redirect_uri", redirectUri.AbsoluteUri}
                };

                IRedditResponse<RedditAccessToken> redditResponse = await RedditRepository.PostAsync<RedditAccessToken>(
                    new Uri("https://www.reddit.com/api/v1/access_token"),
                    "Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}")),
                    formFields);

                RedditAccessToken redditAccessToken = redditResponse.Data;
                if (string.IsNullOrWhiteSpace(redditAccessToken.Error))
                {
                    return redditAccessToken;
                }
                throw new Exception($"Unable to get the access token from Reddit: {redditAccessToken.Error}");
            });
        }

        public Task<IRedditAccessToken> RenewRedditAccessTokenAsync(string clientId, string clientSecret, string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentNullException(nameof(refreshToken));
            }

            return Task.Run<IRedditAccessToken>(async () => 
            {
                IDictionary<string, string> formFields = new Dictionary<string, string>
                {
                    {"grant_type", "refresh_token"},
                    {"refresh_token", refreshToken}
                };

                IRedditResponse<RedditAccessToken> redditResponse = await RedditRepository.PostAsync<RedditAccessToken>(
                    new Uri("https://www.reddit.com/api/v1/access_token"),
                    "Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}")),
                    formFields);

                RedditAccessToken redditAccessToken = redditResponse.Data;
                if (string.IsNullOrWhiteSpace(redditAccessToken.Error))
                {
                    return redditAccessToken;
                }
                throw new Exception($"Unable to renew the access token from Reddit: {redditAccessToken.Error}");
            });
        }

        public Task<IEnumerable<IRedditKnownSubreddit>> GetKnownNsfwSubredditsAsync()
        {
            IEnumerable<IRedditKnownSubreddit> knownNsfwSubreddits = new List<IRedditKnownSubreddit>
            {
                new RedditKnownSubreddit("gonewildcurvy", CalculateRank()),
                new RedditKnownSubreddit("gonewildplus", CalculateRank()),
                new RedditKnownSubreddit("bigboobsgw", CalculateRank()),
                new RedditKnownSubreddit("homegrowntits", CalculateRank()),
                new RedditKnownSubreddit("hugehangers", CalculateRank()),
                new RedditKnownSubreddit("cleavagegw", CalculateRank()),
                new RedditKnownSubreddit("milf", CalculateRank()),
                new RedditKnownSubreddit("gonewild30plus", CalculateRank()),
                new RedditKnownSubreddit("40plusgonewild", CalculateRank()),
                new RedditKnownSubreddit("gonewild50plus", CalculateRank()),
                new RedditKnownSubreddit("onmww", CalculateRank()),
                new RedditKnownSubreddit("wouldyoufuckmywife", CalculateRank()),
                new RedditKnownSubreddit("wifesharing", CalculateRank()),
                new RedditKnownSubreddit("gifsgonewild", CalculateRank()),
                new RedditKnownSubreddit("gwnerdy", CalculateRank()),
                new RedditKnownSubreddit("chubby", CalculateRank()),
                new RedditKnownSubreddit("swingersgw", CalculateRank())
            };

            return Task.Run<IEnumerable<IRedditKnownSubreddit>>(() => knownNsfwSubreddits);
        }

        private int CalculateRank()
        {
            return _randomizer.Next(0, 1000);
        }
 
        #endregion 
    }
}