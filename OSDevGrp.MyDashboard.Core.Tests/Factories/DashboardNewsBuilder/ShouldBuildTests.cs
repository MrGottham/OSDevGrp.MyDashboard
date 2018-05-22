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
        public void ShouldBuild_WhenCalled_AssertNumberOfNewsWasCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();

            IDashboardNewsBuilder sut = CreateSut();

            bool result = sut.ShouldBuild(dashboardSettingsMock.Object);

            dashboardSettingsMock.Verify(m => m.NumberOfNews, Times.Once);
        }

        [TestMethod]
        public void ShouldBuild_WhenNumberOfNewsInDashboardSettingsIsLowerThanZero_AssertOnlyNsfwContentWasNotCalledOnDashboardSettings()
        {
            int numberOfNews = _random.Next(1, 10) * -1;
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock(numberOfNews);

            IDashboardNewsBuilder sut = CreateSut();

            bool result = sut.ShouldBuild(dashboardSettingsMock.Object);

            dashboardSettingsMock.Verify(m => m.OnlyNsfwContent, Times.Never);
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
        public void ShouldBuild_WhenNumberOfNewsInDashboardSettingsIsEqualToZero_AssertOnlyNsfwContentWasNotCalledOnDashboardSettings()
        {
            const int numberOfNews = 0;
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock(numberOfNews);

            IDashboardNewsBuilder sut = CreateSut();

            bool result = sut.ShouldBuild(dashboardSettingsMock.Object);

            dashboardSettingsMock.Verify(m => m.OnlyNsfwContent, Times.Never);
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
        public void ShouldBuild_WhenNumberOfNewsInDashboardSettingsIsGreaterThanZero_AssertOnlyNsfwContentWasCalledOnDashboardSettings()
        {
            int numberOfNews = _random.Next(1, 10);
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock(numberOfNews);

            IDashboardNewsBuilder sut = CreateSut();

            bool result = sut.ShouldBuild(dashboardSettingsMock.Object);

            dashboardSettingsMock.Verify(m => m.OnlyNsfwContent, Times.Once);
        }

        [TestMethod]
        public void ShouldBuild_WhenNumberOfNewsInDashboardSettingsIsGreaterThanZeroAndOnlyNsfwContentInDashboardSettingsIsTrue_ReturnsFalse()
        {
            int numberOfNews = _random.Next(1, 10);
            const bool onlyNsfwContent = true;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(numberOfNews, onlyNsfwContent);

            IDashboardNewsBuilder sut = CreateSut();

            bool result = sut.ShouldBuild(dashboardSettings);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldBuild_WhenNumberOfNewsInDashboardSettingsIsGreaterThanZeroAndOnlyNsfwContentInDashboardSettingsIsFalse_ReturnsTrue()
        {
            int numberOfNews = _random.Next(1, 10);
            const bool onlyNsfwContent = false;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(numberOfNews, onlyNsfwContent);

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

        private IDashboardSettings CreateDashboardSettings(int numberOfNews = 25, bool? onlyNsfwContent = null)
        {
            return CreateDashboardSettingsMock(numberOfNews, onlyNsfwContent).Object;
        }

        private Mock<IDashboardSettings> CreateDashboardSettingsMock(int numberOfNews = 25, bool? onlyNsfwContent = null)
        {
            Mock<IDashboardSettings> dashboardSettingsMock = new Mock<IDashboardSettings>();
            dashboardSettingsMock.Setup(m => m.NumberOfNews)
                .Returns(numberOfNews);
            dashboardSettingsMock.Setup(m => m.OnlyNsfwContent)
                .Returns(onlyNsfwContent ?? _random.Next(100) > 50);
            return dashboardSettingsMock;
        }
    }
}