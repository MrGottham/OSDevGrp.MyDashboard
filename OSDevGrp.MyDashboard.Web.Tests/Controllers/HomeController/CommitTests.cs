using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Controllers.HomeController
{
    [TestClass]
    public class CommitTests
    {
        #region Private variables

        private Mock<IDashboardFactory> _dashboardFactoryMock;
        private Mock<IViewModelBuilder<DashboardViewModel, IDashboard>> _dashboardViewModelBuilderMock;
        private Mock<IConfiguration> _configurationMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _dashboardFactoryMock = new Mock<IDashboardFactory>();
            _dashboardViewModelBuilderMock = new Mock<IViewModelBuilder<DashboardViewModel, IDashboard>>();
            _configurationMock = new Mock<IConfiguration>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullException("dashboardSettingsViewModel")]
        public void Commit_WhenDashboardSettingsViewModelIsNull_ThrowsArgumentNullExcpetion()
        {
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(null);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfReddit_AssertAuthenticationRedditClientIdWasCalledOnConfiguration()
        {
            const bool useReddit = true;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _configurationMock.Verify(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)], Times.Once);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfReddit_AssertBuildAsyncWasNotCalledOnDashboardFactory()
        {
            const bool useReddit = true;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.IsAny<IDashboardSettings>()), Times.Never);
        }

        [TestMethod]
        public void Commit_WhenCalledWithUseOfReddit_AssertBuildAsyncWasNotCalledOnDashboardViewModelBuilder()
        {
            const bool useReddit = true;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _dashboardViewModelBuilderMock.Verify(m => m.BuildAsync(It.IsAny<IDashboard>()), Times.Never);
        }

        [TestMethod]
        public void Called_WhenCalledWithUseOfReddit_ReturnsNull()
        {
            const bool useReddit = true;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            IActionResult result = sut.Commit(dashboardSettingsViewModel);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Commit_WhenCalledWithoutUseOfReddit_AssertAuthenticationRedditClientIdWasNotCalledOnConfiguration()
        {
            const bool useReddit = false;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _configurationMock.Verify(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)], Times.Never);
        }

        [TestMethod]
        public void Commit_WhenCalledWithoutUseOfReddit_AssertBuildAsyncWasCalledOnDashboardFactory()
        {
            int numberOfNews = _random.Next(25, 50);
            const bool useReddit = false;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(numberOfNews, useReddit);

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.Commit(dashboardSettingsViewModel);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(dashboardSettings =>
                    dashboardSettings != null &&
                    dashboardSettings.NumberOfNews == numberOfNews &&
                    dashboardSettings.UseReddit == false)),
                Times.Once);
        }

        [TestMethod]
        public void Commit_WhenCalledWithoutUseOfReddit_AssertBuildAsyncWasCalledOnDashboardViewModelBuilder()
        {
            const bool useReddit = false;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            IDashboard dashboard = CreateDashboard();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboard: dashboard);

            sut.Commit(dashboardSettingsViewModel);

            _dashboardViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public void Called_WhenCalledWithoutUseOfReddit_ReturnsViewWithDashboardViewModel()
        {
            const bool useReddit = false;
            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel(useReddit: useReddit);

            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut(dashboardViewModel: dashboardViewModel);

            IActionResult result = sut.Commit(dashboardSettingsViewModel);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult) result;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewName);
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreEqual(dashboardViewModel, viewResult.Model);
        }

        private OSDevGrp.MyDashboard.Web.Controllers.HomeController CreateSut(IDashboard dashboard = null, DashboardViewModel dashboardViewModel = null, string redditClientId = null)
        {
            _dashboardFactoryMock.Setup(m => m.BuildAsync(It.IsAny<IDashboardSettings>()))
                .Returns(Task.Run<IDashboard>(() => dashboard ?? CreateDashboard()));

            _dashboardViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<IDashboard>()))
                .Returns(Task.Run<DashboardViewModel>(() => dashboardViewModel ?? new DashboardViewModel()));

            _configurationMock.Setup(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)])
                .Returns<string>(value => redditClientId ?? Guid.NewGuid().ToString("D"));
            
            return new OSDevGrp.MyDashboard.Web.Controllers.HomeController(
                _dashboardFactoryMock.Object,
                _dashboardViewModelBuilderMock.Object,
                _configurationMock.Object
            );
        }

        private DashboardSettingsViewModel CreateDashboardSettingsViewModel(int? numberOfNews = null, bool? useReddit = null)
        {
            return new DashboardSettingsViewModel
            {
                NumberOfNews = numberOfNews ?? _random.Next(25, 50),
                UseReddit = useReddit ?? _random.Next(100) > 50
            };
        }
 
        private IDashboard CreateDashboard()
        {
            Mock<IDashboard> dashboardMock = new Mock<IDashboard>();
            return dashboardMock.Object;
        }
    }
}