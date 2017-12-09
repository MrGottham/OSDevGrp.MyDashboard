using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class CommitTests
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
            _configurationMock = new Mock<IConfiguration>();
            _dataProviderFactoryMock = new Mock<IDataProviderFactory>();
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
        public void Commit_WhenCalledWithUseOfReddit_AssertHttpContextWasCalledOnHttpContextAccessor()
        {
            const bool useReddit = true;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _httpContextAccessorMock.Verify(m => m.HttpContext, Times.Once);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfReddit_AssertAuthenticationRedditClientIdWasCalledOnConfiguration()
        {
            const bool useReddit = true;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _configurationMock.Verify(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)], Times.Once);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfReddit_AssertAcquireRedditAccessTokenAsyncWasCalledOnDataProviderFactory()
        {
            const bool useReddit = true;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            string redditClientId = Guid.NewGuid().ToString("D");
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(redditClientId: redditClientId);

            sut.Commit(dashboardSettingsViewModel);

            _dataProviderFactoryMock.Verify(m => m.AcquireRedditAccessTokenAsync(
                    It.Is<string>(value => string.Compare(value, redditClientId, StringComparison.Ordinal) == 0),
                    It.Is<string>(value => string.Compare(value, dashboardSettingsViewModel.ToBase64(), StringComparison.Ordinal) == 0),
                    It.Is<Uri>(value => value == new Uri("http://localhost:5000/Home/RedditCallback"))),
                Times.Once);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfReddit_AssertBuildAsyncWasNotCalledOnDashboardFactory()
        {
            const bool useReddit = true;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.IsAny<IDashboardSettings>()), Times.Never);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfReddit_AssertBuildAsyncWasNotCalledOnDashboardViewModelBuilder()
        {
            const bool useReddit = true;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _dashboardViewModelBuilderMock.Verify(m => m.BuildAsync(It.IsAny<IDashboard>()), Times.Never);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfReddit_ReturnsRedirectResultToAcquireRedditAccessTokenUrl()
        {
            const bool useReddit = true;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            Uri acquireRedditAccessTokenUrl = new Uri($"https://reddit.com/{Guid.NewGuid().ToString("D")}");
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(acquireRedditAccessTokenUrl: acquireRedditAccessTokenUrl);

            IActionResult result = sut.Commit(dashboardSettingsViewModel);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));

            RedirectResult redirectResult = (RedirectResult) result;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(acquireRedditAccessTokenUrl.AbsoluteUri, redirectResult.Url);
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
        public void Commit_WhenCalledWithoutUseOfReddit_AssertAcquireRedditAccessTokenAsyncWasNotCalledOnDataProviderFactory()
        {
            const bool useReddit = false;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _dataProviderFactoryMock.Verify(m => m.AcquireRedditAccessTokenAsync(
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
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(numberOfNews, useReddit);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(dashboardSettings =>
                    dashboardSettings != null &&
                    dashboardSettings.NumberOfNews == numberOfNews &&
                    dashboardSettings.UseReddit == false)),
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

        private OSDevGrp.MyDashboard.Web.Controllers.HomeController CreateSut(IDashboard dashboard = null, DashboardViewModel dashboardViewModel = null, Uri acquireRedditAccessTokenUrl = null, string redditClientId = null)
        {
            _dashboardFactoryMock.Setup(m => m.BuildAsync(It.IsAny<IDashboardSettings>()))
                .Returns(Task.Run<IDashboard>(() => dashboard ?? CreateDashboard()));

            _dashboardViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<IDashboard>()))
                .Returns(Task.Run<DashboardViewModel>(() => dashboardViewModel ?? new DashboardViewModel()));

            _dataProviderFactoryMock.Setup(m => m.AcquireRedditAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Uri>()))
                .Returns(Task.Run<Uri>(() => acquireRedditAccessTokenUrl ?? new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}")));

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
 
        private IDashboard CreateDashboard()
        {
            Mock<IDashboard> dashboardMock = new Mock<IDashboard>();
            return dashboardMock.Object;
        }
    }
}