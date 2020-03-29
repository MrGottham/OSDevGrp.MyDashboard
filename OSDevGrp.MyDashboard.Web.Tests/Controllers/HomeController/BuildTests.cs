using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Controllers.HomeController
{
    [TestClass]
    public class BuildTests : TestBase
    {
        #region Private variables

        private Mock<IDashboardFactory> _dashboardFactoryMock;
        private Mock<IViewModelBuilder<DashboardViewModel, IDashboard>> _dashboardViewModelBuilderMock;
        private Mock<IRedditAccessTokenProviderFactory> _redditAccessTokenProviderFactoryMock;
        private Mock<IContentHelper> _contentHelperMock;
        private Mock<ICookieHelper> _cookieHelperMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _dashboardFactoryMock = new Mock<IDashboardFactory>();
            _dashboardViewModelBuilderMock = new Mock<IViewModelBuilder<DashboardViewModel, IDashboard>>();
            _redditAccessTokenProviderFactoryMock = new Mock<IRedditAccessTokenProviderFactory>();
            _contentHelperMock = new Mock<IContentHelper>();
            _cookieHelperMock = new Mock<ICookieHelper>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullException("dashboardSettings")]
        public async Task Build_WhenDashboardSettingsIsNull_ThrowsArgumentNullException()
        {
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            await sut.Build(null);
        }

        [TestMethod]
        [ExpectedArgumentNullException("dashboardSettings")]
        public async Task Build_WhenDashboardSettingsIsEmpty_ThrowsArgumentNullException()
        {
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            await sut.Build(string.Empty);
        }

        [TestMethod]
        [ExpectedArgumentNullException("dashboardSettings")]
        public async Task Build_WhenDashboardSettingsIsWhiteSpace_ThrowsArgumentNullException()
        {
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            await sut.Build(" ");
        }

        [TestMethod]
        [ExpectedArgumentNullException("dashboardSettings")]
        public async Task Build_WhenDashboardSettingsIsWhiteSpaces_ThrowsArgumentNullException()
        {
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            await sut.Build("  ");
        }

        [TestMethod]
        public async Task Build_WhenCalled_AssertToDashboardSettingsViewModelWasCalledOnContentHelperWithDashboardSettings()
        {
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            string dashboardSettings = Guid.NewGuid().ToString("D"); 
            await sut.Build(dashboardSettings);

            _contentHelperMock.Verify(m => m.ToDashboardSettingsViewModel(It.Is<string>(value => string.CompareOrdinal(value, dashboardSettings) == 0)), Times.Once);
        }

        [TestMethod]
        public async Task Build_WhenDashboardSettingsCannotBeConvertedToDashboardSettingsViewModel_AssertBuildAsyncWasNotCalledOnDashboardFactory()
        {
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(false);

            await sut.Build(Guid.NewGuid().ToString("D"));

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.IsAny<IDashboardSettings>()), Times.Never);
        }

        [TestMethod]
        public async Task Build_WhenDashboardSettingsCannotBeConvertedToDashboardSettingsViewModel_AssertBuildAsyncWasNotCalledOnDashboardViewModelBuilder()
        {
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(false);

            await sut.Build(Guid.NewGuid().ToString("D"));

            _dashboardViewModelBuilderMock.Verify(m => m.BuildAsync(It.IsAny<IDashboard>()), Times.Never);
        }

        [TestMethod]
        public async Task Build_WhenDashboardSettingsCannotBeConvertedToDashboardSettingsViewModel_ReturnsBadRequestResult()
        {
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(false);

            IActionResult result = await sut.Build(Guid.NewGuid().ToString("D"));

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task Build_WhenDashboardSettingsCanBeConvertedToDashboardSettingsViewModel_AssertBuildAsyncWasCalledOnDashboardFactoryWithNumberOfNewsFromDashboardSettingsViewModel()
        {
            int numberOfNews = _random.Next(25, 50);
            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, numberOfNews);
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardSettingsViewModel: dashboardSettingsViewModel);

            await sut.Build(Guid.NewGuid().ToString("D"));

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value != null && value.NumberOfNews == numberOfNews)), Times.Once);
        }

        [TestMethod]
        public async Task Build_WhenDashboardSettingsCanBeConvertedToDashboardSettingsViewModel_AssertBuildAsyncWasCalledOnDashboardFactoryWithUseRedditFromDashboardSettingsViewModel()
        {
            bool useReddit = _random.Next(100) > 50;
            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, useReddit: useReddit);
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardSettingsViewModel: dashboardSettingsViewModel);

            await sut.Build(Guid.NewGuid().ToString("D"));

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value != null && value.UseReddit == useReddit)), Times.Once);
        }

        [TestMethod]
        public async Task Build_WhenDashboardSettingsCanBeConvertedToDashboardSettingsViewModel_AssertBuildAsyncWasCalledOnDashboardFactoryWithRedditAccessTokenFromDashboardSettingsViewModel()
        {
            string redditAccessToken = BuildRedditAccessToken(_random).ToBase64();
            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, redditAccessToken: redditAccessToken);
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardSettingsViewModel: dashboardSettingsViewModel);

            await sut.Build(Guid.NewGuid().ToString("D"));

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value != null && value.RedditAccessToken != null && string.CompareOrdinal(value.RedditAccessToken.ToBase64(), redditAccessToken) == 0)), Times.Once);
        }

        [TestMethod]
        public async Task Build_WhenDashboardSettingsCanBeConvertedToDashboardSettingsViewModel_AssertBuildAsyncWasCalledOnDashboardFactoryWithIncludeNsfwContentFromDashboardSettingsViewModel()
        {
            bool includeNsfwContent = _random.Next(100) > 50;
            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, includeNsfwContent: includeNsfwContent);
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardSettingsViewModel: dashboardSettingsViewModel);

            await sut.Build(Guid.NewGuid().ToString("D"));

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value != null && value.IncludeNsfwContent == includeNsfwContent)), Times.Once);
        }

        [TestMethod]
        public async Task Build_WhenDashboardSettingsCanBeConvertedToDashboardSettingsViewModel_AssertBuildAsyncWasCalledOnDashboardFactoryWithOnlyNsfwContentFromDashboardSettingsViewModel()
        {
            bool onlyNsfwContent = _random.Next(100) > 50;
            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, onlyNsfwContent: onlyNsfwContent);
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardSettingsViewModel: dashboardSettingsViewModel);

            await sut.Build(Guid.NewGuid().ToString("D"));

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value != null && value.OnlyNsfwContent == onlyNsfwContent)), Times.Once);
        }

        [TestMethod]
        public async Task Build_WhenDashboardSettingsCanBeConvertedToDashboardSettingsViewModel_AssertBuildAsyncWasCalledOnDashboardViewModelBuilder()
        {
            IDashboard dashboard = BuildDashboard();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboard: dashboard);

            await sut.Build(Guid.NewGuid().ToString("D"));

            _dashboardViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public async Task Build_WhenDashboardSettingsCanBeConvertedToDashboardSettingsViewModel_ReturnsPartialViewResultWithBuildDashboardViewModel()
        {
            DashboardViewModel dashboardViewModel = BuildDashboardViewModel(_random);
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardViewModel: dashboardViewModel);

            IActionResult result = await sut.Build(Guid.NewGuid().ToString("D"));
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            PartialViewResult partialViewResult = (PartialViewResult) result;
            Assert.IsNotNull(partialViewResult);
            Assert.IsNotNull(partialViewResult.ViewName);
            Assert.AreEqual("_MainContentPartial", partialViewResult.ViewName);
            Assert.IsNotNull(partialViewResult.Model);
            Assert.IsInstanceOfType(partialViewResult.Model, typeof(DashboardViewModel));
            Assert.AreSame(dashboardViewModel, partialViewResult.Model);
        }

        private OSDevGrp.MyDashboard.Web.Controllers.HomeController CreateSut(bool hasDashboardSettingsViewModel = true, DashboardSettingsViewModel dashboardSettingsViewModel = null, IDashboard dashboard = null, DashboardViewModel dashboardViewModel = null)
        {
            _contentHelperMock.Setup(m => m.ToDashboardSettingsViewModel(It.IsAny<string>()))
                .Returns(hasDashboardSettingsViewModel ? dashboardSettingsViewModel ?? BuildDashboardSettingsViewModel(_random) : null);

            _dashboardFactoryMock.Setup(m => m.BuildAsync(It.IsAny<IDashboardSettings>()))
                .Returns(Task.Run(() => dashboard ?? BuildDashboard()));

            _dashboardViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<IDashboard>()))
                .Returns(Task.Run(() => dashboardViewModel ?? BuildDashboardViewModel(_random)));

            return new OSDevGrp.MyDashboard.Web.Controllers.HomeController(
                _dashboardFactoryMock.Object,
                _dashboardViewModelBuilderMock.Object,
                _redditAccessTokenProviderFactoryMock.Object,
                _contentHelperMock.Object,
                _cookieHelperMock.Object);
        }
    }
}