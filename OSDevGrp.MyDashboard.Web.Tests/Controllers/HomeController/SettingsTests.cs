using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;
using System;

namespace OSDevGrp.MyDashboard.Web.Tests.Controllers.HomeController
{
    [TestClass]
    public class SettingsTests : TestBase
    {
        #region Private variables

        private Mock<IDashboardFactory> _dashboardFactoryMock;
        private Mock<IViewModelBuilder<DashboardViewModel, IDashboard>> _dashboardViewModelBuilderMock;
        private Mock<IModelExporter<DashboardExportModel, IDashboard>> _dashboardModelExporterMock;
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
            _dashboardModelExporterMock = new Mock<IModelExporter<DashboardExportModel, IDashboard>>();
            _redditAccessTokenProviderFactoryMock = new Mock<IRedditAccessTokenProviderFactory>();
            _contentHelperMock = new Mock<IContentHelper>();
            _cookieHelperMock = new Mock<ICookieHelper>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        public void Settings_WhenDashboardSettingsIsNull_ReturnsBadRequestResult()
        {
            Web.Controllers.HomeController sut = CreateSut();

            IActionResult result = sut.Settings(null);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public void Settings_WhenDashboardSettingsIsEmpty_ReturnsBadRequestResult()
        {
            Web.Controllers.HomeController sut = CreateSut();

            IActionResult result = sut.Settings(string.Empty);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public void Settings_WhenDashboardSettingsIsWhiteSpace_ReturnsBadRequestResult()
        {
            Web.Controllers.HomeController sut = CreateSut();

            IActionResult result = sut.Settings(" ");

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public void Settings_WhenDashboardSettingsIsWhiteSpaces_ReturnsBadRequestResult()
        {
            Web.Controllers.HomeController sut = CreateSut();

            IActionResult result = sut.Settings("  ");

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public void Settings_WhenCalled_AssertToDashboardSettingsViewModelWasCalledOnContentHelperWithDashboardSettings()
        {
            Web.Controllers.HomeController sut = CreateSut();

            string dashboardSettings = Guid.NewGuid().ToString("D"); 
            sut.Settings(dashboardSettings);

            _contentHelperMock.Verify(m => m.ToDashboardSettingsViewModel(It.Is<string>(value => string.CompareOrdinal(value, dashboardSettings) == 0)), Times.Once);
        }

        [TestMethod]
        public void Settings_WhenDashboardSettingsCannotBeConvertedToDashboardSettingsViewModel_ReturnsBadRequestResult()
        {
            Web.Controllers.HomeController sut = CreateSut(false);

            IActionResult result = sut.Settings(Guid.NewGuid().ToString("D"));

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public void Settings_WhenDashboardSettingsCanBeConvertedToDashboardSettingsViewModel_ReturnsPartialViewResultForDashboardSettings()
        {
            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random);
            Web.Controllers.HomeController sut = CreateSut(dashboardSettingsViewModel: dashboardSettingsViewModel);

            IActionResult result = sut.Settings(Guid.NewGuid().ToString("D"));
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            PartialViewResult viewResult = (PartialViewResult) result;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewName);
            Assert.AreEqual("_DashboardSettingsPartial", viewResult.ViewName);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreEqual(dashboardSettingsViewModel, viewResult.Model);
        }

        private Web.Controllers.HomeController CreateSut(bool hasDashboardSettingsViewModel = true, DashboardSettingsViewModel dashboardSettingsViewModel = null)
        {
            _contentHelperMock.Setup(m => m.ToDashboardSettingsViewModel(It.IsAny<string>()))
                .Returns(hasDashboardSettingsViewModel ? dashboardSettingsViewModel ?? BuildDashboardSettingsViewModel(_random) : null);

            return new Web.Controllers.HomeController(
                _dashboardFactoryMock.Object,
                _dashboardViewModelBuilderMock.Object,
                _dashboardModelExporterMock.Object,
                _redditAccessTokenProviderFactoryMock.Object,
                _contentHelperMock.Object,
                _cookieHelperMock.Object);
        }
    }
}