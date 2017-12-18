using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Core.Utilities;

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

            return Task.Run(() => new Uri($"https://www.reddit.com/api/v1/authorize?client_id={clientId}&response_type=code&state={state}&redirect_uri={redirectUri.AbsoluteUri}&duration=permanent&scope=identity"));
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
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}")));
                    
                    IDictionary<string, string> formFields = new Dictionary<string, string>
                    {
                        {"grant_type", "authorization_code"},
                        {"code", code},
                        {"redirect_uri", redirectUri.AbsoluteUri}
                    };
                    using (HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("https://www.reddit.com/api/v1/access_token", new FormUrlEncodedContent(formFields)))
                    {
                        if (httpResponseMessage.IsSuccessStatusCode == false)
                        {
                            switch (httpResponseMessage.StatusCode)
                            {
                                case HttpStatusCode.Unauthorized:
                                    throw new UnauthorizedAccessException("Unable to get the access token from Reddit.");

                                default:
                                    string errorMessage = await httpResponseMessage.Content.ReadAsStringAsync();
                                    throw new Exception($"Unable to get the access token from Reddit: {errorMessage}");
                            }
                        }
                        using (Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync())
                        {
                            RedditAccessToken redditAccessToken = JsonSerialization.FromStream<RedditAccessToken>(stream);
                            if (string.IsNullOrWhiteSpace(redditAccessToken.Error))
                            {
                                return redditAccessToken;
                            }
                            throw new Exception($"Unable to get the access token from Reddit: {redditAccessToken.Error}");
                        }
                    }
                }
            });
        }

        #endregion 
    }
}