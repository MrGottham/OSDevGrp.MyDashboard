using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Web.Tests.Controllers.HomeController
{
    [TestClass]
    public class CommitTests : TestBase
    {
        #region Private variables

        private Mock<IDashboardFactory> _dashboardFactoryMock;
        private Mock<IViewModelBuilder<DashboardViewModel, IDashboard>> _dashboardViewModelBuilderMock;
        private Mock<IModelExporter<DashboardExportModel, IDashboard>> _dashboardModelExporterMock;
        private Mock<IRedditAccessTokenProviderFactory> _redditAccessTokenProviderFactoryMock;
        private Mock<IRedditLogic> _redditLogicMock;
        private Mock<IContentHelper> _contentHelperMock;
        private Mock<ICookieHelper> _cookieHelperMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _dashboardFactoryMock = new Mock<IDashboardFactory>();
            _dashboardViewModelBuilderMock = new Mock<IViewModelBuilder<DashboardViewModel, IDashboard>>();
            _dashboardModelExporterMock = new Mock<IModelExporter<DashboardExportModel, IDashboard>>();
            _redditAccessTokenProviderFactoryMock = new Mock<IRedditAccessTokenProviderFactory>();
            _redditLogicMock = new Mock<IRedditLogic>();
            _contentHelperMock = new Mock<IContentHelper>();
            _cookieHelperMock = new Mock<ICookieHelper>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullException("dashboardSettingsViewModel")]
        public async Task Commit_WhenDashboardSettingsViewModelIsNull_ThrowsArgumentNullExcpetion()
        {
            Web.Controllers.HomeController sut = CreateSut();

            await sut.Commit(null);
        }

        [TestMethod]
        public async Task Commit_WhenCalledWithInvalidDashboardSettingsViewModel_ReturnsRedirectToActionResult()
        {
            Web.Controllers.HomeController sut = CreateSut(false);

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random);

            IActionResult result = await sut.Commit(dashboardSettingsViewModel);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            RedirectToActionResult redirectToActionResult = (RedirectToActionResult) result;
            Assert.IsNotNull(redirectToActionResult);
            Assert.IsNotNull(redirectToActionResult.ActionName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            Assert.IsNotNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("HomeController", redirectToActionResult.ControllerName);
        }

        [TestMethod]
        public async Task Commit_WhenDashboardSettingsViewModelIsWithUseOfRedditAndWithoutRedditAccessToken_AssertToBase64StringWasCalledOnContentHelperWithDashboardSettingsViewModel()
        {
            Web.Controllers.HomeController sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, useReddit: true, redditAccessToken: null);

            await sut.Commit(dashboardSettingsViewModel);

            _contentHelperMock.Verify(m => m.ToBase64String(It.Is<DashboardSettingsViewModel>(value => value == dashboardSettingsViewModel)), Times.Once);
        }

        [TestMethod]
        public async Task Commit_WhenDashboardSettingsViewModelIsWithUseOfRedditAndWithoutRedditAccessToken_AssertAbsoluteUrlWasCalledOnContentHelper()
        {
            Web.Controllers.HomeController sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, useReddit: true, redditAccessToken: null);

            await sut.Commit(dashboardSettingsViewModel);

            _contentHelperMock.Verify(m => m.AbsoluteUrl(
                    It.Is<string>(value => string.CompareOrdinal(value, "RedditCallback") == 0),
                    It.Is<string>(value => string.CompareOrdinal(value, "Home") == 0 )), 
                Times.Once);
        }

        [TestMethod]
        public async Task Commit_WhenDashboardSettingsViewModelIsWithUseOfRedditAndWithoutRedditAccessToken_AssertAcquireRedditAuthorizationTokenAsyncWasCalledOnRedditAccessTokenProviderFactory()
        {
            string dashboardSettingsViewModelAsBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("D")));
            Web.Controllers.HomeController sut = CreateSut(dashboardSettingsViewModelAsBase64: dashboardSettingsViewModelAsBase64);

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, useReddit: true, redditAccessToken: null);

            await sut.Commit(dashboardSettingsViewModel);

            _redditAccessTokenProviderFactoryMock.Verify(m => m.AcquireRedditAuthorizationTokenAsync(
                    It.Is<string>(value => string.CompareOrdinal(value, dashboardSettingsViewModelAsBase64) == 0), 
                    It.Is<Uri>(value => value != null && string.CompareOrdinal(value.AbsoluteUri, $"http://localhost/Home/RedditCallback") == 0)), 
                Times.Once);
        }

        [TestMethod]
        public async Task Commit_WhenDashboardSettingsViewModelIsWithUseOfRedditAndWithoutRedditAccessToken_ReturnsRedirectResultToUrlFromAcquireRedditAuthorizationTokenAsyncOnRedditAccessTokenProviderFactory()
        {
            Uri acquireRedditAuthorizationTokenUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}/{Guid.NewGuid().ToString("D")}");
            Web.Controllers.HomeController sut = CreateSut(acquireRedditAuthorizationTokenUri: acquireRedditAuthorizationTokenUri);

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, useReddit: true, redditAccessToken: null);

            IActionResult result = await sut.Commit(dashboardSettingsViewModel);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));

            RedirectResult redirectResult = (RedirectResult) result;
            Assert.IsNotNull(redirectResult);
            Assert.IsNotNull(redirectResult.Url);
            Assert.AreEqual(redirectResult.Url, acquireRedditAuthorizationTokenUri.AbsoluteUri);
        }

        [TestMethod]
        public async Task Commit_WhenDashboardSettingsViewModelIsWithUseOfRedditAndWithRedditAccessToken_AssertToBase64StringWasNotCalledOnContentHelperWithDashboardSettingsViewModel()
        {
            Web.Controllers.HomeController sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, useReddit: true, redditAccessToken: BuildRedditAccessTokenAsBase64(_random));

            await sut.Commit(dashboardSettingsViewModel);

            _contentHelperMock.Verify(m => m.ToBase64String(It.IsAny<DashboardSettingsViewModel>()), Times.Never);
        }

        [TestMethod]
        public async Task Commit_WhenDashboardSettingsViewModelIsWithUseOfRedditAndWithRedditAccessToken_AssertAbsoluteUrlWasNotCalledOnContentHelper()
        {
            Web.Controllers.HomeController sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, useReddit: true, redditAccessToken: BuildRedditAccessTokenAsBase64(_random));

            await sut.Commit(dashboardSettingsViewModel);

            _contentHelperMock.Verify(m => m.AbsoluteUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task Commit_WhenDashboardSettingsViewModelIsWithUseOfRedditAndWithRedditAccessToken_AssertAcquireRedditAuthorizationTokenAsyncWasNotCalledOnRedditAccessTokenProviderFactory()
        {
            Web.Controllers.HomeController sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, useReddit: true, redditAccessToken: BuildRedditAccessTokenAsBase64(_random));

            await sut.Commit(dashboardSettingsViewModel);

            _redditAccessTokenProviderFactoryMock.Verify(m => m.AcquireRedditAuthorizationTokenAsync(It.IsAny<string>(), It.IsAny<Uri>()), Times.Never);
        }

        [TestMethod]
        public async Task Commit_WhenDashboardSettingsViewModelIsWithUseOfRedditAndWithRedditAccessToken_ReturnsViewResultWithDashboardSettingsViewModel()
        {
            Web.Controllers.HomeController sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, useReddit: true, redditAccessToken: BuildRedditAccessTokenAsBase64(_random));

            IActionResult result = await sut.Commit(dashboardSettingsViewModel);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult) result;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewName);
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreEqual(dashboardSettingsViewModel, viewResult.Model);
        }

        [TestMethod]
        public async Task Commit_WhenDashboardSettingsViewModelIsWithoutUseOfReddit_AssertToBase64StringWasNotCalledOnContentHelperWithDashboardSettingsViewModel()
        {
            Web.Controllers.HomeController sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, useReddit: false);

            await sut.Commit(dashboardSettingsViewModel);

            _contentHelperMock.Verify(m => m.ToBase64String(It.IsAny<DashboardSettingsViewModel>()), Times.Never);
        }

        [TestMethod]
        public async Task Commit_WhenDashboardSettingsViewModelIsWithoutUseOfReddit_AssertAbsoluteUrlWasNotCalledOnContentHelper()
        {
            Web.Controllers.HomeController sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, useReddit: false);

            await sut.Commit(dashboardSettingsViewModel);

            _contentHelperMock.Verify(m => m.AbsoluteUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task Commit_WhenDashboardSettingsViewModelIsWithoutUseOfReddit_AssertAcquireRedditAuthorizationTokenAsyncWasNotCalledOnRedditAccessTokenProviderFactory()
        {
            Web.Controllers.HomeController sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, useReddit: false);

            await sut.Commit(dashboardSettingsViewModel);

            _redditAccessTokenProviderFactoryMock.Verify(m => m.AcquireRedditAuthorizationTokenAsync(It.IsAny<string>(), It.IsAny<Uri>()), Times.Never);
        }

        [TestMethod]
        public async Task Commit_WhenDashboardSettingsViewModelIsWithoutUseOfReddit_ReturnsViewResultWithDashboardSettingsViewModel()
        {
            Web.Controllers.HomeController sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, useReddit: false);

            IActionResult result = await sut.Commit(dashboardSettingsViewModel);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult) result;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewName);
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreEqual(dashboardSettingsViewModel, viewResult.Model);
        }

        private Web.Controllers.HomeController CreateSut(bool modelIsValid = true, string dashboardSettingsViewModelAsBase64 = null, Uri acquireRedditAuthorizationTokenUri = null)
        {
            _contentHelperMock.Setup(m => m.ToBase64String(It.IsAny<DashboardSettingsViewModel>()))
                .Returns(dashboardSettingsViewModelAsBase64 ?? Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("D"))));
            _contentHelperMock.Setup(m => m.AbsoluteUrl(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((action, controller) => new Uri($"http://localhost/{controller}/{action}").AbsoluteUri);

            _redditAccessTokenProviderFactoryMock.Setup(m => m.AcquireRedditAuthorizationTokenAsync(It.IsAny<string>(), It.IsAny<Uri>()))
                .Returns(Task.Run(() => acquireRedditAuthorizationTokenUri ?? new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}/{Guid.NewGuid().ToString("D")}")));

            Web.Controllers.HomeController homeController = new Web.Controllers.HomeController(
                _dashboardFactoryMock.Object,
                _dashboardViewModelBuilderMock.Object,
                _dashboardModelExporterMock.Object,
                _redditAccessTokenProviderFactoryMock.Object,
                _redditLogicMock.Object,
                _contentHelperMock.Object,
                _cookieHelperMock.Object);

            if (modelIsValid == false)
            {
                homeController.ModelState.AddModelError(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"));
            }

            return homeController;
        }
    }
}