using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Controllers.HomeController
{
    [TestClass]
    public class RedditCallbackTests
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
        [ExpectedArgumentNullException("code")]
        public void RedditCallback_WhenCodeIsNull_ThrowsArgumentNullException()
        {
            const string code = null;
            string state = CreateDashboardSettingsViewModel().ToBase64();

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state);
        }

        [TestMethod]
        [ExpectedArgumentNullException("code")]
        public void RedditCallback_WhenCodeIsEmpty_ThrowsArgumentNullException()
        {
            string code = string.Empty;
            string state = CreateDashboardSettingsViewModel().ToBase64();

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state);
        }

        [TestMethod]
        [ExpectedArgumentNullException("code")]
        public void RedditCallback_WhenCodeIsWhitespace_ThrowsArgumentNullException()
        {
            const string code = " ";
            string state = CreateDashboardSettingsViewModel().ToBase64();

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state);
        }

        [TestMethod]
        [ExpectedArgumentNullException("code")]
        public void RedditCallback_WhenCodeIsWhitespaces_ThrowsArgumentNullException()
        {
            const string code = "  ";
            string state = CreateDashboardSettingsViewModel().ToBase64();

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public void RedditCallback_WhenStateIsNull_ThrowsArgumentNullException()
        {
            string code = Guid.NewGuid().ToString("D");
            const string state = null;

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public void RedditCallback_WhenStateIsEmpty_ThrowsArgumentNullException()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = string.Empty;

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public void RedditCallback_WhenStateIsWhitespace_ThrowsArgumentNullException()
        {
            string code = Guid.NewGuid().ToString("D");
            const string state = " ";

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public void RedditCallback_WhenStateIsWhitespaces_ThrowsArgumentNullException()
        {
            string code = Guid.NewGuid().ToString("D");
            const string state = "  ";

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithError_AssertHttpContextWasNotCalledOnHttpContextAccessor()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();
            string error = Guid.NewGuid().ToString("D");

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state, error);

            _httpContextAccessorMock.Verify(m => m.HttpContext, Times.Never);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithError_AssertAuthenticationRedditClientIdWasNotCalledOnConfiguration()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();
            string error = Guid.NewGuid().ToString("D");

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state, error);

            _configurationMock.Verify(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)], Times.Never);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithError_AssertAuthenticationRedditClientSecretWasNotCalledOnConfiguration()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();
            string error = Guid.NewGuid().ToString("D");

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state, error);

            _configurationMock.Verify(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientSecret", value, StringComparison.Ordinal) == 0)], Times.Never);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithError_AssertGetRedditAccessTokenAsyncWasNotCalledOnDataProviderFactory()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();
            string error = Guid.NewGuid().ToString("D");

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state, error);

            _dataProviderFactoryMock.Verify(m => m.GetRedditAccessTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Uri>()),
                Times.Never);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithError_AssertHandleAsyncWasCalledOnExceptionHandlerMock()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();
            string error = Guid.NewGuid().ToString("D");

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state, error);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(value => string.Compare(value.Message, $"Unable to get the access token from Reddit: {error}", StringComparison.Ordinal) == 0)), Times.Once);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithError_AssertBuildAsyncWasCalledOnDashboardFactory()
        {
            string code = Guid.NewGuid().ToString("D");
            int numberOfNews = _random.Next(25, 50);
            const bool useReddit = true;
            bool? includeNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            bool? onlyNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            string state = CreateDashboardSettingsViewModel(numberOfNews: numberOfNews, useReddit: useReddit, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent).ToBase64();
            string error = Guid.NewGuid().ToString("D");

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state, error);

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
        public void RedditCallback_WhenCalledWithError_AssertBuildAsyncWasCalledOnDashboardViewModelBuilder()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();
            string error = Guid.NewGuid().ToString("D");

            IDashboard dashboard = CreateDashboard();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboard: dashboard);

            sut.RedditCallback(code, state, error);

            _dashboardViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithError_ReturnsViewWithDashboardViewModel()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();
            string error = Guid.NewGuid().ToString("D");

            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardViewModel: dashboardViewModel);

            IActionResult result = sut.RedditCallback(code, state, error);
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
        public void RedditCallback_WhenCalledWithoutError_AssertHttpContextWasCalledOnHttpContextAccessor()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state);

            _httpContextAccessorMock.Verify(m => m.HttpContext, Times.Once);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithoutError_AssertAuthenticationRedditClientIdWasCalledOnConfiguration()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state);

            _configurationMock.Verify(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)], Times.Once);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithoutError_AssertAuthenticationRedditClientSecretWasCalledOnConfiguration()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state);

            _configurationMock.Verify(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientSecret", value, StringComparison.Ordinal) == 0)], Times.Once);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithoutError_AssertGetRedditAccessTokenAsyncWasCalledOnDataProviderFactory()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();

            string redditClientId = Guid.NewGuid().ToString("D");
            string redditClientSecret = Guid.NewGuid().ToString("D");
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(redditClientId: redditClientId, redditClientSecret: redditClientSecret);

            sut.RedditCallback(code, state);

            _dataProviderFactoryMock.Verify(m => m.GetRedditAccessTokenAsync(
                    It.Is<string>(value => string.Compare(redditClientId, value, StringComparison.Ordinal) == 0),
                    It.Is<string>(value => string.Compare(redditClientSecret, value, StringComparison.Ordinal) == 0),
                    It.Is<string>(value => string.Compare(code, value, StringComparison.Ordinal) == 0),
                    It.Is<Uri>(value => value == new Uri("http://localhost:5000/Home/RedditCallback"))),
                Times.Once);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithoutError_AssertBuildAsyncWasCalledOnDashboardFactory()
        {
            string code = Guid.NewGuid().ToString("D");
            int numberOfNews = _random.Next(25, 50);
            const bool useReddit = true;
            bool? includeNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            bool? onlyNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            string state = CreateDashboardSettingsViewModel(numberOfNews: numberOfNews, useReddit: useReddit, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent).ToBase64();

            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(redditAccessToken: redditAccessToken);

            sut.RedditCallback(code, state);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(dashboardSettings =>
                    dashboardSettings != null &&
                    dashboardSettings.NumberOfNews == numberOfNews &&
                    dashboardSettings.UseReddit == true &&
                    dashboardSettings.RedditAccessToken == redditAccessToken &&
                    dashboardSettings.IncludeNsfwContent == (includeNsfwContent != null && includeNsfwContent.Value) &&
                    dashboardSettings.OnlyNsfwContent == (onlyNsfwContent != null && onlyNsfwContent.Value))),
                Times.Once);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithoutError_AssertBuildAsyncWasCalledOnDashboardViewModelBuilder()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();

            IDashboard dashboard = CreateDashboard();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboard: dashboard);

            sut.RedditCallback(code, state);

            _dashboardViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithoutError_AssertHandleAsyncWasNotCalledOnExceptionHandlerMock()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.RedditCallback(code, state);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithoutError_ReturnsViewWithDashboardViewModel()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();

            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardViewModel: dashboardViewModel);

            IActionResult result = sut.RedditCallback(code, state);
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
        public void RedditCallback_WhenCalledWithoutErrorAndAggregateExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();

            AggregateException aggregateException = new AggregateException();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(exception: aggregateException);

            sut.RedditCallback(code, state);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<AggregateException>(value => value == aggregateException)), Times.Once);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithoutErrorAndAggregateExceptionOccurs_AssertBuildAsyncWasCalledOnDashboardFactory()
        {
            string code = Guid.NewGuid().ToString("D");
            int numberOfNews = _random.Next(25, 50);
            const bool useReddit = true;
            bool? includeNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            bool? onlyNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            string state = CreateDashboardSettingsViewModel(numberOfNews: numberOfNews, useReddit: useReddit, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent).ToBase64();

            AggregateException aggregateException = new AggregateException();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(exception: aggregateException);

            sut.RedditCallback(code, state);

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
        public void RedditCallback_WhenCalledWithoutErrorAndAggregateExceptionOccurs_AssertBuildAsyncWasCalledOnDashboardViewModelBuilder()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();

            IDashboard dashboard = CreateDashboard();
            AggregateException aggregateException = new AggregateException();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboard: dashboard, exception: aggregateException);

            sut.RedditCallback(code, state);

            _dashboardViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithoutErrorAndAggregateExceptionOccurs_ReturnsViewWithDashboardViewModel()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();

            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            AggregateException aggregateException = new AggregateException();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardViewModel: dashboardViewModel, exception: aggregateException);

            sut.RedditCallback(code, state);

            IActionResult result = sut.RedditCallback(code, state);
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
        public void RedditCallback_WhenCalledWithoutErrorAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();

            Exception exception = new Exception();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(exception: exception);

            sut.RedditCallback(code, state);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(value => value == exception)), Times.Once);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithoutErrorAndExceptionOccurs_AssertBuildAsyncWasCalledOnDashboardFactory()
        {
            string code = Guid.NewGuid().ToString("D");
            int numberOfNews = _random.Next(25, 50);
            const bool useReddit = true;
            bool? includeNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            bool? onlyNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            string state = CreateDashboardSettingsViewModel(numberOfNews: numberOfNews, useReddit: useReddit, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent).ToBase64();

            Exception exception = new Exception();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(exception: exception);

            sut.RedditCallback(code, state);

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
        public void RedditCallback_WhenCalledWithoutErrorAndExceptionOccurs_AssertBuildAsyncWasCalledOnDashboardViewModelBuilder()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();

            IDashboard dashboard = CreateDashboard();
            Exception exception = new Exception();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboard: dashboard, exception: exception);

            sut.RedditCallback(code, state);

            _dashboardViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public void RedditCallback_WhenCalledWithoutErrorAndExceptionOccurs_ReturnsViewWithDashboardViewModel()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = CreateDashboardSettingsViewModel().ToBase64();

            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            Exception exception = new Exception();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardViewModel: dashboardViewModel, exception: exception);

            sut.RedditCallback(code, state);

            IActionResult result = sut.RedditCallback(code, state);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult) result;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewName);
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreEqual(dashboardViewModel, viewResult.Model);
        }

        private OSDevGrp.MyDashboard.Web.Controllers.HomeController CreateSut(IDashboard dashboard = null, DashboardViewModel dashboardViewModel = null, string redditClientId = null, string redditClientSecret = null, IRedditAccessToken redditAccessToken = null, Exception exception = null)
        {
            _dashboardFactoryMock.Setup(m => m.BuildAsync(It.IsAny<IDashboardSettings>()))
                .Returns(Task.Run<IDashboard>(() => dashboard ?? CreateDashboard()));

            _dashboardViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<IDashboard>()))
                .Returns(Task.Run<DashboardViewModel>(() => dashboardViewModel ?? new DashboardViewModel()));

            if (exception != null)
            {
                _dataProviderFactoryMock.Setup(m => m.GetRedditAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Uri>()))
                    .Throws(exception);
            }
            else
            {
                _dataProviderFactoryMock.Setup(m => m.GetRedditAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Uri>()))
                    .Returns(Task.Run<IRedditAccessToken>(() => redditAccessToken ?? CreateRedditAccessToken()));
            }

            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<AggregateException>()))
                .Returns(Task.Run(() => { }));
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.Run(() => { }));
            
            _configurationMock.Setup(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)])
                .Returns(redditClientId ?? Guid.NewGuid().ToString("D"));
            _configurationMock.Setup(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientSecret", value, StringComparison.Ordinal) == 0)])
                .Returns(redditClientSecret ?? Guid.NewGuid().ToString("D"));

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
 
        private DashboardSettingsViewModel CreateDashboardSettingsViewModel(int? numberOfNews = null, bool? useReddit = null, bool? allowNsfwContent = null, bool? includeNsfwContent = null, bool? onlyNsfwContent = null)
        {
            return new DashboardSettingsViewModel
            {
                NumberOfNews = numberOfNews ?? _random.Next(25, 50),
                UseReddit = useReddit ?? _random.Next(100) > 50,
                AllowNsfwContent = allowNsfwContent ?? _random.Next(100) > 50,
                IncludeNsfwContent = includeNsfwContent,
                OnlyNsfwContent = onlyNsfwContent
            };
        }

        private IDashboard CreateDashboard()
        {
            Mock<IDashboard> dashboardMock = new Mock<IDashboard>();
            return dashboardMock.Object;
        }

        private IRedditAccessToken CreateRedditAccessToken()
        {
            Mock<IRedditAccessToken> redditAccessTokenMock = new Mock<IRedditAccessToken>();
            return redditAccessTokenMock.Object;
        }
    }
}