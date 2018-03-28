using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Controllers.HomeController
{
    [TestClass]
    public class IndexTests
    {
        #region Private variables

        private Mock<IDashboardFactory> _dashboardFactoryMock;
        private Mock<IViewModelBuilder<DashboardViewModel, IDashboard>> _dashboardViewModelBuilderMock;
        private Mock<IDataProviderFactory> _dataProviderFactoryMock;
        private Mock<IExceptionHandler> _exceptionHandlerMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _dashboardFactoryMock = new Mock<IDashboardFactory>();
            _dashboardViewModelBuilderMock = new Mock<IViewModelBuilder<DashboardViewModel, IDashboard>>();
            _dataProviderFactoryMock = new Mock<IDataProviderFactory>();
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
            _configurationMock = new Mock<IConfiguration>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        public void Index_WhenCalled_AssertHttpContextWasCalledOnHttpContextAccessor()
        {
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Index();
            
            _httpContextAccessorMock.Verify(m => m.HttpContext, Times.Once);
        }

        [TestMethod]
        public void Index_WhenCalled_AssertContainsKeyWasCalledOnRequestCookieCollectionWithCookieNameForDashboardSettingsViewModel()
        {
            Mock<IRequestCookieCollection> requestCookieCollectionMock = CreateRequestCookieCollectionMock();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(requestCookieCollection: requestCookieCollectionMock.Object);

            sut.Index();
            
            requestCookieCollectionMock.Verify(m => m.ContainsKey(It.Is<string>(value => string.Compare(DashboardSettingsViewModel.CookieName, value, StringComparison.Ordinal) == 0)), Times.Once);
        }

        [TestMethod]
        public void Index_WhenCalledAndCookieForDashboardSettingsViewModelDoesNotExist_AssertBuildAsyncWasCalledOnDashboardFactory()
        {
            string dashboardSettingsViewModelAsBase64 = null; 
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardSettingsViewModelAsBase64: dashboardSettingsViewModelAsBase64);

            sut.Index();

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(dashboardSettings =>
                    dashboardSettings != null &&
                    dashboardSettings.NumberOfNews == 100 &&
                    dashboardSettings.UseReddit == false &&
                    dashboardSettings.RedditAccessToken == null &&
                    dashboardSettings.IncludeNsfwContent == false &&
                    dashboardSettings.OnlyNsfwContent == false)),
                Times.Once);
        }

        [TestMethod]
        public void Index_WhenCalledAndCookieForDashboardSettingsViewModelDoesExistWithoutRedditAccessToken_AssertBuildAsyncWasCalledOnDashboardFactory()
        {
            int numberOfNews = _random.Next(25, 50);
            bool useReddit = _random.Next(100) > 50;
            const string redditAccessTokenAsBase64 = null;
            string dashboardSettingsViewModelAsBase64 = BuildDashboardSettingsViewModelAsBase64(numberOfNews: numberOfNews, useReddit: useReddit, redditAccessToken: redditAccessTokenAsBase64);
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardSettingsViewModelAsBase64: dashboardSettingsViewModelAsBase64);

            sut.Index();

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(dashboardSettings =>
                    dashboardSettings != null &&
                    dashboardSettings.NumberOfNews == numberOfNews &&
                    dashboardSettings.UseReddit == useReddit &&
                    dashboardSettings.RedditAccessToken == null &&
                    dashboardSettings.IncludeNsfwContent == false &&
                    dashboardSettings.OnlyNsfwContent == false)),
                Times.Once);
        }

        [TestMethod]
        public void Index_WhenCalledAndCookieForDashboardSettingsViewModelDoesExistWithRedditAccessToken_AssertBuildAsyncWasCalledOnDashboardFactory()
        {
            int numberOfNews = _random.Next(25, 50);
            bool useReddit = _random.Next(100) > 50;
            string accessToken = Guid.NewGuid().ToString("D");
            string tokenType = Guid.NewGuid().ToString("D");
            int expiresIn = _random.Next(30, 60);
            string scope = Guid.NewGuid().ToString("D");
            string refreshToken = Guid.NewGuid().ToString("D");
            DateTime received = DateTime.UtcNow;
            string redditAccessTokenAsBase64 = BuildRedditAccessTokenAsBase64(
                accessToken,
                tokenType,
                expiresIn,
                scope,
                refreshToken,
                received);
            string dashboardSettingsViewModelAsBase64 = BuildDashboardSettingsViewModelAsBase64(numberOfNews: numberOfNews, useReddit: useReddit, redditAccessToken: redditAccessTokenAsBase64);
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardSettingsViewModelAsBase64: dashboardSettingsViewModelAsBase64);

            sut.Index();

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(dashboardSettings =>
                    dashboardSettings != null &&
                    dashboardSettings.NumberOfNews == numberOfNews &&
                    dashboardSettings.UseReddit == useReddit &&
                    dashboardSettings.RedditAccessToken != null &&
                    string.Compare(dashboardSettings.RedditAccessToken.AccessToken, accessToken, StringComparison.Ordinal) == 0 &&
                    string.Compare(dashboardSettings.RedditAccessToken.TokenType, tokenType, StringComparison.Ordinal) == 0 &&
                    dashboardSettings.RedditAccessToken.Expires.ToString() == received.ToLocalTime().AddSeconds(expiresIn).ToString() &&
                    string.Compare(dashboardSettings.RedditAccessToken.Scope, scope, StringComparison.Ordinal) == 0 &&
                    string.Compare(dashboardSettings.RedditAccessToken.RefreshToken, refreshToken, StringComparison.Ordinal) == 0 &&
                    dashboardSettings.IncludeNsfwContent == false &&
                    dashboardSettings.OnlyNsfwContent == false)),
                Times.Once);
        }

        [TestMethod]
        public void Index_WhenCalled_AssertBuildAsyncWasCalledOnDashboardViewModelBuilder()
        {
            IDashboard dashboard = BuildDashboard();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboard: dashboard);

            sut.Index();

            _dashboardViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public void Index_WhenCalled_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Index();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void Index_WhenCalled_ReturnsViewWithDashboardViewModel()
        {
            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardViewModel: dashboardViewModel);

            IActionResult result = sut.Index();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult) result;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewName);
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreEqual(dashboardViewModel, viewResult.Model);
        }

        [TestMethod]
        public void Index_WhenCalledAndAggregateExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            AggregateException aggregateException = new AggregateException();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(exception: aggregateException);

            try
            {
                sut.Index();
            }
            catch (Exception)
            {
            }

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<AggregateException>(value => value == aggregateException)), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException), "My AggregateException")]
        public void Index_WhenCalledAndAggregateExceptionOccurs_ThrowsAggregateException()
        {
            AggregateException aggregateException = new AggregateException("My AggregateException");
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(exception: aggregateException);

            sut.Index();
        }

        [TestMethod]
        public void Index_WhenCalledAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            Exception exception = new Exception();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(exception: exception);

            try
            {
                sut.Index();
            }
            catch (Exception)
            {
            }

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(value => value == exception)), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "My Exception")]
        public void Index_WhenCalledAndExceptionOccurs_ThrowsException()
        {
            Exception exception = new Exception("My Exception");
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(exception: exception);

            sut.Index();
        }

        private OSDevGrp.MyDashboard.Web.Controllers.HomeController CreateSut(IDashboard dashboard = null, DashboardViewModel dashboardViewModel = null, Exception exception = null, IRequestCookieCollection requestCookieCollection = null, string dashboardSettingsViewModelAsBase64 = null)
        {
            if (exception != null)
            {
                _dashboardFactoryMock.Setup(m => m.BuildAsync(It.IsAny<IDashboardSettings>()))
                    .Throws(exception);
            }
            else
            {
                _dashboardFactoryMock.Setup(m => m.BuildAsync(It.IsAny<IDashboardSettings>()))
                    .Returns(Task.Run<IDashboard>(() => dashboard ?? BuildDashboard()));
            }

            _dashboardViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<IDashboard>()))
                .Returns(Task.Run<DashboardViewModel>(() => dashboardViewModel ?? new DashboardViewModel()));

            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<AggregateException>()))
                .Returns(Task.Run(() => { }));
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.Run(() => { }));

            _httpContextAccessorMock.Setup(m => m.HttpContext)
                .Returns(CreateHttpContext(requestCookieCollection, dashboardSettingsViewModelAsBase64));
            
            return new OSDevGrp.MyDashboard.Web.Controllers.HomeController(
                _dashboardFactoryMock.Object,
                _dashboardViewModelBuilderMock.Object,
                _dataProviderFactoryMock.Object,
                _exceptionHandlerMock.Object,
                _configurationMock.Object,
                _httpContextAccessorMock.Object);
        }

        private string BuildDashboardSettingsViewModelAsBase64(int? numberOfNews = null, bool? useReddit = null, string redditAccessToken = null)
        {
            return new DashboardSettingsViewModel
            {
                NumberOfNews = numberOfNews ?? _random.Next(25, 50),
                UseReddit = useReddit ?? _random.Next(100) > 50,
                RedditAccessToken = redditAccessToken
            }.ToBase64();
        }

        private string BuildRedditAccessTokenAsBase64(string accessToken = null, string tokenType = null, int? expiresIn = null, string scope = null, string refreshToken = null, DateTime? received = null)
        {
            return new MyRedditAccessToken(
                accessToken ?? Guid.NewGuid().ToString("D"),
                tokenType ?? Guid.NewGuid().ToString("D"),
                expiresIn ?? _random.Next(30, 60),
                scope ?? Guid.NewGuid().ToString("D"),
                refreshToken ?? Guid.NewGuid().ToString("D"),
                received ?? DateTime.UtcNow).ToBase64();
        }

        private IDashboard BuildDashboard()
        {
            Mock<IDashboard> dashboardMock = new Mock<IDashboard>();
            return dashboardMock.Object;
        }

        private HttpContext CreateHttpContext(IRequestCookieCollection requestCookieCollection = null, string dashboardSettingsViewModelAsBase64 = null)
        {
            return CreateHttpContextMock(requestCookieCollection, dashboardSettingsViewModelAsBase64).Object;
        }

        private Mock<HttpContext> CreateHttpContextMock(IRequestCookieCollection requestCookieCollection = null, string dashboardSettingsViewModelAsBase64 = null)
        {
            Mock<HttpContext> httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(m => m.Request)
                .Returns(CreateHttpRequest(requestCookieCollection, dashboardSettingsViewModelAsBase64));
            return httpContextMock; 
        } 

        private HttpRequest CreateHttpRequest(IRequestCookieCollection requestCookieCollection = null, string dashboardSettingsViewModelAsBase64 = null)
        {
            return CreateHttpRequestMock(requestCookieCollection, dashboardSettingsViewModelAsBase64).Object;
        }

        private Mock<HttpRequest> CreateHttpRequestMock(IRequestCookieCollection requestCookieCollection = null, string dashboardSettingsViewModelAsBase64 = null)
        {
            Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(m => m.Cookies)
                .Returns(requestCookieCollection ?? CreateRequestCookieCollection(dashboardSettingsViewModelAsBase64));
            return httpRequestMock;
        }

        private IRequestCookieCollection CreateRequestCookieCollection(string dashboardSettingsViewModelAsBase64 = null)
        {
            return CreateRequestCookieCollectionMock(dashboardSettingsViewModelAsBase64).Object;
        }

        private Mock<IRequestCookieCollection> CreateRequestCookieCollectionMock(string dashboardSettingsViewModelAsBase64 = null)
        {
            Mock<IRequestCookieCollection> requestCookieCollectionMock = new Mock<IRequestCookieCollection>();
            requestCookieCollectionMock.Setup(m => m.ContainsKey(It.Is<string>(value => string.Compare(DashboardSettingsViewModel.CookieName, value, StringComparison.Ordinal) == 0)))
                .Returns(string.IsNullOrWhiteSpace(dashboardSettingsViewModelAsBase64) == false);
            requestCookieCollectionMock.Setup(m => m[It.Is<string>(value => string.Compare(DashboardSettingsViewModel.CookieName, value, StringComparison.Ordinal) == 0)])
                .Returns(dashboardSettingsViewModelAsBase64);
            return requestCookieCollectionMock; 
        }

        [DataContract]
        private class MyRedditAccessToken : RedditAccessToken
        {
            #region Constructor

            public MyRedditAccessToken(string accessToken, string tokenType, int expiresIn, string scope, string refreshToken, DateTime received)
            {
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    throw new ArgumentNullException(nameof(accessToken));
                }
                if (string.IsNullOrWhiteSpace(tokenType))
                {
                    throw new ArgumentNullException(nameof(tokenType));
                }
                if (string.IsNullOrWhiteSpace(scope))
                {
                    throw new ArgumentNullException(nameof(scope));
                }
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    throw new ArgumentNullException(nameof(refreshToken));
                }

                AccessToken = accessToken;
                TokenType = tokenType;
                ExpiresIn = expiresIn;
                Scope = scope;
                RefreshToken = refreshToken;
                Received = received;
            }

            #endregion
        }
    }
}