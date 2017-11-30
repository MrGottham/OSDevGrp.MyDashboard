using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.DashboardSettingsViewModelBuilder
{
    [TestClass]
    public class BuildAsyncTests
    {
        #region Private variables

        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullExceptionAttribute("input")]
        public void BuildAsync_WhenDashboardSettingsIsNull_ThrowsArgumentNullException()
        {
            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            sut.BuildAsync(null);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertNumberOfNewsWasCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            Task<DashboardSettingsViewModel> buildTask = sut.BuildAsync(dashboardSettingsMock.Object);
            buildTask.Wait();

            dashboardSettingsMock.Verify(m => m.NumberOfNews, Times.Once());
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_ReturnsInitializedDashboardSettingsViewModel()
        {
            int numberOfNews = _random.Next(25, 50);
            IDashboardSettings dashboardSettings = CreateDashboardSettings(numberOfNews);

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            Task<DashboardSettingsViewModel> buildTask = sut.BuildAsync(dashboardSettings);
            buildTask.Wait();

            DashboardSettingsViewModel result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(numberOfNews, result.NumberOfNews);
        }

        private IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Factories.DashboardSettingsViewModelBuilder();
        }

        private IDashboardSettings CreateDashboardSettings(int? numberOfNews = null)
        {
            return CreateDashboardSettingsMock(numberOfNews).Object;
        }

        private Mock<IDashboardSettings> CreateDashboardSettingsMock(int? numberOfNews = null)
        {
            Mock<IDashboardSettings> dashboardSettingsMock = new Mock<IDashboardSettings>();
            dashboardSettingsMock.Setup(m => m.NumberOfNews)
                .Returns(numberOfNews ?? _random.Next(25, 50));
            return dashboardSettingsMock;
        }
    }
}