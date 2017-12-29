using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
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

        private readonly IDataProviderFactory _dataProviderFactory;
        private readonly IExceptionHandler _exceptionHandler;

        #endregion

        #region Constructor

        public RedditRepository(IDataProviderFactory dataProviderFactory, IExceptionHandler exceptionHandler)
        {
            if (dataProviderFactory == null)
            {
                throw new ArgumentNullException(nameof(dataProviderFactory));
            }
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(exceptionHandler));
            }

            _dataProviderFactory = dataProviderFactory;
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

        public Task<IRedditResponse<IRedditAuthenticatedUser>> GetAuthenticatedUserAsync(IRedditAccessToken accessToken)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            return Task.Run<IRedditResponse<IRedditAuthenticatedUser>>(() => 
            {
                try
                {
                    Task<IRedditResponse<RedditAuthenticatedUser>> getTask = GetAsync<RedditAuthenticatedUser>(new Uri($"{RedditApiUrl}/api/v1/me"), accessToken.TokenType, accessToken.AccessToken);
                    getTask.Wait();

                    return getTask.Result.As<IRedditAuthenticatedUser>();
                }
                catch (AggregateException ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                return null;
            });
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
                    case HttpStatusCode.Unauthorized:
                        throw new UnauthorizedAccessException("You are not authorized to perform this operation against Reddit.");
                        
                    default:
                        throw new Exception($"Unable to perform an operation against Reddit: {httpResponseMessage.ReasonPhrase}");
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
                    JsonSerialization.FromStream<TRedditObject>(stream));
            }
        }

        #endregion
    }
}