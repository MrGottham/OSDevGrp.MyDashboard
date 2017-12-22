using System;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Web.Tests.Models.DashboardSettingsViewModel
{
    [TestClass]
    public class ToCookieTests
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
        [ExpectedArgumentNullException("httpContext")]
        public void ToCookie_WhenHttpContextIsNull_ThrowsArgumentNullException()
        {
            DateTime expires = DateTime.Now.AddMinutes(_random.Next(1, 60));

            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ToCookie(null, expires);
        }

        [TestMethod]
        public void ToCookie_WhenCalled_AssertResponseWasCalledOnHttpContext()
        {
            Mock<HttpContext> httpContextMock = CreateHttpContextMock();
            DateTime expires = DateTime.Now.AddMinutes(_random.Next(1, 60));

            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ToCookie(httpContextMock.Object, expires);

            httpContextMock.Verify(m => m.Response, Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalled_AssertCookiesWasCalledOnHttpResponse()
        {
            Mock<HttpResponse> httpResponseMock = CreateHttpResponseMock();
            HttpContext httpContext = CreateHttpContext(httpResponseMock.Object);
            DateTime expires = DateTime.Now.AddMinutes(_random.Next(1, 60));
            
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut();
            
            sut.ToCookie(httpContext, expires);
            
            httpResponseMock.Verify(m => m.Cookies, Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalled_AssertAppendWasCalledOnResponseCookies()
        {
            Mock<IResponseCookies> responseCookiesMock = CreateResponseCookiesMock();
            HttpContext httpContext = CreateHttpContext(responseCookies: responseCookiesMock.Object);
            DateTime expires = DateTime.Now.AddMinutes(_random.Next(1, 60));
            
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut();
            
            sut.ToCookie(httpContext, expires);
            
            responseCookiesMock.Verify(m => m.Append(
                    It.Is<string>(value => string.Compare(value, OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.CookieName, StringComparison.Ordinal) == 0),
                    It.Is<string>(value => string.Compare(value, sut.ToBase64(), StringComparison.Ordinal) == 0),
                    It.Is<CookieOptions>(value => value.Expires == new DateTimeOffset(expires))), 
                Times.Once);
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

        private HttpContext CreateHttpContext(HttpResponse httpResponse = null, IResponseCookies responseCookies = null)
        {
            return CreateHttpContextMock(httpResponse, responseCookies).Object;
        }

        private Mock<HttpContext> CreateHttpContextMock(HttpResponse httpResponse = null, IResponseCookies responseCookies = null)
        {
            Mock<HttpContext> httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(m => m.Response)
                .Returns(httpResponse ?? CreateHttpResponse(responseCookies));
            return httpContextMock;
        }

        private HttpResponse CreateHttpResponse(IResponseCookies responseCookies = null)
        {
            return CreateHttpResponseMock(responseCookies).Object;
        }

        private Mock<HttpResponse> CreateHttpResponseMock(IResponseCookies responseCookies = null)
        {
            Mock<HttpResponse> httpResponseMock = new Mock<HttpResponse>();
            httpResponseMock.Setup(m => m.Cookies)
                .Returns(responseCookies ?? CreateResponseCookies());
            return httpResponseMock;
        }

        private IResponseCookies CreateResponseCookies()
        {
            return CreateResponseCookiesMock().Object;
        }

        private Mock<IResponseCookies> CreateResponseCookiesMock()
        {
            Mock<IResponseCookies> responseCookies = new Mock<IResponseCookies>();
            return responseCookies;
        }
    }
}