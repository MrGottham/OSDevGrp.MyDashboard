using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Factories.DashboardNewsBuilder
{
    [TestClass]
    public class ShouldBuildTests
    {
        #region Private variables

        private Mock<INewsLogic> _newsLogicMock;
        private Mock<IExceptionHandler> _exceptionHandlerMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _newsLogicMock = new Mock<INewsLogic>();
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
            _random = new Random(DateTime.Now.Millisecond);
        }
        
        [TestMethod]
        [ExpectedArgumentNullException("dashboardSettings")]
        public void ShouldBuild_WhenDashboardSettingsIsNull_ThrowsArgumentNullException()
        {
            IDashboardNewsBuilder sut = CreateSut();

            sut.ShouldBuild(null);
        }

        [TestMethod]
        public void ShouldBuild_WhenNumberOfNewsInDashboardSettingsIsLowerThanZero_ReturnsFalse()
        {
            int numberOfNews = _random.Next(1, 10) * -1;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(numberOfNews);

            IDashboardNewsBuilder sut = CreateSut();

            bool result = sut.ShouldBuild(dashboardSettings);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldBuild_WhenNumberOfNewsInDashboardSettingsIsEqualToZero_ReturnsFalse()
        {
            const int numberOfNews = 0;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(numberOfNews);

            IDashboardNewsBuilder sut = CreateSut();

            bool result = sut.ShouldBuild(dashboardSettings);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldBuild_WhenNumberOfNewsInDashboardSettingsIsGreaterThanZero_ReturnsTrue()
        {
            int numberOfNews = _random.Next(1, 10);
            IDashboardSettings dashboardSettings = CreateDashboardSettings(numberOfNews);

            IDashboardNewsBuilder sut = CreateSut();

            bool result = sut.ShouldBuild(dashboardSettings);

            Assert.IsTrue(result);
        }

        private IDashboardNewsBuilder CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Factories.DashboardNewsBuilder(
                _newsLogicMock.Object,
                _exceptionHandlerMock.Object
            );
        }

        private IDashboardSettings CreateDashboardSettings(int numberOfNews = 25)
        {
            Mock<IDashboardSettings> dashboardSettingsMock = new Mock<IDashboardSettings>();
            dashboardSettingsMock.Setup(m => m.NumberOfNews)
                .Returns(numberOfNews);
            return dashboardSettingsMock.Object;
        }
    }
}