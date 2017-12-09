using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
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

        private OSDevGrp.MyDashboard.Web.Controllers.HomeController CreateSut(string redditClientId = null, string redditClientSecret = null, IRedditAccessToken redditAccessToken = null)
        {
            _dataProviderFactoryMock.Setup(m => m.GetRedditAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Uri>()))
                .Returns(Task.Run<IRedditAccessToken>(() => redditAccessToken ?? new Mock<IRedditAccessToken>().Object));

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
                _configurationMock.Object,
                _httpContextAccessorMock.Object);
        }
 
        private DashboardSettingsViewModel CreateDashboardSettingsViewModel(int? numberOfNews = null, bool? useReddit = null)
        {
            return new DashboardSettingsViewModel
            {
                NumberOfNews = numberOfNews ?? _random.Next(25, 50),
                UseReddit = useReddit ?? _random.Next(100) > 50
            };
        }
    }
}