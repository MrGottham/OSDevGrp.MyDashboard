using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Helpers.CookieHelper
{
    [TestClass]
    public class ToDashboardSettingsViewModelTests : TestBase
    {
        #region Private variables

        private Mock<IContentHelper> _contentHelperMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IMemoryCache> _memoryCacheMock;
        private Random _random;

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
        public void ToDashboardSettingsViewModel_WhenCalled_AssertHttpContextWasCalledOnHttpContextAccessor()
        {
            ICookieHelper sut = CreateSut();

            sut.ToDashboardSettingsViewModel();

            _httpContextAccessorMock.Verify(m => m.HttpContext, Times.Once);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenHttpContextWasNotReturned_AssertToDashboardSettingsViewModelWasNotCalledOnContentHelper()
        {
            ICookieHelper sut = CreateSut(hasHttpContext: false);

            sut.ToDashboardSettingsViewModel();

            _contentHelperMock.Verify(m => m.ToDashboardSettingsViewModel(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenHttpContextWasNotReturned_ReturnsNull()
        {
            ICookieHelper sut = CreateSut(hasHttpContext: false);

            DashboardSettingsViewModel result = sut.ToDashboardSettingsViewModel();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenHttpContextWasReturned_AssertRequestWasCalledOnReturnedHttpContext()
        {
            Mock<HttpContext> httpContextMock = BuildHttpContextMock();
            ICookieHelper sut = CreateSut(httpContext: httpContextMock.Object);

            sut.ToDashboardSettingsViewModel();

            httpContextMock.Verify(m => m.Request, Times.Once);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenHttpContextWasReturnedButHttpRequestWasNotReturned_AssertToDashboardSettingsViewModelWasNotCalledOnContentHelper()
        {
            HttpContext httpContext = BuildHttpContext(false);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardSettingsViewModel();

            _contentHelperMock.Verify(m => m.ToDashboardSettingsViewModel(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenHttpContextWasReturnedButHttpRequestWasNotReturned_ReturnsNull()
        {
            HttpContext httpContext = BuildHttpContext(false);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardSettingsViewModel result = sut.ToDashboardSettingsViewModel();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenHttpRequestWasReturned_AssertCookiesWasCalledOnReturnedHttpRequest()
        {
            Mock<HttpRequest> httpRequestMock = BuildHttpRequestMock();
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequestMock.Object);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardSettingsViewModel();

            httpRequestMock.Verify(m => m.Cookies, Times.Once);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenHttpRequestWasReturnedButRequestCookieCollectionWasNotReturned_AssertToDashboardSettingsViewModelWasNotCalledOnContentHelper()
        {
            HttpRequest httpRequest = BuildHttpRequest(hasRequestCookieCollection: false);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardSettingsViewModel();

            _contentHelperMock.Verify(m => m.ToDashboardSettingsViewModel(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenHttpRequestWasReturnedButRequestCookieCollectionWasNotReturned_ReturnsNull()
        {
            HttpRequest httpRequest = BuildHttpRequest(hasRequestCookieCollection: false);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardSettingsViewModel result = sut.ToDashboardSettingsViewModel();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenRequestCookieCollectiontWasReturned_AssertContainsKeyWasCalledOnReturnedRequestCookieCollectionWithCookieNameForDashboardSettingsViewModel()
        {
            Mock<IRequestCookieCollection> requestCookieCollectionMock = BuildRequestCookieCollectionMock(BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollectionMock.Object);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardSettingsViewModel();

            requestCookieCollectionMock.Verify(m => m.ContainsKey(It.Is<string>(value => string.CompareOrdinal(value, DashboardSettingsViewModel.CookieName) == 0)), Times.Once);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenRequestCookieCollectiontWasReturnedButDoNotContainCookieNameForDashboardSettingsViewModel_AssertItemWasNotCalledOnReturnedRequestCookieCollectionWithCookieNameForDashboardSettingsViewModel()
        {
            Mock<IRequestCookieCollection> requestCookieCollectionMock = BuildRequestCookieCollectionMock();
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollectionMock.Object);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardSettingsViewModel();

            requestCookieCollectionMock.Verify(m => m[It.Is<string>(value => string.CompareOrdinal(value, DashboardSettingsViewModel.CookieName) == 0)], Times.Never);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenRequestCookieCollectiontWasReturnedButDoNotContainCookieNameForDashboardSettingsViewModel_AssertToDashboardSettingsViewModelWasNotCalledOnContentHelper()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection();
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardSettingsViewModel();

            _contentHelperMock.Verify(m => m.ToDashboardSettingsViewModel(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenRequestCookieCollectiontWasReturnedButDoNotContainCookieNameForDashboardSettingsViewModel_ReturnsNull()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection();
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            DashboardSettingsViewModel result = sut.ToDashboardSettingsViewModel();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenRequestCookieCollectiontContainingCookieNameForDashboardSettingsViewModelWasReturned_AssertItemWasCalledOnReturnedRequestCookieCollectionWithCookieNameForDashboardSettingsViewModel()
        {
            Mock<IRequestCookieCollection> requestCookieCollectionMock = BuildRequestCookieCollectionMock(BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollectionMock.Object);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardSettingsViewModel();

            requestCookieCollectionMock.Verify(m => m[It.Is<string>(value => string.CompareOrdinal(value, DashboardSettingsViewModel.CookieName) == 0)], Times.Once);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenCookieValueWasReturned_AssertToDashboardSettingsViewModelWasCalledOnContentHelper()
        {
            string cookieValue = BuildBase64String();
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection(cookieValue);
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(httpContext: httpContext);

            sut.ToDashboardSettingsViewModel();

            _contentHelperMock.Verify(m => m.ToDashboardSettingsViewModel(It.Is<string>(value => string.CompareOrdinal(value, cookieValue) == 0)), Times.Once);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenCookieValueCanBeConvertedToDashboardSettingsViewModel_ReturnsDashboardSettingsViewModel()
        {
            DashboardSettingsViewModel dashboardSettingsViewModelInCookie = BuildDashboardSettingsViewModel(_random);
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection(BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(dashboardSettingsViewModelInCookie: dashboardSettingsViewModelInCookie, httpContext: httpContext);

            DashboardSettingsViewModel result = sut.ToDashboardSettingsViewModel();

            Assert.AreEqual(result, dashboardSettingsViewModelInCookie);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenCookieValueCannotBeConvertedToDashboardSettingsViewModel_ReturnsNull()
        {
            IRequestCookieCollection requestCookieCollection = BuildRequestCookieCollection(BuildBase64String());
            HttpRequest httpRequest = BuildHttpRequest(requestCookieCollection: requestCookieCollection);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            ICookieHelper sut = CreateSut(false, httpContext: httpContext);

            DashboardSettingsViewModel result = sut.ToDashboardSettingsViewModel();

            Assert.IsNull(result);
        }

        private ICookieHelper CreateSut(bool hasDashboardSettingsViewModelInCookie = true, DashboardSettingsViewModel dashboardSettingsViewModelInCookie = null, bool hasHttpContext = true, HttpContext httpContext = null)
        {
            _contentHelperMock.Setup(m => m.ToDashboardSettingsViewModel(It.IsAny<string>()))
                .Returns(hasDashboardSettingsViewModelInCookie ? dashboardSettingsViewModelInCookie ?? BuildDashboardSettingsViewModel(_random) : null);
            
            _httpContextAccessorMock.Setup(m => m.HttpContext)
                .Returns(hasHttpContext ? httpContext ?? BuildHttpContext() : null);

            return new Web.Helpers.CookieHelper(_contentHelperMock.Object, _httpContextAccessorMock.Object, _memoryCacheMock.Object);
        }
    }
}