using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Utilities;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Helpers.CookieHelper
{
    public abstract class TestBase
    {
        protected static DashboardSettingsViewModel BuildDashboardSettingsViewModel(Random random, string redditAccessToken = null)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            return new DashboardSettingsViewModel
            {
                NumberOfNews = random.Next(25, 50),
                UseReddit = random.Next(100) > 50,
                AllowNsfwContent = random.Next(100) > 50,
                IncludeNsfwContent = random.Next(100) > 50,
                OnlyNsfwContent = random.Next(100) > 50,
                RedditAccessToken = redditAccessToken,
                ExportData = random.Next(100) > 50
            };
        }

        protected static IRedditAccessToken BuildRedditAccessToken(Random random, int? expiresIn = null, DateTime? received = null)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            return new MyRedditAccessToken(
                Guid.NewGuid().ToString("D"),
                Guid.NewGuid().ToString("D"),
                expiresIn ?? random.Next(30, 60) * 60,
                Guid.NewGuid().ToString("D"),
                Guid.NewGuid().ToString("D"),
                received ?? DateTime.UtcNow);
        }

        protected static string BuildRedditAccessTokenAsBase64(Random random, int? expiresIn = null, DateTime? received = null)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            IRedditAccessToken redditAccessToken = BuildRedditAccessToken(random, expiresIn, received);
            return Convert.ToBase64String(JsonSerialization.ToByteArray(redditAccessToken));
        }

        protected static DashboardViewModel BuildDashboardViewModel()
        {
            return new DashboardViewModel();
        }

        protected static string BuildBase64String(string value = null)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value ?? Guid.NewGuid().ToString())); 
        }

        protected static HttpContext BuildHttpContext(bool hasHttpRequest = true, HttpRequest httpRequest = null, bool hasHttpResponse = true, HttpResponse httpResponse = null)
        {
            return BuildHttpContextMock(hasHttpRequest, httpRequest, hasHttpResponse, httpResponse).Object;
        }

        protected static Mock<HttpContext> BuildHttpContextMock(bool hasHttpRequest = true, HttpRequest httpRequest = null, bool hasHttpResponse = true, HttpResponse httpResponse = null)
        {
            Mock<HttpContext> httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(m => m.Request)
                .Returns(hasHttpRequest ? httpRequest ?? BuildHttpRequest() : null);
            httpContextMock.Setup(m => m.Response)
                .Returns(hasHttpResponse ? httpResponse ?? BuildHttpResponse() : null);
            return httpContextMock;
        }

        protected static HttpRequest BuildHttpRequest(bool hasRequestCookieCollection = true, IRequestCookieCollection requestCookieCollection = null)
        {
            return BuildHttpRequestMock(hasRequestCookieCollection, requestCookieCollection).Object;
        }

        protected static Mock<HttpRequest> BuildHttpRequestMock(bool hasRequestCookieCollection = true, IRequestCookieCollection requestCookieCollection = null)
        {
            Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(m => m.Cookies)
                .Returns(requestCookieCollection ?? BuildRequestCookieCollection());
            return httpRequestMock;
        }

        protected static IRequestCookieCollection BuildRequestCookieCollection(string dashboardSettingsCookieValue = null, string dashboardCookieValue = null)
        {
            return BuildRequestCookieCollectionMock(dashboardSettingsCookieValue, dashboardCookieValue).Object;
        }

        protected static Mock<IRequestCookieCollection> BuildRequestCookieCollectionMock(string dashboardSettingsCookieValue = null, string dashboardCookieValue = null)
        {
            Mock<IRequestCookieCollection> requestCookieCollectionMock = new Mock<IRequestCookieCollection>();
            requestCookieCollectionMock.Setup(m => m.ContainsKey(It.Is<string>(value => string.CompareOrdinal(value, DashboardSettingsViewModel.CookieName) == 0)))
                .Returns(string.IsNullOrWhiteSpace(dashboardSettingsCookieValue) == false);
            requestCookieCollectionMock.Setup(m => m.ContainsKey(It.Is<string>(value => string.CompareOrdinal(value, DashboardViewModel.CookieName) == 0)))
                .Returns(string.IsNullOrWhiteSpace(dashboardCookieValue) == false);
            requestCookieCollectionMock.Setup(m => m[It.Is<string>(value => string.CompareOrdinal(value, DashboardSettingsViewModel.CookieName) == 0)])
                .Returns(dashboardSettingsCookieValue);
            requestCookieCollectionMock.Setup(m => m[It.Is<string>(value => string.CompareOrdinal(value, DashboardViewModel.CookieName) == 0)])
                .Returns(dashboardCookieValue);
            return requestCookieCollectionMock; 
        }

        protected static HttpResponse BuildHttpResponse(bool hasResponseCookies = true, IResponseCookies responseCookies = null)
        {
            return BuildHttpResponseMock(hasResponseCookies, responseCookies).Object;
        }

        protected static Mock<HttpResponse> BuildHttpResponseMock(bool hasResponseCookies = true, IResponseCookies responseCookies = null)
        {
            Mock<HttpResponse> httpResponseMock = new Mock<HttpResponse>();
            httpResponseMock.Setup(m => m.Cookies)
                .Returns(hasResponseCookies ? responseCookies ?? BuildResponseCookies() : null);
            return httpResponseMock;
        }

        protected static IResponseCookies BuildResponseCookies()
        {
            return BuildResponseCookiesMock().Object;
        }

        protected static Mock<IResponseCookies> BuildResponseCookiesMock()
        {
            return new Mock<IResponseCookies>();
        }
    }
}