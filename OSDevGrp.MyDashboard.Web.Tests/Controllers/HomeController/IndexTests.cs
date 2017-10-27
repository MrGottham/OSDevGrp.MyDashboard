using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Controllers.HomeController
{
    [TestClass]
    public class IndexTests
    {
        #region Private variables

        private Mock<IDashboardFactory> _dashboardFactoryMock;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _dashboardFactoryMock = new Mock<IDashboardFactory>();
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

        private OSDevGrp.MyDashboard.Web.Controllers.HomeController CreateSut(IDashboard dashboard = null)
        {
            _dashboardFactoryMock.Setup(m => m.BuildAsync(It.IsAny<IDashboardSettings>()))
                .Returns(Task.Run<IDashboard>(() => dashboard ?? BuildDashboard()));
            
            return new OSDevGrp.MyDashboard.Web.Controllers.HomeController(
                _dashboardFactoryMock.Object
            );
        }

        private IDashboard BuildDashboard()
        {
            Mock<IDashboard> dashboardMock = new Mock<IDashboard>();
            return dashboardMock.Object;
        }
    }
}