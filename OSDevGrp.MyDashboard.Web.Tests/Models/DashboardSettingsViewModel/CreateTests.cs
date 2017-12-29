using System;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Web.Tests.Models.DashboardSettingsViewModel
{
    [TestClass]
    public class CreateTests
    {
        #region Private variables

        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullException("base64")]
        public void Create_WhenBase64IsNull_ThrowsArgumentNullException()
        {
            const string base64 = null;
            
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(base64);
        }
        
        [TestMethod]
        [ExpectedArgumentNullException("base64")]
        public void Create_WhenBase64IsEmpty_ThrowsArgumentNullException()
        {
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(string.Empty);
        }
        
        [TestMethod]
        [ExpectedArgumentNullException("base64")]
        public void Create_WhenBase64IsWhitespace_ThrowsArgumentNullException()
        {
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(" ");
        }
        
        [TestMethod]
        [ExpectedArgumentNullException("base64")]
        public void Create_WhenBase64IsWhitespaces_ThrowsArgumentNullException()
        {
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create("  ");
        }
        
        [TestMethod]
        public void Create_WhenCalledWithBase64String_ReturnsDashboardSettingsViewModel()
        {
            int numberOfNews = _random.Next(25, 50);
            bool useReddit = _random.Next(100) > 50;
            string redditAccessToken = _random.Next(100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string base64 = CreateSut(numberOfNews, useReddit, redditAccessToken).ToBase64();

            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel result = OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(base64);
            Assert.IsNotNull(result);
            Assert.AreEqual(numberOfNews, result.NumberOfNews);
            Assert.AreEqual(useReddit, result.UseReddit);
            Assert.AreEqual(redditAccessToken, result.RedditAccessToken);
        }
        
        [TestMethod]
        [ExpectedArgumentNullException("httpContext")]
        public void Create_WhenHttpContextIsNull_ThrowsArgumentNullException()
        {
            const HttpContext httpContext = null;

            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(httpContext);
        }
        
        [TestMethod]
        public void Create_WhenCalledWithHttpContext_AssertRequestWasCalledOnHttpContext()
        {
            Mock<HttpContext> httpContextMock = CreateHttpContextMock();

            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(httpContextMock.Object);

            httpContextMock.Verify(m => m.Request, Times.Once);
        }
        
        [TestMethod]
        public void Create_WhenCalledWithHttpContext_AssertCookiesWasCalledOnHttpRequest()
        {
            Mock<HttpRequest> httpRequestMock = CreateHttpRequestMock();
            HttpContext httpContex = CreateHttpContext(httpRequest: httpRequestMock.Object);

            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(httpContex);

            httpRequestMock.Verify(m => m.Cookies, Times.Once);
        }
        
        [TestMethod]
        public void Create_WhenCalledWithHttpContext_AssertContainsKeyWasCalledOnRequestCookieCollectionWithCookieName()
        {
            Mock<IRequestCookieCollection> requestCookieCollectionMock = CreateRequestCookieCollectionMock();
            HttpContext httpContex = CreateHttpContext(requestCookieCollection: requestCookieCollectionMock.Object);

            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(httpContex);

            requestCookieCollectionMock.Verify(m => m.ContainsKey(It.Is<string>(value => string.Compare(OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.CookieName, value, StringComparison.Ordinal) == 0)), Times.Once);
        }
        
        [TestMethod]
        public void Create_WhenCalledWithHttpContext_AssertItemWasCalledOnRequestCookieCollectionWithCookieName()
        {
            const bool containsCookie = true;
            string cookieValue = CreateSut().ToBase64();
            Mock<IRequestCookieCollection> requestCookieCollectionMock = CreateRequestCookieCollectionMock(containsCookie: containsCookie, cookieValue: cookieValue);
            HttpContext httpContex = CreateHttpContext(requestCookieCollection: requestCookieCollectionMock.Object);

            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(httpContex);

            requestCookieCollectionMock.Verify(m => m[It.Is<string>(value => string.Compare(OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.CookieName, value, StringComparison.Ordinal) == 0)], Times.Once);
        }
        
        [TestMethod]
        public void Create_WhenCalledWithHttpContextWhereRequestIsNull_ReturnsNull()
        {
            const bool hasHttpRequest = false;
            HttpContext httpContext = CreateHttpContext(hasHttpRequest: hasHttpRequest);

            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel result = OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(httpContext);

            Assert.IsNull(result);
        }
        
        [TestMethod]
        public void Create_WhenCalledWithHttpContextWhereCookiesInRequestIsNull_ReturnsNull()
        {
            const bool hasRequestCookieCollection = false;
            HttpContext httpContext = CreateHttpContext(hasRequestCookieCollection: hasRequestCookieCollection);

            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel result = OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(httpContext);

            Assert.IsNull(result);
        }
        
        [TestMethod]
        public void Create_WhenCalledWithHttpContextWhereCookiesInRequestDoesNotContainCookie_ReturnsNull()
        {
            const bool containsCookie = false;
            HttpContext httpContext = CreateHttpContext(containsCookie: containsCookie);

            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel result = OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(httpContext);

            Assert.IsNull(result);
        }
        
        [TestMethod]
        public void Create_WhenCalledWithHttpContextWhereCookiesInRequestDoesContainCookie_ReturnsDashboardSettingsViewModel()
        {
            const bool containsCookie = true;
            int numberOfNews = _random.Next(25, 50);
            bool useReddit = _random.Next(100) > 50;
            string redditAccessToken = _random.Next(100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string cookieValue = CreateSut(numberOfNews, useReddit, redditAccessToken).ToBase64();
            HttpContext httpContext = CreateHttpContext(containsCookie: containsCookie, cookieValue: cookieValue);

            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel result = OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(httpContext);
            Assert.IsNotNull(result);
            Assert.AreEqual(numberOfNews, result.NumberOfNews);
            Assert.AreEqual(useReddit, result.UseReddit);
            Assert.AreEqual(redditAccessToken, result.RedditAccessToken);
        }
        
        private OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel CreateSut(int? numberOfNews = null, bool? useReddit = null, string redditAccessToken = null)
        {
            return new OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel
            {
                NumberOfNews = numberOfNews ?? _random.Next(25, 50),
                UseReddit = useReddit ?? _random.Next(100) > 50,
                RedditAccessToken = redditAccessToken
            };
        }

        public HttpContext CreateHttpContext(bool hasHttpRequest = true, HttpRequest httpRequest = null, bool hasRequestCookieCollection = true, IRequestCookieCollection requestCookieCollection = null, bool containsCookie = false, string cookieValue = null)
        {
            return CreateHttpContextMock(hasHttpRequest, httpRequest, hasRequestCookieCollection, requestCookieCollection, containsCookie, cookieValue).Object;
        }

        private Mock<HttpContext> CreateHttpContextMock(bool hasHttpRequest = true, HttpRequest httpRequest = null, bool hasRequestCookieCollection = true, IRequestCookieCollection requestCookieCollection = null, bool containsCookie = false, string cookieValue = null)
        {
            Mock<HttpContext> httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(m => m.Request)
                .Returns(hasHttpRequest == false ? (HttpRequest) null : httpRequest ?? CreateHttpRequest(hasRequestCookieCollection, requestCookieCollection, containsCookie, cookieValue));
            return httpContextMock;
        }

        private HttpRequest CreateHttpRequest(bool hasRequestCookieCollection = true, IRequestCookieCollection requestCookieCollection = null, bool containsCookie = false, string cookieValue = null)
        {
            return CreateHttpRequestMock(hasRequestCookieCollection, requestCookieCollection, containsCookie, cookieValue).Object;
        }

        private Mock<HttpRequest> CreateHttpRequestMock(bool hasRequestCookieCollection = true, IRequestCookieCollection requestCookieCollection = null, bool containsCookie = false, string cookieValue = null)
        {
            Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(m => m.Cookies)
                .Returns(hasRequestCookieCollection == false ? (IRequestCookieCollection) null : requestCookieCollection ?? CreateRequestCookieCollection(containsCookie, cookieValue));
            return httpRequestMock;
        }

        private IRequestCookieCollection CreateRequestCookieCollection(bool containsCookie = false, string cookieValue = null)
        {
            return CreateRequestCookieCollectionMock(containsCookie, cookieValue).Object;
        }

        private Mock<IRequestCookieCollection> CreateRequestCookieCollectionMock(bool containsCookie = false, string cookieValue = null)
        {
            Mock<IRequestCookieCollection> requestCookieCollectionMock = new Mock<IRequestCookieCollection>();
            requestCookieCollectionMock.Setup(m => m.ContainsKey(It.IsAny<string>()))
                .Returns(containsCookie);
            requestCookieCollectionMock.Setup(m => m[It.IsAny<string>()])
                .Returns(cookieValue);
            return requestCookieCollectionMock;
        }
    }
}