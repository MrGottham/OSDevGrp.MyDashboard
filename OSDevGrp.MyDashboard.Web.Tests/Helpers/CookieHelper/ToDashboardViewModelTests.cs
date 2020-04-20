using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Helpers.CookieHelper
{
    [TestClass]
    public class ToDashboardViewModelTests : TestBase
    {
        #region Private variables

        private Mock<IContentHelper> _contentHelperMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IMemoryCache> _memoryCacheMock;
        private Random _random;
        private readonly Regex _memoryCacheKeyRegex = new Regex("^" + DashboardViewModel.CookieName + @".(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$", RegexOptions.Compiled);

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _contentHelperMock = new Mock<IContentHelper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _memoryCacheMock = new Mock<IMemoryCache>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenCalled_AssertHttpContextWasCalledOnHttpContextAccessor()
        {
            ICookieHelper sut = CreateSut();

            sut.ToDashboardViewModel();

            _httpContextAccessorMock.Verify(m => m.HttpContext, Times.Once);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenCalledAndHttpContextWasNotReturned_AssertToDashboardViewModelWasNotCalledOnContentHelperWithByteArray()
        {
            ICookieHelper sut = CreateSut(hasHttpContext: false);

            sut.ToDashboardViewModel();

            _contentHelperMock.Verify(m => m.ToDashboardViewModel(It.IsAny<byte[]>()), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenCalledAndHttpContextWasNotReturned_AssertToValueWasNotCalledOnContentHelperWithCookieValue()
        {
            ICookieHelper sut = CreateSut(hasHttpContext: false);

            sut.ToDashboardViewModel();

            _contentHelperMock.Verify(m => m.ToValue(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenCalledAndHttpContextWasNotReturned_AssertTryGetValueWasNotCalledOnMemoryCache()
        {
            ICookieHelper sut = CreateSut(hasHttpContext: false);

            sut.ToDashboardViewModel();

            object outValue;
            _memoryCacheMock.Verify(m => m.TryGetValue(It.IsAny<object>(), out outValue), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenCalledAndHttpContextWasNotReturned_ReturnsNull()
        {
            ICookieHelper sut = CreateSut(hasHttpContext: false);

            DashboardViewModel result = sut.ToDashboardViewModel();

            Assert.IsNull(result);
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
        public void ToDashboardViewModel_WhenHttpContextWasReturnedButHttpRequestWasNotReturned_AssertToDashboardViewModelWasNotCalledOnContentHelperWithByteArray()
        {
            HttpContext httpContext = BuildHttpContext(false);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            _contentHelperMock.Verify(m => m.ToDashboardViewModel(It.IsAny<byte[]>()), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenHttpContextWasReturnedButHttpRequestWasNotReturned_AssertToValueWasNotCalledOnContentHelperWithCookieValue()
        {
            HttpContext httpContext = BuildHttpContext(false);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            _contentHelperMock.Verify(m => m.ToValue(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenHttpContextWasReturnedButHttpRequestWasNotReturned_AssertTryGetValueWasNotCalledOnMemoryCache()
        {
            HttpContext httpContext = BuildHttpContext(false);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            object outValue;
            _memoryCacheMock.Verify(m => m.TryGetValue(It.IsAny<object>(), out outValue), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenHttpContextWasReturnedButHttpRequestWasNotReturned_ReturnsNull()
        {
            HttpContext httpContext = BuildHttpContext(false);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardViewModel result = sut.ToDashboardViewModel();

            Assert.IsNull(result);
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
        public void ToDashboardViewModel_WhenHttpRequestWasReturnedButRequestCookieCollectionWasNotReturned_AssertToDashboardViewModelWasNotCalledOnContentHelperWithByteArray()
        {
            HttpRequest httpRequest = BuildHttpRequest(false);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            _contentHelperMock.Verify(m => m.ToDashboardViewModel(It.IsAny<byte[]>()), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenHttpRequestWasReturnedButRequestCookieCollectionWasNotReturned_AssertToValueWasNotCalledOnContentHelperWithCookieValue()
        {
            HttpRequest httpRequest = BuildHttpRequest(false);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            _contentHelperMock.Verify(m => m.ToValue(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenHttpRequestWasReturnedButRequestCookieCollectionWasNotReturned_AssertTryGetValueWasNotCalledOnMemoryCache()
        {
            HttpRequest httpRequest = BuildHttpRequest(false);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            object outValue;
            _memoryCacheMock.Verify(m => m.TryGetValue(It.IsAny<object>(), out outValue), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenHttpRequestWasReturnedButRequestCookieCollectionWasNotReturned_ReturnsNull()
        {
            HttpRequest httpRequest = BuildHttpRequest(false);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardViewModel result = sut.ToDashboardViewModel();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenRequestCookieCollectiontWasReturned_AssertContainsKeyWasCalledOnReturnedRequestCookieCollectionWithCookieNameForDashboardViewModel()
        {
            Mock<IRequestCookieCollection> requestCookieCollectionMock = BuildRequestCookieCollectionMock(dashboardCookieValue: BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollectionMock.Object);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            requestCookieCollectionMock.Verify(m => m.ContainsKey(It.Is<string>(value => string.CompareOrdinal(value, DashboardViewModel.CookieName) == 0)), Times.Once);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenRequestCookieCollectiontWasReturnedButDoNotContainCookieNameForDashboardViewModel_AssertItemWasNotCalledOnReturnedRequestCookieCollectionWithCookieNameForDashboardViewModel()
        {
            Mock<IRequestCookieCollection> requestCookieCollectionMock = BuildRequestCookieCollectionMock();
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollectionMock.Object);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            requestCookieCollectionMock.Verify(m => m[It.Is<string>(value => string.CompareOrdinal(value, DashboardViewModel.CookieName) == 0)], Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenRequestCookieCollectiontWasReturnedButDoNotContainCookieNameForDashboardViewModel_AssertToDashboardViewModelWasNotCalledOnContentHelperWithByteArray()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection();
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            _contentHelperMock.Verify(m => m.ToDashboardViewModel(It.IsAny<byte[]>()), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenRequestCookieCollectiontWasReturnedButDoNotContainCookieNameForDashboardViewModel_AssertToValueWasNotCalledOnContentHelperWithCookieValue()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection();
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardViewModel();

            _contentHelperMock.Verify(m => m.ToValue(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenRequestCookieCollectiontWasReturnedButDoNotContainCookieNameForDashboardViewModel_AssertTryGetValueWasNotCalledOnMemoryCache()
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
        public void ToDashboardViewModel_WhenRequestCookieCollectiontWasReturnedButDoNotContainCookieNameForDashboardViewModel_ReturnsNull()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection();
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardViewModel result = sut.ToDashboardViewModel();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenRequestCookieCollectiontContainingCookieNameForCookieNameForDashboardViewModelWasReturned_AssertItemWasCalledOnReturnedRequestCookieCollectionWithCookieNameForDashboardViewModel()
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
        public void ToDashboardViewModel_WhenCookieValueCannotBeConvertedToMemoryCacheKey_AssertToDashboardViewModelWasNotCalledOnContentHelperWithByteArray()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection(dashboardCookieValue: BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(hasMemoryCacheKeyInCookie: false, httpContext: httpContext);

            sut.ToDashboardViewModel();

            _contentHelperMock.Verify(m => m.ToDashboardViewModel(It.IsAny<byte[]>()), Times.Never);
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
            string memoryCacheKeyInCookie = $"{DashboardViewModel.CookieName}.{Guid.NewGuid().ToString("D")}";
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection(dashboardCookieValue: BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(memoryCacheKeyInCookie: memoryCacheKeyInCookie, httpContext: httpContext);

            sut.ToDashboardViewModel();

            object outValue;
            _memoryCacheMock.Verify(m => m.TryGetValue(It.Is<object>(value => value != null && _memoryCacheKeyRegex.IsMatch((string) value)), out outValue), Times.Once);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenCookieValueCanBeConvertedToNonExistingMemoryCacheKey_AssertToDashboardViewModelWasNotCalledOnContentHelperWithByteArray()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection(dashboardCookieValue: BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext, hasMemoryCacheValue: false);

            sut.ToDashboardViewModel();

            _contentHelperMock.Verify(m => m.ToDashboardViewModel(It.IsAny<byte[]>()), Times.Never);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenCookieValueCanBeConvertedToNonExistingMemoryCacheKey_ReturnsNull()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection(dashboardCookieValue: BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext, hasMemoryCacheValue: false);

            DashboardViewModel result = sut.ToDashboardViewModel();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenCookieValueCanBeConvertedToExistingMemoryCacheKey_AssertToDashboardViewModelWasCalledOnContentHelperWithByteArray()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection(dashboardCookieValue: BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            string memoryCacheValue = BuildBase64String();
            ICookieHelper sut = CreateSut(httpContext: httpContext, memoryCacheValue: Convert.FromBase64String(memoryCacheValue));

            sut.ToDashboardViewModel();

            _contentHelperMock.Verify(m => m.ToDashboardViewModel(It.IsAny<byte[]>()), Times.Once);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenMemoryCacheValueCanBeConvertedToDashboardViewModel_ReturnsDashboardViewModel()
        {
            DashboardViewModel dashboardViewModelInMemoryCache = BuildDashboardViewModel();
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection(dashboardCookieValue: BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(dashboardViewModelInMemoryCache: dashboardViewModelInMemoryCache, httpContext: httpContext, memoryCacheValue: Convert.FromBase64String(BuildBase64String()));

            DashboardViewModel result = sut.ToDashboardViewModel();

            Assert.AreEqual(result, dashboardViewModelInMemoryCache);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenMemoryCacheValueCannotBeConvertedToDashboardViewModel_ReturnsNull()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection(dashboardCookieValue: BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(false, httpContext: httpContext, memoryCacheValue: Convert.FromBase64String(BuildBase64String()));

            DashboardViewModel result = sut.ToDashboardViewModel();

            Assert.IsNull(result);
        }

        private ICookieHelper CreateSut(bool hasDashboardViewModelInMemoryCache = true, DashboardViewModel dashboardViewModelInMemoryCache = null, bool hasMemoryCacheKeyInCookie = true, string memoryCacheKeyInCookie = null, bool hasHttpContext = true, HttpContext httpContext = null, bool hasMemoryCacheValue = true, byte[] memoryCacheValue = null)
        {
            _contentHelperMock.Setup(m => m.ToDashboardViewModel(It.IsAny<byte[]>()))
                .Returns(hasDashboardViewModelInMemoryCache ? dashboardViewModelInMemoryCache ?? BuildDashboardViewModel() : null);
            _contentHelperMock.Setup(m => m.ToValue(It.IsAny<string>()))
                .Returns(hasMemoryCacheKeyInCookie ? memoryCacheKeyInCookie ?? Guid.NewGuid().ToString("D") : null);

            _httpContextAccessorMock.Setup(m => m.HttpContext)
                .Returns(hasHttpContext ? httpContext ?? BuildHttpContext() : null);

            object outValue;
            _memoryCacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out outValue))
                .Callback(() => outValue = memoryCacheValue)
                .Returns(hasMemoryCacheValue);

            return new Web.Helpers.CookieHelper(_contentHelperMock.Object, _httpContextAccessorMock.Object, _memoryCacheMock.Object);
        }
    }
}