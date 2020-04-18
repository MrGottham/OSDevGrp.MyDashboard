using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Controllers.HomeController
{
    [TestClass]
    public class IndexTests : TestBase
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
        public void Index_WhenCalled_AssertToDashboardSettingsViewModelWasCalledOnCookieHelper()
        {
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Index();

            _cookieHelperMock.Verify(m => m.ToDashboardSettingsViewModel(), Times.Once);
        }

        [TestMethod]
        public void Index_WhenCookieWithDashboardSettingsWasNotFound_ReturnsViewResultWithDashboardSettingsViewModel()
        {
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            IActionResult result = sut.Index();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult) result;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewName);
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(DashboardSettingsViewModel));

            DashboardSettingsViewModel dashboardSettingsViewModel = (DashboardSettingsViewModel) viewResult.Model;
            Assert.IsNotNull(dashboardSettingsViewModel);
            Assert.AreEqual(100, dashboardSettingsViewModel.NumberOfNews);
            Assert.IsFalse(dashboardSettingsViewModel.UseReddit);
            Assert.IsFalse(dashboardSettingsViewModel.AllowNsfwContent);
            Assert.IsNotNull(dashboardSettingsViewModel.IncludeNsfwContent);
            Assert.IsFalse(dashboardSettingsViewModel.IncludeNsfwContent.Value);
            Assert.IsFalse(dashboardSettingsViewModel.NotNullableIncludeNsfwContent);
            Assert.IsNotNull(dashboardSettingsViewModel.OnlyNsfwContent);
            Assert.IsFalse(dashboardSettingsViewModel.OnlyNsfwContent.Value);
            Assert.IsFalse(dashboardSettingsViewModel.NotNullableOnlyNsfwContent);
            Assert.IsNull(dashboardSettingsViewModel.RedditAccessToken);
            Assert.IsFalse(dashboardSettingsViewModel.ExportData);
        }

        [TestMethod]
        public void Index_WhenCookieWithDashboardSettingsWasFound_ReturnsViewResultWithDashboardSettingsViewModel()
        {
            int numberOfNews = _random.Next(25, 50);
            bool useReddit = _random.Next(100) > 50;
            bool allowNsfwContent = _random.Next(100) > 50;
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;
            string redditAccessToken = BuildRedditAccessTokenAsBase64(_random);
            bool exportData = _random.Next(100) > 50;
            DashboardSettingsViewModel dashboardSettingsViewModelInCookie = BuildDashboardSettingsViewModel(_random, numberOfNews, useReddit, allowNsfwContent, includeNsfwContent, onlyNsfwContent, redditAccessToken, exportData);
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(true, dashboardSettingsViewModelInCookie);

            IActionResult result = sut.Index();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult) result;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewName);
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(DashboardSettingsViewModel));

            DashboardSettingsViewModel dashboardSettingsViewModel = (DashboardSettingsViewModel) viewResult.Model;
            Assert.IsNotNull(dashboardSettingsViewModel);
            Assert.AreEqual(numberOfNews, dashboardSettingsViewModel.NumberOfNews);
            Assert.AreEqual(useReddit, dashboardSettingsViewModel.UseReddit);
            Assert.AreEqual(allowNsfwContent, dashboardSettingsViewModel.AllowNsfwContent);
            Assert.IsNotNull(dashboardSettingsViewModel.IncludeNsfwContent);
            Assert.AreEqual(includeNsfwContent, dashboardSettingsViewModel.IncludeNsfwContent.Value);
            Assert.AreEqual(includeNsfwContent, dashboardSettingsViewModel.NotNullableIncludeNsfwContent);
            Assert.IsNotNull(dashboardSettingsViewModel.OnlyNsfwContent);
            Assert.AreEqual(onlyNsfwContent, dashboardSettingsViewModel.OnlyNsfwContent.Value);
            Assert.AreEqual(onlyNsfwContent, dashboardSettingsViewModel.NotNullableOnlyNsfwContent);
            Assert.IsNotNull(dashboardSettingsViewModel.RedditAccessToken);
            Assert.AreEqual(redditAccessToken, dashboardSettingsViewModel.RedditAccessToken);
            Assert.AreEqual(exportData, dashboardSettingsViewModel.ExportData);
        }

        private OSDevGrp.MyDashboard.Web.Controllers.HomeController CreateSut(bool hasDashboardSettingsViewModelInCookie = false, DashboardSettingsViewModel dashboardSettingsViewModelInCookie = null)
        {
            _cookieHelperMock.Setup(m => m.ToDashboardSettingsViewModel())
                .Returns(hasDashboardSettingsViewModelInCookie ? dashboardSettingsViewModelInCookie ?? BuildDashboardSettingsViewModel(_random) : null);

            return new OSDevGrp.MyDashboard.Web.Controllers.HomeController(
                _dashboardFactoryMock.Object,
                _dashboardViewModelBuilderMock.Object,
                _dashboardModelExporterMock.Object,
                _redditAccessTokenProviderFactoryMock.Object,
                _contentHelperMock.Object,
                _cookieHelperMock.Object);
        }
    }
}