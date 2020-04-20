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
    public class TopContentTests : TestBase
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
        public void TopContent_WhenCalled_AssertToDashboardViewModelWasCalledOnCookieHelper()
        {
            Web.Controllers.HomeController sut = CreateSut();

            sut.TopContent();

            _cookieHelperMock.Verify(m => m.ToDashboardViewModel());
        }

        [TestMethod]
        public void TopContent_WhenDashboardViewModelWasNotStoredInCoookie_ReturnsPartialViewResultForEmptyContent()
        {
            Web.Controllers.HomeController sut = CreateSut(false);

            IActionResult result = sut.TopContent();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            PartialViewResult viewResult = (PartialViewResult) result;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewName);
            Assert.AreEqual("_EmptyContentPartial", viewResult.ViewName);
            Assert.IsNull(viewResult.Model);
        }

        [TestMethod]
        public void TopContent_WhenDashboardViewModelWasStoredInCoookie_ReturnsPartialViewResultForTopContent()
        {
            DashboardViewModel dashboardViewModel = BuildDashboardViewModel(_random);
            Web.Controllers.HomeController sut = CreateSut(dashboardViewModel: dashboardViewModel);

            IActionResult result = sut.TopContent();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            PartialViewResult viewResult = (PartialViewResult) result;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewName);
            Assert.AreEqual("_TopContentPartial", viewResult.ViewName);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreEqual(dashboardViewModel, viewResult.Model);
        }

        private Web.Controllers.HomeController CreateSut(bool hasDashboardViewModel = true, DashboardViewModel dashboardViewModel = null)
        {
            _cookieHelperMock.Setup(m => m.ToDashboardViewModel())
                .Returns(hasDashboardViewModel ? dashboardViewModel ?? BuildDashboardViewModel(_random) : null);

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