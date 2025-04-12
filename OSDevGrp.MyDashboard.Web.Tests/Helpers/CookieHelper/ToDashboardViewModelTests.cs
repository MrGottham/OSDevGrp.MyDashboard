using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;
using System;
using System.Text.RegularExpressions;

namespace OSDevGrp.MyDashboard.Web.Tests.Helpers.CookieHelper
{
    [TestClass]
    public class ToDashboardViewModelTests : TestBase
    {
        #region Private variables

        private Mock<IContentHelper> _contentHelperMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IMemoryCache> _memoryCacheMock;
        private readonly Regex _memoryCacheKeyRegex = new Regex("^" + DashboardViewModel.CookieName + @".(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$", RegexOptions.Compiled);

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _contentHelperMock = new Mock<IContentHelper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _memoryCacheMock = new Mock<IMemoryCache>();
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenCalled_AssertHttpContextWasCalledOnHttpContextAccessor()
        {
            ICookieHelper sut = CreateSut();

            sut.ToDashboardViewModel();

            _httpContextAccessorMock.Verify(m => m.HttpContext, Times.Once);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenHttpContextWasReturned_AssertRequestWasCalledOnReturnedHttpContext()
        {
            Mock<HttpContext> httpContextMock = BuildHttpContextMock();
            ICookieHelper sut = CreateSut(httpContext: httpContextMock.Object);

            sut.ToDashboardViewModel();

            httpContextMock.Verify(m => m.Request, Times.Once);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenHttpRequestWasReturned_AssertCookiesWasCalledOnReturnedHttpRequest()
        {
            Mock<HttpRequest> httpRequestMock = BuildHttpRequestMock();
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequestMock.Object);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            httpRequestMock.Verify(m => m.Cookies, Times.Once);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenHttpRequestWasReturnedButRequestCookieCollectionWasNotReturned_AssertToValueWasNotCalledOnContentHelperWithCookieValue()
        {
            HttpRequest httpRequest = BuildHttpRequest(hasRequestCookieCollection: false);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            _contentHelperMock.Verify(m => m.ToValue(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenHttpRequestWasReturnedButRequestCookieCollectionWasNotReturned_AssertTryGetValueWasNotCalledOnMemoryCache()
        {
            HttpRequest httpRequest = BuildHttpRequest(hasRequestCookieCollection: false);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            object outValue;
            _memoryCacheMock.Verify(m => m.TryGetValue(It.IsAny<object>(), out outValue), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenHttpRequestWasReturnedButRequestCookieCollectionWasNotReturned_ReturnsNull()
        {
            HttpRequest httpRequest = BuildHttpRequest(hasRequestCookieCollection: false);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardViewModel result = sut.ToDashboardViewModel();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenRequestCookieCollectionWasReturned_AssertContainsKeyWasCalledOnReturnedRequestCookieCollectionWithCookieNameForDashboardViewModel()
        {
            Mock<IRequestCookieCollection> requestCookieCollectionMock = BuildRequestCookieCollectionMock(dashboardCookieValue: BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollectionMock.Object);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            requestCookieCollectionMock.Verify(m => m.ContainsKey(It.Is<string>(value => string.CompareOrdinal(value, DashboardViewModel.CookieName) == 0)), Times.Once);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenRequestCookieCollectionWasReturnedButDoNotContainCookieNameForDashboardViewModel_AssertItemWasNotCalledOnReturnedRequestCookieCollectionWithCookieNameForDashboardViewModel()
        {
            Mock<IRequestCookieCollection> requestCookieCollectionMock = BuildRequestCookieCollectionMock();
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollectionMock.Object);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            requestCookieCollectionMock.Verify(m => m[It.Is<string>(value => string.CompareOrdinal(value, DashboardViewModel.CookieName) == 0)], Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenRequestCookieCollectionWasReturnedButDoNotContainCookieNameForDashboardViewModel_AssertToValueWasNotCalledOnContentHelperWithCookieValue()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection();
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            _contentHelperMock.Verify(m => m.ToValue(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenRequestCookieCollectionWasReturnedButDoNotContainCookieNameForDashboardViewModel_AssertTryGetValueWasNotCalledOnMemoryCache()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection();
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            object outValue;
            _memoryCacheMock.Verify(m => m.TryGetValue(It.IsAny<object>(), out outValue), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenRequestCookieCollectionWasReturnedButDoNotContainCookieNameForDashboardViewModel_ReturnsNull()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection();
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardViewModel result = sut.ToDashboardViewModel();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenRequestCookieCollectionContainingCookieNameForCookieNameForDashboardViewModelWasReturned_AssertItemWasCalledOnReturnedRequestCookieCollectionWithCookieNameForDashboardViewModel()
        {
            Mock<IRequestCookieCollection> requestCookieCollectionMock = BuildRequestCookieCollectionMock(dashboardCookieValue: BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollectionMock.Object);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            requestCookieCollectionMock.Verify(m => m[It.Is<string>(value => string.CompareOrdinal(value, DashboardViewModel.CookieName) == 0)], Times.Once);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenCookieValueWasReturned_AssertToValueWasCalledOnContentHelperWithCookieValue()
        {
            string cookieValue = BuildBase64String();
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection(dashboardCookieValue: cookieValue);
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            _contentHelperMock.Verify(m => m.ToValue(It.Is<string>(value => string.CompareOrdinal(value, cookieValue) == 0)), Times.Once);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenCookieValueCannotBeConvertedToMemoryCacheKey_AssertTryGetValueWasNotCalledOnMemoryCache()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection(dashboardCookieValue: BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(hasMemoryCacheKeyInCookie: false, httpContext: httpContext);

            sut.ToDashboardViewModel();

            object outValue;
            _memoryCacheMock.Verify(m => m.TryGetValue(It.IsAny<object>(), out outValue), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenCookieValueCannotBeConvertedToMemoryCacheKey_ReturnsNull()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection(dashboardCookieValue: BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(hasMemoryCacheKeyInCookie: false, httpContext: httpContext);

            DashboardViewModel result = sut.ToDashboardViewModel();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenCookieValueCanBeConvertedToMemoryCacheKey_AssertTryGetValueWasCalledOnMemoryCache()
        {
            string memoryCacheKeyInCookie = $"{DashboardViewModel.CookieName}.{Guid.NewGuid():D}";
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection(dashboardCookieValue: BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(memoryCacheKeyInCookie: memoryCacheKeyInCookie, httpContext: httpContext);

            sut.ToDashboardViewModel();

            object outValue;
            _memoryCacheMock.Verify(m => m.TryGetValue(It.Is<object>(value => value != null && _memoryCacheKeyRegex.IsMatch((string) value)), out outValue), Times.Once);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenCookieValueCanBeConvertedToNonExistingMemoryCacheKey_ReturnsNull()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection(dashboardCookieValue: BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext, hasDashboardViewModelInMemoryCache: false);

            DashboardViewModel result = sut.ToDashboardViewModel();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenMemoryCacheValueCanBeConvertedToDashboardViewModel_ReturnsDashboardViewModel()
        {
            DashboardViewModel dashboardViewModelInMemoryCache = BuildDashboardViewModel();
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection(dashboardCookieValue: BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(dashboardViewModelInMemoryCache: dashboardViewModelInMemoryCache, httpContext: httpContext);

            DashboardViewModel result = sut.ToDashboardViewModel();

            Assert.AreEqual(result, dashboardViewModelInMemoryCache);
        }

        private ICookieHelper CreateSut(bool hasDashboardViewModelInMemoryCache = true, DashboardViewModel dashboardViewModelInMemoryCache = null, bool hasMemoryCacheKeyInCookie = true, string memoryCacheKeyInCookie = null, HttpContext httpContext = null)
        {
            _contentHelperMock.Setup(m => m.ToValue(It.IsAny<string>()))
                .Returns(hasMemoryCacheKeyInCookie ? memoryCacheKeyInCookie ?? Guid.NewGuid().ToString("D") : null);

            _httpContextAccessorMock.Setup(m => m.HttpContext)
                .Returns(httpContext ?? BuildHttpContext());

            object outValue = hasDashboardViewModelInMemoryCache ? dashboardViewModelInMemoryCache ?? BuildDashboardViewModel() : null;
            _memoryCacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out outValue))
                .Returns(outValue != null);

            return new Web.Helpers.CookieHelper(_contentHelperMock.Object, _httpContextAccessorMock.Object, _memoryCacheMock.Object);
        }
    }
}