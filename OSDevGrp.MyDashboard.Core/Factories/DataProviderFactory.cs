using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Core.Repositories;

namespace OSDevGrp.MyDashboard.Core.Factories
{
    public class DataProviderFactory : IDataProviderFactory
    {
        #region Methods

        public Task<IEnumerable<INewsProvider>> BuildNewsProvidersAsync()
        {
            IEnumerable<INewsProvider> newsProviders = new List<INewsProvider>
            {
                new NewsProvider("DR", new Uri("http://www.dr.dk/nyheder/service/feeds/allenyheder")),
                new NewsProvider("TV 2", new Uri("http://feeds.tv2.dk/nyheder/rss")),
                new NewsProvider("Børsen", new Uri("http://borsen.dk/rss")),
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
                return new Uri($"https://www.reddit.com/api/v1/authorize?client_id={clientId}&response_type=code&state={state}&redirect_uri={redirectUri.AbsoluteUri}&duration=permanent&scope={scope}");
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

        public Task<IEnumerable<IRedditKnownSubreddit>> GetKnownNsfwSubredditsAsync()
        {
            Random random = new Random(DateTime.Now.Millisecond);            
            IEnumerable<IRedditKnownSubreddit> knownNsfwSubreddits = new List<IRedditKnownSubreddit>
            {
                new RedditKnownSubreddit("gonewildcurvy", CalculateRank(random)),
                new RedditKnownSubreddit("gonewildplus", CalculateRank(random)),
                new RedditKnownSubreddit("bigboobsgw", CalculateRank(random)),
                new RedditKnownSubreddit("homegrowntits", CalculateRank(random)),
                new RedditKnownSubreddit("milf", CalculateRank(random)),
                new RedditKnownSubreddit("gonewild30plus", CalculateRank(random)),
                new RedditKnownSubreddit("onmww", CalculateRank(random)),
                new RedditKnownSubreddit("wouldyoufuckmywife", CalculateRank(random)),
                new RedditKnownSubreddit("wifesharing", CalculateRank(random)),
                new RedditKnownSubreddit("gifsgonewild", CalculateRank(random)),
                new RedditKnownSubreddit("gwnerdy", CalculateRank(random)),
                new RedditKnownSubreddit("chubby", CalculateRank(random))
            };

            return Task.Run<IEnumerable<IRedditKnownSubreddit>>(() => knownNsfwSubreddits);
        }

        private int CalculateRank(Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }
            return random.Next(0, 1000);
        }
        

        #endregion 
    }
}