using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Core.Utilities;

namespace OSDevGrp.MyDashboard.Core.Repositories
{
    public class RedditRepository : IRedditRepository
    {
        #region Private constants

        private const string RedditApiUrl = "https://oauth.reddit.com";

        #endregion

        #region Private variables

        private readonly IExceptionHandler _exceptionHandler;

        #endregion

        #region Constructor

        public RedditRepository(IExceptionHandler exceptionHandler)
        {
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(exceptionHandler));
            }

            _exceptionHandler = exceptionHandler;
        }

        #endregion

        #region Properties

        public static string UserAgent
        {
            get
            {
                return "windows:osdevgrp.mydashboard:v0.1.0 (by /u/mrgottham)";
            }
        }

        #endregion

        #region Methods

        public async Task<IRedditResponse<IRedditAuthenticatedUser>> GetAuthenticatedUserAsync(IRedditAccessToken accessToken)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            try
            {
                return (await GetAsync<RedditAuthenticatedUser>(new Uri($"{RedditApiUrl}/api/v1/me"), accessToken.TokenType, accessToken.AccessToken)).As<IRedditAuthenticatedUser>();
            }
            catch (AggregateException ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            return null;
        }

        public async Task<IRedditResponse<IRedditList<IRedditSubreddit>>> GetSubredditsForAuthenticatedUserAsync(IRedditAccessToken accessToken)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            try
            {
                IRedditResponse<RedditList<RedditSubreddit>> response = await GetAsync<RedditList<RedditSubreddit>>(new Uri($"{RedditApiUrl}/subreddits/mine/subscriber"), accessToken.TokenType, accessToken.AccessToken);

                return response.As<IRedditList<IRedditSubreddit>>(response.Data.As<IRedditSubreddit>());
            }
            catch (AggregateException ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            return null;
        }

        public async Task<IRedditResponse<IRedditSubreddit>> GetSpecificSubredditAsync(IRedditAccessToken accessToken, IRedditKnownSubreddit knownSubreddit)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            if (knownSubreddit == null)
            {
                throw new ArgumentNullException(nameof(knownSubreddit));
            }

            try
            {
                IRedditResponse<RedditListChild<RedditSubreddit>> response = await GetAsync<RedditListChild<RedditSubreddit>>(new Uri($"{RedditApiUrl}/r/{knownSubreddit.Name}/about"), accessToken.TokenType, accessToken.AccessToken);

                return response.As<IRedditSubreddit>(response.Data.Data);
            }
            catch (AggregateException ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            return null;
        }

        public async Task<IRedditResponse<IRedditList<IRedditLink>>> GetLinksAsync(IRedditAccessToken accessToken, IRedditSubreddit subreddit)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            if (subreddit == null)
            {
                throw new ArgumentNullException(nameof(subreddit));
            }

            try
            {
                if (subreddit.Url == null)
                {
                    return null;
                }

                string localPath = subreddit.Url.LocalPath;
                if (localPath.StartsWith("/"))
                {
                    localPath = localPath.Substring(1);
                }
                if (localPath.EndsWith("/"))
                {
                    localPath = localPath.Substring(0, localPath.Length - 1);
                }

                IRedditResponse<RedditList<RedditLink>> response = await GetAsync<RedditList<RedditLink>>(new Uri($"{RedditApiUrl}/{localPath}/new"), accessToken.TokenType, accessToken.AccessToken);

                foreach (RedditLink link in response.Data)
                {
                    link.Subreddit = subreddit;
                }

                return response.As<IRedditList<IRedditLink>>(response.Data.As<IRedditLink>());
            }
            catch (AggregateException ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            return null;
        }

        internal async Task<IRedditResponse<TRedditObject>> GetAsync<TRedditObject>(Uri requestUri, string authenticationHeaderScheme, string authenticationHeaderParameter) where TRedditObject : class, IRedditObject
        {
            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }
            if (string.IsNullOrWhiteSpace(authenticationHeaderScheme))
            {
                throw new ArgumentNullException(nameof(authenticationHeaderScheme));
            }
            if (string.IsNullOrWhiteSpace(authenticationHeaderParameter))
            {
                throw new ArgumentNullException(nameof(authenticationHeaderParameter));
            }

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpRequestMessage httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Get, requestUri, authenticationHeaderScheme, authenticationHeaderParameter))
                {
                    using (HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage))
                    {
                        return await HandleHttpResponseMessage<TRedditObject>(httpResponseMessage);
                    }
                }
            }
        }

        internal async static Task<IRedditResponse<TRedditObject>> PostAsync<TRedditObject>(Uri requestUri, string authenticationHeaderScheme, string authenticationHeaderParameter, IDictionary<string, string> formFields) where TRedditObject : class, IRedditObject
        {
            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }
            if (string.IsNullOrWhiteSpace(authenticationHeaderScheme))
            {
                throw new ArgumentNullException(nameof(authenticationHeaderScheme));
            }
            if (string.IsNullOrWhiteSpace(authenticationHeaderParameter))
            {
                throw new ArgumentNullException(nameof(authenticationHeaderParameter));
            }
            if (formFields == null)
            {
                throw new ArgumentNullException(nameof(formFields));
            }

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpRequestMessage httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Post, requestUri, authenticationHeaderScheme, authenticationHeaderParameter))
                {
                    httpRequestMessage.Content = new FormUrlEncodedContent(formFields);
                    using (HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage))
                    {
                        return await HandleHttpResponseMessage<TRedditObject>(httpResponseMessage);
                    }
                }
            }
        }

        private static HttpRequestMessage CreateHttpRequestMessage(HttpMethod httpMethod, Uri requestUri, string authenticationHeaderScheme, string authenticationHeaderParameter)
        {
            if (httpMethod == null)
            {
                throw new ArgumentNullException(nameof(httpMethod));
            }
            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }
            if (string.IsNullOrWhiteSpace(authenticationHeaderScheme))
            {
                throw new ArgumentNullException(nameof(authenticationHeaderScheme));
            }
            if (string.IsNullOrWhiteSpace(authenticationHeaderParameter))
            {
                throw new ArgumentNullException(nameof(authenticationHeaderParameter));
            }

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, requestUri);
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(authenticationHeaderScheme, authenticationHeaderParameter);
            httpRequestMessage.Headers.UserAgent.Clear();
            httpRequestMessage.Headers.TryAddWithoutValidation("User-Agent", UserAgent);
            return httpRequestMessage;
        }

        private async static Task<IRedditResponse<TRedditObject>> HandleHttpResponseMessage<TRedditObject>(HttpResponseMessage httpResponseMessage) where TRedditObject : class, IRedditObject
        {
            if (httpResponseMessage == null)
            {
                throw new ArgumentNullException(nameof(httpResponseMessage));
            }

            if (httpResponseMessage.IsSuccessStatusCode == false)
            {
                switch (httpResponseMessage.StatusCode)
                {
                    case HttpStatusCode.Forbidden:
                    case HttpStatusCode.Unauthorized:
                        throw new UnauthorizedAccessException("You are not authorized to perform this operation against Reddit.");

                    default:
                        throw new Exception($"Unable to perform an operation against Reddit ({httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri}): {httpResponseMessage.ReasonPhrase}");
                }
            }

            int rateLimitUsed = 0;
            int rateLimitRemaining = 0;
            DateTime? rateLimitResetTime = null;

            IEnumerable<string> values = null;
            if (httpResponseMessage.Headers.TryGetValues("X-Ratelimit-Used", out values))
            {
                if (values != null && values.Any())
                {
                    rateLimitUsed = int.Parse(values.First(), CultureInfo.InvariantCulture);
                }
            }

            values = null;
            if (httpResponseMessage.Headers.TryGetValues("X-Ratelimit-Remaining", out values))
            {
                if (values != null && values.Any())
                {
                    rateLimitRemaining = Convert.ToInt32(decimal.Parse(values.First(), CultureInfo.InvariantCulture));
                }
            }

            values = null;
            if (httpResponseMessage.Headers.TryGetValues("X-Ratelimit-Reset", out values))
            {
                if (values != null && values.Any())
                {
                    rateLimitResetTime = DateTime.Now.AddSeconds(int.Parse(values.First(), CultureInfo.InvariantCulture));
                }
            }

            using (Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync())
            {
                return new RedditResponse<TRedditObject>(
                    rateLimitUsed,
                    rateLimitRemaining,
                    rateLimitResetTime,
                    DateTime.Now,
                    JsonSerialization.FromStream<TRedditObject>(stream));
            }
        }

        #endregion
    }
}