using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Helpers.CookieHelper
{
    [TestClass]
    public class ToCookieTests : TestBase
    {
        #region Private variables

        private Mock<IContentHelper> _contentHelperMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IMemoryCache> _memoryCacheMock;
        private Mock<ICacheEntry> _cacheEntryMock;
        private Random _random;
        private readonly Regex _memoryCacheKeyRegex = new Regex("^" + DashboardViewModel.CookieName + @".(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$", RegexOptions.Compiled);

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _contentHelperMock = new Mock<IContentHelper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _memoryCacheMock = new Mock<IMemoryCache>();
            _cacheEntryMock = new Mock<ICacheEntry>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullException("dashboardSettingsViewModel")]
        public void ToCookie_WhenDashboardSettingsViewModelIsNull_ThrowsArgumentNullException()
        {
            ICookieHelper sut = CreateSut();

            sut.ToCookie((DashboardSettingsViewModel) null);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardSettingsViewModel_AssertHttpContextWasCalledOnHttpContextAccessor()
        {
            ICookieHelper sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random);
            sut.ToCookie(dashboardSettingsViewModel);

            _httpContextAccessorMock.Verify(m => m.HttpContext, Times.Exactly(2));
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardSettingsViewModelAndHttpContextWasNotReturned_AssertToBase64StringWasNotCalledOnContentHelperWithDashboardSettingsViewModel()
        {
            ICookieHelper sut = CreateSut(hasHttpContext: false);

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random);
            sut.ToCookie(dashboardSettingsViewModel);

            _contentHelperMock.Verify(m => m.ToBase64String(It.IsAny<DashboardSettingsViewModel>()), Times.Never);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardSettingsViewModelAndHttpContextWasReturned_AssertRequestWasCalledOnReturnedHttpContext()
        {
            Mock<HttpContext> httpContextMock = BuildHttpContextMock();
            ICookieHelper sut = CreateSut(httpContext: httpContextMock.Object);

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random);
            sut.ToCookie(dashboardSettingsViewModel);

            httpContextMock.Verify(m => m.Request, Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardSettingsViewModelAndHttpRequestWasReturned_AssertIsHttpsWasCalledOnReturnedHttpRequest()
        {
            Mock<HttpRequest> httpRequestMock = BuildHttpRequestMock();
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequestMock.Object);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random);
            sut.ToCookie(dashboardSettingsViewModel);

            httpRequestMock.Verify(m => m.IsHttps, Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardSettingsViewModelAndHttpRequestWasReturned_AssertSchemeWasCalledOnReturnedHttpRequest()
        {
            Mock<HttpRequest> httpRequestMock = BuildHttpRequestMock();
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequestMock.Object);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random);
            sut.ToCookie(dashboardSettingsViewModel);

            httpRequestMock.Verify(m => m.Scheme, Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardSettingsViewModelAndHttpContextWasReturned_AssertResponseWasCalledOnReturnedHttpContext()
        {
            Mock<HttpContext> httpContextMock = BuildHttpContextMock();
            ICookieHelper sut = CreateSut(httpContext: httpContextMock.Object);

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random);
            sut.ToCookie(dashboardSettingsViewModel);

            httpContextMock.Verify(m => m.Response, Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardSettingsViewModelAndHttpContextWasReturnedButHttpResponseWasNotReturned_AssertToBase64StringWasNotCalledOnContentHelperWithDashboardSettingsViewModel()
        {
            HttpContext httpContext = BuildHttpContext(hasHttpResponse: false);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random);
            sut.ToCookie(dashboardSettingsViewModel);

            _contentHelperMock.Verify(m => m.ToBase64String(It.IsAny<DashboardSettingsViewModel>()), Times.Never);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardSettingsViewModelAndHttpResponseWasReturned_AssertCookiesWasCalledOnReturnedHttpResponse()
        {
            Mock<HttpResponse> httpResponseMock = BuildHttpResponseMock();
            HttpContext httpContext = BuildHttpContext(httpResponse: httpResponseMock.Object);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random);
            sut.ToCookie(dashboardSettingsViewModel);

            httpResponseMock.Verify(m => m.Cookies, Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardSettingsViewModelAndHttpResponseWasReturnedButResponseCookiesWasNotReturned_AssertToBase64StringWasNotCalledOnContentHelperWithDashboardSettingsViewModel()
        {
            HttpResponse httpResponse = BuildHttpResponse(false);
            HttpContext httpContext = BuildHttpContext(httpResponse: httpResponse);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random);
            sut.ToCookie(dashboardSettingsViewModel);

            _contentHelperMock.Verify(m => m.ToBase64String(It.IsAny<DashboardSettingsViewModel>()), Times.Never);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardSettingsViewModelAndResponseCookiesWasReturned_AssertToBase64StringWasCalledOnContentHelperWithDashboardSettingsViewModel()
        {
            ICookieHelper sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random);
            sut.ToCookie(dashboardSettingsViewModel);

            _contentHelperMock.Verify(m => m.ToBase64String(It.Is<DashboardSettingsViewModel>(value => value == dashboardSettingsViewModel)), Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardSettingsViewModelAndResponseCookiesWasReturnedButCookieValueToStoreWasNotReturned_AssertAppendWasNotCalledOnReturnedResponseCookies()
        {
            Mock<IResponseCookies> responseCookiesMock = BuildResponseCookiesMock();
            HttpResponse httpResponse = BuildHttpResponse(responseCookies: responseCookiesMock.Object);
            HttpContext httpContext = BuildHttpContext(httpResponse: httpResponse);
            ICookieHelper sut = CreateSut(false, httpContext: httpContext);

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random);
            sut.ToCookie(dashboardSettingsViewModel);

            responseCookiesMock.Verify(m => m.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Never);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardSettingsViewModelWithoutRedditAccessTokenAndResponseCookiesWasReturnedButCookieValueToStoreWasReturned_AssertAppendWasCalledOnReturnedResponseCookies()
        {
            bool isHttps = _random.Next(100) > 50;
            HttpRequest httpRequest = BuildHttpRequest(isHttps);
            string cookieValueToStore = Guid.NewGuid().ToString("D");
            Mock<IResponseCookies> responseCookiesMock = BuildResponseCookiesMock();
            HttpResponse httpResponse = BuildHttpResponse(responseCookies: responseCookiesMock.Object);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest, httpResponse: httpResponse);
            ICookieHelper sut = CreateSut(cookieValueToStore: cookieValueToStore, httpContext: httpContext);

            DateTime expires = DateTime.Now.AddHours(8);
            
            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random);
            sut.ToCookie(dashboardSettingsViewModel);

            responseCookiesMock.Verify(m => m.Append(
                    It.Is<string>(value => string.CompareOrdinal(value, DashboardSettingsViewModel.CookieName) == 0), 
                    It.Is<string>(value => string.CompareOrdinal(value, BuildBase64String(cookieValueToStore)) == 0), 
                    It.Is<CookieOptions>(value => value != null && value.Expires >= expires.AddSeconds(-5) && value.Expires <= expires.AddSeconds(5) && value.Secure == isHttps && value.SameSite == SameSiteMode.None)), 
                Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardSettingsViewModelWithRedditAccessTokenAndResponseCookiesWasReturnedButCookieValueToStoreWasReturned_AssertAppendWasCalledOnReturnedResponseCookies()
        {
            bool isHttps = _random.Next(100) > 50;
            HttpRequest httpRequest = BuildHttpRequest(isHttps);
            string cookieValueToStore = Guid.NewGuid().ToString("D");
            Mock<IResponseCookies> responseCookiesMock = BuildResponseCookiesMock();
            HttpResponse httpResponse = BuildHttpResponse(responseCookies: responseCookiesMock.Object);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest, httpResponse: httpResponse);
            ICookieHelper sut = CreateSut(cookieValueToStore: cookieValueToStore, httpContext: httpContext);

            int expiresIn = _random.Next(5, 60) * 60;
            DateTime received = DateTime.UtcNow;
            string redditAccessTokenAsBase64 = BuildRedditAccessTokenAsBase64(_random, expiresIn, received);
            DateTime expires = received.AddSeconds(expiresIn).ToLocalTime();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, redditAccessTokenAsBase64);
            sut.ToCookie(dashboardSettingsViewModel);

            responseCookiesMock.Verify(m => m.Append(
                    It.Is<string>(value => string.CompareOrdinal(value, DashboardSettingsViewModel.CookieName) == 0), 
                    It.Is<string>(value => string.CompareOrdinal(value, BuildBase64String(cookieValueToStore)) == 0), 
                    It.Is<CookieOptions>(value => value != null && value.Expires >= expires.AddSeconds(-5) && value.Expires <= expires.AddSeconds(5) && value.Secure == isHttps && value.SameSite == SameSiteMode.None)), 
                Times.Once);
        }

        [TestMethod]
        [ExpectedArgumentNullException("dashboardViewModel")]
        public void ToCookie_WhenDashboardViewModelIsNull_ThrowsArgumentNullException()
        {
            ICookieHelper sut = CreateSut();

            sut.ToCookie((DashboardViewModel) null);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModel_AssertHttpContextWasCalledOnHttpContextAccessor()
        {
            ICookieHelper sut = CreateSut();

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _httpContextAccessorMock.Verify(m => m.HttpContext, Times.Exactly(2));
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndHttpContextWasNotReturned_AssertToByteArrayWasNotCalledOnContentHelperWithDashboardViewModel()
        {
            ICookieHelper sut = CreateSut(hasHttpContext: false);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _contentHelperMock.Verify(m => m.ToByteArray(It.IsAny<DashboardViewModel>()), Times.Never);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndHttpContextWasNotReturned_AssertToBase64StringWasNotCalledOnContentHelperWithValueString()
        {
            ICookieHelper sut = CreateSut(hasHttpContext: false);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _contentHelperMock.Verify(m => m.ToBase64String(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndHttpContextWasNotReturned_AssertCreateEntryWasNotCalledOnMemoryCache()
        {
            ICookieHelper sut = CreateSut(hasHttpContext: false);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _memoryCacheMock.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Never);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndHttpContextWasReturned_AssertRequestWasCalledOnReturnedHttpContext()
        {
            Mock<HttpContext> httpContextMock = BuildHttpContextMock();
            ICookieHelper sut = CreateSut(httpContext: httpContextMock.Object);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            httpContextMock.Verify(m => m.Request, Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndHttpRequestWasReturned_AssertIsHttpsWasCalledOnReturnedHttpRequest()
        {
            Mock<HttpRequest> httpRequestMock = BuildHttpRequestMock();
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequestMock.Object);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            httpRequestMock.Verify(m => m.IsHttps, Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndHttpRequestWasReturned_AssertSchemeWasCalledOnReturnedHttpRequest()
        {
            Mock<HttpRequest> httpRequestMock = BuildHttpRequestMock();
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequestMock.Object);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            httpRequestMock.Verify(m => m.Scheme, Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndHttpContextWasReturned_AssertResponseWasCalledOnReturnedHttpContext()
        {
            Mock<HttpContext> httpContextMock = BuildHttpContextMock();
            ICookieHelper sut = CreateSut(httpContext: httpContextMock.Object);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            httpContextMock.Verify(m => m.Response, Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndHttpContextWasReturnedButHttpResponseWasNotReturned_AssertToByteArrayWasNotCalledOnContentHelperWithDashboardViewModel()
        {
            HttpContext httpContext = BuildHttpContext(hasHttpResponse: false);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _contentHelperMock.Verify(m => m.ToByteArray(It.IsAny<DashboardViewModel>()), Times.Never);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndHttpContextWasReturnedButHttpResponseWasNotReturned_AssertToBase64StringWasNotCalledOnContentHelperWithValueString()
        {
            HttpContext httpContext = BuildHttpContext(hasHttpResponse: false);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _contentHelperMock.Verify(m => m.ToBase64String(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndHttpContextWasReturnedButHttpResponseWasNotReturned_AssertCreateEntryWasNotCalledOnMemoryCache()
        {
            HttpContext httpContext = BuildHttpContext(hasHttpResponse: false);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _memoryCacheMock.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Never);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndHttpResponseWasReturned_AssertCookiesWasCalledOnReturnedHttpResponse()
        {
            Mock<HttpResponse> httpResponseMock = BuildHttpResponseMock();
            HttpContext httpContext = BuildHttpContext(httpResponse: httpResponseMock.Object);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            httpResponseMock.Verify(m => m.Cookies, Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndHttpResponseWasReturnedButResponseCookiesWasNotReturned_AssertToByteArrayWasNotCalledOnContentHelperWithDashboardViewModel()
        {
            HttpResponse httpResponse = BuildHttpResponse(false);
            HttpContext httpContext = BuildHttpContext(httpResponse: httpResponse);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _contentHelperMock.Verify(m => m.ToByteArray(It.IsAny<DashboardViewModel>()), Times.Never);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndHttpResponseWasReturnedButResponseCookiesWasNotReturned_AssertToBase64StringWasNotCalledOnContentHelperWithValueString()
        {
            HttpResponse httpResponse = BuildHttpResponse(false);
            HttpContext httpContext = BuildHttpContext(httpResponse: httpResponse);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _contentHelperMock.Verify(m => m.ToBase64String(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndHttpResponseWasReturnedButResponseCookiesWasNotReturned_AssertCreateEntryWasNotCalledOnMemoryCache()
        {
            HttpResponse httpResponse = BuildHttpResponse(false);
            HttpContext httpContext = BuildHttpContext(httpResponse: httpResponse);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _memoryCacheMock.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Never);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndResponseCookiesWasReturned_AssertToByteArrayWasCalledOnContentHelperWithDashboardViewModel()
        {
            ICookieHelper sut = CreateSut();

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _contentHelperMock.Verify(m => m.ToByteArray(It.Is<DashboardViewModel>(value => value == dashboardViewModel)), Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndDashboardViewModelWasNotConvertedToByteArray_AssertToBase64StringWasNotCalledOnContentHelperWithValueString()
        {
            ICookieHelper sut = CreateSut(false);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _contentHelperMock.Verify(m => m.ToBase64String(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndDashboardViewModelWasNotConvertedToByteArray_AssertCreateEntryWasNotCalledOnMemoryCache()
        {
            ICookieHelper sut = CreateSut(false);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _memoryCacheMock.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Never);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndDashboardViewModelWasNotConvertedToByteArray_AssertAppendWasNotCalledOnReturnedResponseCookies()
        {
            Mock<IResponseCookies> responseCookiesMock = BuildResponseCookiesMock();
            HttpResponse httpResponse = BuildHttpResponse(responseCookies: responseCookiesMock.Object);
            HttpContext httpContext = BuildHttpContext(httpResponse: httpResponse);
            ICookieHelper sut = CreateSut(false, httpContext: httpContext);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            responseCookiesMock.Verify(m => m.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Never);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndDashboardViewModelWasConvertedToByteArray_AssertToBase64StringWasCalledOnContentHelperWithValueString()
        {
            ICookieHelper sut = CreateSut();

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _contentHelperMock.Verify(m => m.ToBase64String(It.Is<string>(value => string.IsNullOrWhiteSpace(value) == false && _memoryCacheKeyRegex.IsMatch(value))), Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndDashboardViewModelWasConvertedToByteArray_AssertCreateEntryWasCalledOnMemoryCache()
        {
            ICookieHelper sut = CreateSut();

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _memoryCacheMock.Verify(m => m.CreateEntry(It.Is<object>(value => value != null && _memoryCacheKeyRegex.IsMatch((string) value))), Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndCacheEntryWasCreated_AssertValueSetterWasCalledOnCreatedCacheEntry()
        {
            string cookieValueToStore = Guid.NewGuid().ToString("D");
            ICookieHelper sut = CreateSut(cookieValueToStore: cookieValueToStore);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _cacheEntryMock.VerifySet(m => m.Value = It.Is<byte[]>(value => value != null && string.CompareOrdinal(Convert.ToBase64String(value), BuildBase64String(cookieValueToStore)) == 0), Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndCacheEntryWasCreated_AssertAbsoluteExpirationSetterWasCalledOnCreatedCacheEntry()
        {
            ICookieHelper sut = CreateSut();

            DateTime expires = DateTime.Now.AddSeconds(30);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            _cacheEntryMock.VerifySet(m => m.AbsoluteExpiration = It.Is<DateTimeOffset>(value => value >= expires.AddSeconds(-5) && value <= expires.AddSeconds(5)), Times.Once);
        }

        [TestMethod]
        public void ToCookie_WhenCalledWithDashboardViewModelAndDashboardViewModelWasConvertedToByteArray_AssertAppendWasCalledOnReturnedResponseCookies()
        {
            bool isHttps = _random.Next(100) > 50;
            HttpRequest httpRequest = BuildHttpRequest(isHttps);
            string cookieValueToStore = $"{DashboardViewModel.CookieName}.{Guid.NewGuid().ToString("D")}";
            Mock<IResponseCookies> responseCookiesMock = BuildResponseCookiesMock();
            HttpResponse httpResponse = BuildHttpResponse(responseCookies: responseCookiesMock.Object);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest, httpResponse: httpResponse);
            ICookieHelper sut = CreateSut(cookieValueToStore: cookieValueToStore, httpContext: httpContext);

            DateTime expires = DateTime.Now.AddSeconds(30);

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToCookie(dashboardViewModel);

            responseCookiesMock.Verify(m => m.Append(
                    It.Is<string>(value => string.CompareOrdinal(value, DashboardViewModel.CookieName) == 0), 
                    It.Is<string>(value => string.CompareOrdinal(value, BuildBase64String(cookieValueToStore)) == 0), 
                    It.Is<CookieOptions>(value => value != null && value.Expires >= expires.AddSeconds(-5) && value.Expires <= expires.AddSeconds(5) && value.Secure == isHttps && value.SameSite == SameSiteMode.None)), 
                Times.Once);
        }

        private ICookieHelper CreateSut(bool hasCookieValueToStore = true, string cookieValueToStore = null, bool hasHttpContext = true, HttpContext httpContext = null)
        {
            _contentHelperMock.Setup(m => m.ToBase64String(It.IsAny<DashboardSettingsViewModel>()))
                .Returns(hasCookieValueToStore ? BuildBase64String(cookieValueToStore) : null);
            _contentHelperMock.Setup(m => m.ToByteArray(It.IsAny<DashboardViewModel>()))
                .Returns(hasCookieValueToStore ? Convert.FromBase64String(BuildBase64String(cookieValueToStore)) : null);
            _contentHelperMock.Setup(m => m.ToBase64String(It.IsAny<string>()))
                .Returns(hasCookieValueToStore ? BuildBase64String(cookieValueToStore) : null);

            _httpContextAccessorMock.Setup(m => m.HttpContext)
                .Returns(hasHttpContext ? httpContext ?? BuildHttpContext() : null);

            _memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(_cacheEntryMock.Object);

            return new Web.Helpers.CookieHelper(_contentHelperMock.Object, _httpContextAccessorMock.Object, _memoryCacheMock.Object);
        }
    }
}