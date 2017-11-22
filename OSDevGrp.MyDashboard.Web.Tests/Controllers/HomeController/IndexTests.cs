using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
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

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _dashboardFactoryMock = new Mock<IDashboardFactory>();
            _dashboardViewModelBuilderMock = new Mock<IViewModelBuilder<DashboardViewModel, IDashboard>>();
        }

        [TestMethod]
        public void Index_WhenCalled_AssertBuildAsyncWasCalledOnDashboardFactory()
        {
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Index();

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(dashboardSettings =>
                    dashboardSettings != null &&
                    dashboardSettings.NumberOfNews == 100)),
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

        private OSDevGrp.MyDashboard.Web.Controllers.HomeController CreateSut(IDashboard dashboard = null, DashboardViewModel dashboardViewModel = null)
        {
            _dashboardFactoryMock.Setup(m => m.BuildAsync(It.IsAny<IDashboardSettings>()))
                .Returns(Task.Run<IDashboard>(() => dashboard ?? BuildDashboard()));

            _dashboardViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<IDashboard>()))
                .Returns(Task.Run<DashboardViewModel>(() => dashboardViewModel ?? new DashboardViewModel()));
            
            return new OSDevGrp.MyDashboard.Web.Controllers.HomeController(
                _dashboardFactoryMock.Object,
                _dashboardViewModelBuilderMock.Object
            );
        }

        private IDashboard BuildDashboard()
        {
            Mock<IDashboard> dashboardMock = new Mock<IDashboard>();
            return dashboardMock.Object;
        }
    }
}