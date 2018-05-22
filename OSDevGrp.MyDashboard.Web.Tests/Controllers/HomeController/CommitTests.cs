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
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Controllers.HomeController
{
    [TestClass]
    public class CommitTests
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
        [ExpectedArgumentNullException("dashboardSettingsViewModel")]
        public void Commit_WhenDashboardSettingsViewModelIsNull_ThrowsArgumentNullExcpetion()
        {
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(null);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithRedditAccessToken_AssertHttpContextWasNotCalledOnHttpContextAccessor()
        {
            const bool useReddit = true;
            string redditAccessToken = CreateReddditAccessTokenAsBase64();
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _httpContextAccessorMock.Verify(m => m.HttpContext, Times.Never);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithRedditAccessToken_AssertAuthenticationRedditClientIdWasNotCalledOnConfiguration()
        {
            const bool useReddit = true;
            string redditAccessToken = CreateReddditAccessTokenAsBase64();
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _configurationMock.Verify(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)], Times.Never);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithRedditAccessToken_AssertAcquireRedditAuthorizationTokenAsyncWasNotCalledOnDataProviderFactory()
        {
            const bool useReddit = true;
            string redditAccessToken = CreateReddditAccessTokenAsBase64();
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _dataProviderFactoryMock.Verify(m => m.AcquireRedditAuthorizationTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Uri>()),
                Times.Never);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithRedditAccessToken_AssertBuildAsyncWasCalledOnDashboardFactory()
        {
            int numberOfNews = _random.Next(25, 50);
            const bool useReddit = true;
            bool allowNsfwContent = _random.Next(100) > 50;
            bool? includeNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            bool? onlyNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            string accessToken = Guid.NewGuid().ToString("D");
            string tokenType = Guid.NewGuid().ToString("D");
            int expiresIn = _random.Next(60, 300);
            string scope = Guid.NewGuid().ToString("D");
            string refreshToken = Guid.NewGuid().ToString("D");
            DateTime received = DateTime.UtcNow;
            string redditAccessToken = CreateReddditAccessTokenAsBase64(accessToken, tokenType, expiresIn, scope, refreshToken, received);
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(numberOfNews, useReddit, allowNsfwContent, includeNsfwContent, onlyNsfwContent, redditAccessToken);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(dashboardSettings =>
                    dashboardSettings != null &&
                    dashboardSettings.NumberOfNews == numberOfNews &&
                    dashboardSettings.UseReddit == true &&
                    dashboardSettings.RedditAccessToken != null &&
                    string.Compare(dashboardSettings.RedditAccessToken.AccessToken, accessToken, StringComparison.Ordinal) == 0 &&
                    string.Compare(dashboardSettings.RedditAccessToken.TokenType, tokenType, StringComparison.Ordinal) == 0 &&
                    string.Compare(dashboardSettings.RedditAccessToken.Expires.ToString(), received.ToLocalTime().AddSeconds(expiresIn).ToString(), StringComparison.Ordinal) == 0 &&
                    string.Compare(dashboardSettings.RedditAccessToken.Scope, scope, StringComparison.Ordinal) == 0 &&
                    string.Compare(dashboardSettings.RedditAccessToken.RefreshToken, refreshToken, StringComparison.Ordinal) == 0 &&
                    dashboardSettings.IncludeNsfwContent == (includeNsfwContent != null && includeNsfwContent.Value) &&
                    dashboardSettings.OnlyNsfwContent == (onlyNsfwContent != null && onlyNsfwContent.Value))),
                Times.Once);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithRedditAccessToken_AssertBuildAsyncWasCalledOnDashboardViewModelBuilder()
        {
            const bool useReddit = true;
            string redditAccessToken = CreateReddditAccessTokenAsBase64();
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            IDashboard dashboard = CreateDashboard();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboard: dashboard);

            sut.Commit(dashboardSettingsViewModel);

            _dashboardViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithRedditAccessToken_AssertHandleAsyncWasNotCalledOnExceptionHandlerMock()
        {
            const bool useReddit = true;
            string redditAccessToken = CreateReddditAccessTokenAsBase64();
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void Called_WhenCalledWithUseOfRedditAndWithRedditAccessToken_ReturnsViewWithDashboardViewModel()
        {
            const bool useReddit = true;
            string redditAccessToken = CreateReddditAccessTokenAsBase64();
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardViewModel: dashboardViewModel);

            IActionResult result = sut.Commit(dashboardSettingsViewModel);
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
        public void Commit_WhenCalledWithUseOfRedditAndWithoutRedditAccessToken_AssertHttpContextWasCalledOnHttpContextAccessor()
        {
            const bool useReddit = true;
            const string redditAccessToken = null;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _httpContextAccessorMock.Verify(m => m.HttpContext, Times.Once);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithoutRedditAccessToken_AssertAuthenticationRedditClientIdWasCalledOnConfiguration()
        {
            const bool useReddit = true;
            const string redditAccessToken = null;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _configurationMock.Verify(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)], Times.Once);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithoutRedditAccessToken_AssertAcquireRedditAuthorizationTokenAsyncWasCalledOnDataProviderFactory()
        {
            const bool useReddit = true;
            bool? includeNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            bool? onlyNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            const string redditAccessToken = null;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent, redditAccessToken: redditAccessToken);

            string redditClientId = Guid.NewGuid().ToString("D");
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(redditClientId: redditClientId);

            sut.Commit(dashboardSettingsViewModel);

            _dataProviderFactoryMock.Verify(m => m.AcquireRedditAuthorizationTokenAsync(
                    It.Is<string>(value => string.Compare(value, redditClientId, StringComparison.Ordinal) == 0),
                    It.Is<string>(value => string.Compare(value, dashboardSettingsViewModel.ToBase64(), StringComparison.Ordinal) == 0),
                    It.Is<Uri>(value => value == new Uri("http://localhost:5000/Home/RedditCallback"))),
                Times.Once);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithoutRedditAccessToken_AssertBuildAsyncWasNotCalledOnDashboardFactory()
        {
            const bool useReddit = true;
            const string redditAccessToken = null;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.IsAny<IDashboardSettings>()), Times.Never);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithoutRedditAccessToken_AssertBuildAsyncWasNotCalledOnDashboardViewModelBuilder()
        {
            const bool useReddit = true;
            const string redditAccessToken = null;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _dashboardViewModelBuilderMock.Verify(m => m.BuildAsync(It.IsAny<IDashboard>()), Times.Never);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithoutRedditAccessToken_AssertHandleAsyncWasNotCalledOnExceptionHandlerMock()
        {
            const bool useReddit = true;
            const string redditAccessToken = null;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithoutRedditAccessToken_ReturnsRedirectResultToAcquireRedditAccessTokenUrl()
        {
            const bool useReddit = true;
            const string redditAccessToken = null;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            Uri acquireRedditAuthorizationTokenUrl = new Uri($"https://reddit.com/{Guid.NewGuid().ToString("D")}");
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(acquireRedditAuthorizationTokenUrl: acquireRedditAuthorizationTokenUrl);

            IActionResult result = sut.Commit(dashboardSettingsViewModel);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));

            RedirectResult redirectResult = (RedirectResult) result;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(acquireRedditAuthorizationTokenUrl.AbsoluteUri, redirectResult.Url);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithoutRedditAccessTokenAndAggregateExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            const bool useReddit = true;
            const string redditAccessToken = null;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            AggregateException aggregateException = new AggregateException();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(exception: aggregateException);

            sut.Commit(dashboardSettingsViewModel);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<AggregateException>(value => value == aggregateException)), Times.Once);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithoutRedditAccessTokenAndAggregateExceptionOccurs_AssertBuildAsyncWasCalledOnDashboardFactory()
        {
            int numberOfNews = _random.Next(25, 50);
            const bool useReddit = true;
            bool allowNsfwContent = _random.Next(100) > 50;
            bool? includeNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            bool? onlyNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            const string redditAccessToken = null;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(numberOfNews, useReddit, allowNsfwContent, includeNsfwContent, onlyNsfwContent, redditAccessToken);

            AggregateException aggregateException = new AggregateException();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(exception: aggregateException);

            sut.Commit(dashboardSettingsViewModel);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(dashboardSettings =>
                    dashboardSettings != null &&
                    dashboardSettings.NumberOfNews == numberOfNews &&
                    dashboardSettings.UseReddit == false &&
                    dashboardSettings.RedditAccessToken == null &&
                    dashboardSettings.IncludeNsfwContent == (includeNsfwContent != null && includeNsfwContent.Value) &&
                    dashboardSettings.OnlyNsfwContent == (onlyNsfwContent != null && onlyNsfwContent.Value))),
                Times.Once);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithoutRedditAccessTokenAndAggregateExceptionOccurs_AssertBuildAsyncWasCalledOnDashboardViewModelBuilder()
        {
            const bool useReddit = true;
            const string redditAccessToken = null;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            IDashboard dashboard = CreateDashboard();
            AggregateException aggregateException = new AggregateException();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboard: dashboard, exception: aggregateException);

            sut.Commit(dashboardSettingsViewModel);

            _dashboardViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public void Called_WhenCalledWithUseOfRedditAndWithoutRedditAccessTokenAndAggregateExceptionOccurs_ReturnsViewWithDashboardViewModel()
        {
            const bool useReddit = true;
            const string redditAccessToken = null;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            AggregateException aggregateException = new AggregateException();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardViewModel: dashboardViewModel, exception: aggregateException);

            IActionResult result = sut.Commit(dashboardSettingsViewModel);
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
        public void Commit_WhenCalledWithUseOfRedditAndWithoutRedditAccessTokenAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            const bool useReddit = true;
            const string redditAccessToken = null;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            Exception exception = new Exception();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(exception: exception);

            sut.Commit(dashboardSettingsViewModel);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(value => value == exception)), Times.Once);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithoutRedditAccessTokenAndExceptionOccurs_AssertBuildAsyncWasCalledOnDashboardFactory()
        {
            int numberOfNews = _random.Next(25, 50);
            const bool useReddit = true;
            bool allowNsfwContent = _random.Next(100) > 50;
            bool? includeNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            bool? onlyNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            const string redditAccessToken = null;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(numberOfNews, useReddit, allowNsfwContent, includeNsfwContent, onlyNsfwContent, redditAccessToken);


            Exception exception = new Exception();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(exception: exception);

            sut.Commit(dashboardSettingsViewModel);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(dashboardSettings =>
                    dashboardSettings != null &&
                    dashboardSettings.NumberOfNews == numberOfNews &&
                    dashboardSettings.UseReddit == false &&
                    dashboardSettings.RedditAccessToken == null &&
                    dashboardSettings.IncludeNsfwContent == (includeNsfwContent != null && includeNsfwContent.Value) &&
                    dashboardSettings.OnlyNsfwContent == (onlyNsfwContent != null && onlyNsfwContent.Value))),
                Times.Once);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfRedditAndWithoutRedditAccessTokenAndExceptionOccurs_AssertBuildAsyncWasCalledOnDashboardViewModelBuilder()
        {
            const bool useReddit = true;
            const string redditAccessToken = null;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            IDashboard dashboard = CreateDashboard();
            Exception exception = new Exception();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboard: dashboard, exception: exception);

            sut.Commit(dashboardSettingsViewModel);

            _dashboardViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public void Called_WhenCalledWithUseOfRedditAndWithoutRedditAccessTokenAndExceptionOccurs_ReturnsViewWithDashboardViewModel()
        {
            const bool useReddit = true;
            const string redditAccessToken = null;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit, redditAccessToken: redditAccessToken);

            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            Exception exception = new Exception();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardViewModel: dashboardViewModel, exception: exception);

            IActionResult result = sut.Commit(dashboardSettingsViewModel);
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
        public void Commit_WhenCalledWithoutUseOfReddit_AssertHttpContextWasNotCalledOnHttpContextAccessor()
        {
            const bool useReddit = false;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _httpContextAccessorMock.Verify(m => m.HttpContext, Times.Never);
        }

        [TestMethod]
        public void Commit_WhenCalledWithoutUseOfReddit_AssertAuthenticationRedditClientIdWasNotCalledOnConfiguration()
        {
            const bool useReddit = false;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _configurationMock.Verify(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)], Times.Never);
        }

        [TestMethod]
        public void Commit_WhenCalledWithoutUseOfReddit_AssertAcquireRedditAuthorizationTokenAsyncWasNotCalledOnDataProviderFactory()
        {
            const bool useReddit = false;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _dataProviderFactoryMock.Verify(m => m.AcquireRedditAuthorizationTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Uri>()),
                Times.Never);
        }

        [TestMethod]
        public void Commit_WhenCalledWithoutUseOfReddit_AssertBuildAsyncWasCalledOnDashboardFactory()
        {
            int numberOfNews = _random.Next(25, 50);
            const bool useReddit = false;
            bool allowNsfwContent = _random.Next(100) > 50;
            bool? includeNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            bool? onlyNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(numberOfNews, useReddit, allowNsfwContent, includeNsfwContent, onlyNsfwContent);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(dashboardSettings =>
                    dashboardSettings != null &&
                    dashboardSettings.NumberOfNews == numberOfNews &&
                    dashboardSettings.UseReddit == false &&
                    dashboardSettings.RedditAccessToken == null &&
                    dashboardSettings.IncludeNsfwContent == (includeNsfwContent != null && includeNsfwContent.Value) &&
                    dashboardSettings.OnlyNsfwContent == (onlyNsfwContent != null && onlyNsfwContent.Value))),
                Times.Once);
        }

        [TestMethod]
        public void Commit_WhenCalledWithoutUseOfReddit_AssertBuildAsyncWasCalledOnDashboardViewModelBuilder()
        {
            const bool useReddit = false;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            IDashboard dashboard = CreateDashboard();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboard: dashboard);

            sut.Commit(dashboardSettingsViewModel);

            _dashboardViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public void Commit_WhenCalledWithoutUseOfReddit_AssertHandleAsyncWasNotCalledOnExceptionHandlerMock()
        {
            const bool useReddit = false;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void Called_WhenCalledWithoutUseOfReddit_ReturnsViewWithDashboardViewModel()
        {
            const bool useReddit = false;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardViewModel: dashboardViewModel);

            IActionResult result = sut.Commit(dashboardSettingsViewModel);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult) result;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewName);
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreEqual(dashboardViewModel, viewResult.Model);
        }

        private OSDevGrp.MyDashboard.Web.Controllers.HomeController CreateSut(IDashboard dashboard = null, DashboardViewModel dashboardViewModel = null, Uri acquireRedditAuthorizationTokenUrl = null, string redditClientId = null, Exception exception = null)
        {
            _dashboardFactoryMock.Setup(m => m.BuildAsync(It.IsAny<IDashboardSettings>()))
                .Returns(Task.Run<IDashboard>(() => dashboard ?? CreateDashboard()));

            _dashboardViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<IDashboard>()))
                .Returns(Task.Run<DashboardViewModel>(() => dashboardViewModel ?? new DashboardViewModel()));

            if (exception != null)
            {
                _dataProviderFactoryMock.Setup(m => m.AcquireRedditAuthorizationTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Uri>()))
                    .Throws(exception);
            }
            else
            {
                _dataProviderFactoryMock.Setup(m => m.AcquireRedditAuthorizationTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Uri>()))
                    .Returns(Task.Run<Uri>(() => acquireRedditAuthorizationTokenUrl ?? new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}")));
            }

            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<AggregateException>()))
                .Returns(Task.Run(() => { }));
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.Run(() => { }));
            
            _configurationMock.Setup(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)])
                .Returns(redditClientId ?? Guid.NewGuid().ToString("D"));

            HttpContext httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5000);
            _httpContextAccessorMock.Setup(m => m.HttpContext)
                .Returns(httpContext);
            
            return new OSDevGrp.MyDashboard.Web.Controllers.HomeController(
                _dashboardFactoryMock.Object,
                _dashboardViewModelBuilderMock.Object,
                _dataProviderFactoryMock.Object,
                _exceptionHandlerMock.Object,
                _configurationMock.Object,
                _httpContextAccessorMock.Object);
        }

        private DashboardSettingsViewModel CreateDashboardSettingsViewModel(int? numberOfNews = null, bool? useReddit = null, bool? allowNsfwContent = null, bool? includeNsfwContent = null, bool? onlyNsfwContent = null, string redditAccessToken = null)
        {
            return new DashboardSettingsViewModel
            {
                NumberOfNews = numberOfNews ?? _random.Next(25, 50),
                UseReddit = useReddit ?? _random.Next(100) > 50,
                AllowNsfwContent = allowNsfwContent ?? _random.Next(100) > 50,
                IncludeNsfwContent = includeNsfwContent,
                OnlyNsfwContent = onlyNsfwContent,
                RedditAccessToken = redditAccessToken
            };
        }
 
        private IDashboard CreateDashboard()
        {
            Mock<IDashboard> dashboardMock = new Mock<IDashboard>();
            return dashboardMock.Object;
        }

        private string CreateReddditAccessTokenAsBase64(string accessToken = null, string tokenType = null, int? expiresIn = null, string scope = null, string refreshToken = null, DateTime? received = null)
        {
            return CreateReddditAccessToken(accessToken, tokenType, expiresIn, scope, refreshToken, received).ToBase64();
        }
 
        private IRedditAccessToken CreateReddditAccessToken(string accessToken = null, string tokenType = null, int? expiresIn = null, string scope = null, string refreshToken = null, DateTime? received = null)
        {
            return new MyRedditAccessToken(
                accessToken ?? Guid.NewGuid().ToString("D"),
                tokenType ?? Guid.NewGuid().ToString("D"),
                expiresIn ?? _random.Next(60, 300),
                scope ?? Guid.NewGuid().ToString("D"),
                refreshToken ?? Guid.NewGuid().ToString("D"),
                received ?? DateTime.UtcNow);
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