using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Factories.DashboardFactory
{
    [TestClass]
    public class BuildAsyncTests
    {
        #region Private variables

        private Mock<IDashboardContentBuilder> _dashboardContentBuilder1Mock;
        private Mock<IDashboardContentBuilder> _dashboardContentBuilder2Mock;
        private Mock<IDashboardContentBuilder> _dashboardContentBuilder3Mock;
        private Mock<ISystemErrorLogic> _systemErrorLogic;
        private Mock<IExceptionHandler> _exceptionHandlerMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _dashboardContentBuilder1Mock = new Mock<IDashboardContentBuilder>();
            _dashboardContentBuilder2Mock = new Mock<IDashboardContentBuilder>();
            _dashboardContentBuilder3Mock = new Mock<IDashboardContentBuilder>();
            _systemErrorLogic = new Mock<ISystemErrorLogic>();
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
            _random = new Random(DateTime.Now.Millisecond);
        }
        
        [TestMethod]
        [ExpectedArgumentNullException("dashboardSettings")]
        public async Task BuildAsync_WhenDashboardSettingsIsNull_ThrowsArgumentNullException()
        {
            IDashboardFactory sut = CreateSut();

            await sut.BuildAsync(null);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertShouldBuildWasCalledOnEachDashboardContentBuilder()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();

            IDashboardFactory sut = CreateSut();

            await sut.BuildAsync(dashboardSettings);

            _dashboardContentBuilder1Mock.Verify(m => m.ShouldBuild(It.Is<IDashboardSettings>(value => value == dashboardSettings)), Times.Once);
            _dashboardContentBuilder2Mock.Verify(m => m.ShouldBuild(It.Is<IDashboardSettings>(value => value == dashboardSettings)), Times.Once);
            _dashboardContentBuilder3Mock.Verify(m => m.ShouldBuild(It.Is<IDashboardSettings>(value => value == dashboardSettings)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertBuildAsyncWasCalledOnEachDashboardContentBuilderWhichShouldBuild()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();

            IDashboardFactory sut = CreateSut();

            await sut.BuildAsync(dashboardSettings);

            _dashboardContentBuilder1Mock.Verify(m => m.BuildAsync(
                    It.Is<IDashboardSettings>(value => value == dashboardSettings),
                    It.IsNotNull<IDashboard>()), 
                Times.Once);
            _dashboardContentBuilder2Mock.Verify(m => m.BuildAsync(
                    It.Is<IDashboardSettings>(value => value == dashboardSettings),
                    It.IsNotNull<IDashboard>()), 
                Times.Once);
            _dashboardContentBuilder3Mock.Verify(m => m.BuildAsync(
                    It.Is<IDashboardSettings>(value => value == dashboardSettings),
                    It.IsNotNull<IDashboard>()), 
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndFirstDashboardBuilderShouldNotBuild_AssertBuildAsyncWasNotCalledOnFirstDashboardContentBuilder()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();

            IDashboardFactory sut = CreateSut(dashboardContentBuilder1ShouldBuild: false);

            await sut.BuildAsync(dashboardSettings);

            _dashboardContentBuilder1Mock.Verify(m => m.BuildAsync(
                    It.IsAny<IDashboardSettings>(),
                    It.IsAny<IDashboard>()), 
                Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndSecondDashboardBuilderShouldNotBuild_AssertBuildAsyncWasNotCalledOnSecondDashboardContentBuilder()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();

            IDashboardFactory sut = CreateSut(dashboardContentBuilder2ShouldBuild: false);

            await sut.BuildAsync(dashboardSettings);

            _dashboardContentBuilder2Mock.Verify(m => m.BuildAsync(
                    It.IsAny<IDashboardSettings>(),
                    It.IsAny<IDashboard>()), 
                Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndThirdDashboardBuilderShouldNotBuild_AssertBuildAsyncWasNotCalledOnThirdDashboardContentBuilder()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();

            IDashboardFactory sut = CreateSut(dashboardContentBuilder3ShouldBuild: false);

            await sut.BuildAsync(dashboardSettings);

            _dashboardContentBuilder3Mock.Verify(m => m.BuildAsync(
                    It.IsAny<IDashboardSettings>(),
                    It.IsAny<IDashboard>()), 
                Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertGetSystemErrorsAsyncWasCalledOnSystemErrorLogic()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();

            IDashboardFactory sut = CreateSut(dashboardContentBuilder3ShouldBuild: false);

            await sut.BuildAsync(dashboardSettings);

            _systemErrorLogic.Verify(m => m.GetSystemErrorsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_ReturnsDashboard()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();

            List<ISystemError> systemErrors = BuildSystemErrors(_random.Next(1, 10));
            IDashboardFactory sut = CreateSut(systemErrors: systemErrors);

            IDashboard dashboard = await sut.BuildAsync(dashboardSettings);

            Assert.IsNotNull(dashboard);
            Assert.IsNotNull(dashboard.SystemErrors);
            Assert.AreEqual(systemErrors.Count, dashboard.SystemErrors.Count());
            Assert.IsNotNull(dashboard.Settings);
            Assert.AreEqual(dashboardSettings, dashboard.Settings);

            systemErrors.ForEach(systemError => Assert.IsTrue(dashboard.SystemErrors.Contains(systemError)));
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndAggregateExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();

            AggregateException aggregateException = new AggregateException();
            IDashboardFactory sut = CreateSut(provokeException: aggregateException);

            await sut.BuildAsync(dashboardSettings);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<AggregateException>(ex => ex == aggregateException)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndAggregateExceptionOccurs_AssertGetSystemErrorsAsyncWasCalledOnSystemErrorLogic()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();

            AggregateException aggregateException = new AggregateException();
            IDashboardFactory sut = CreateSut(provokeException: aggregateException);

            await sut.BuildAsync(dashboardSettings);

            _systemErrorLogic.Verify(m => m.GetSystemErrorsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndAggregateExceptionOccurs_ReturnsDashboard()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();

            AggregateException aggregateException = new AggregateException();
            List<ISystemError> systemErrors = BuildSystemErrors(_random.Next(1, 10));
            IDashboardFactory sut = CreateSut(provokeException: aggregateException, systemErrors: systemErrors);

            IDashboard dashboard = await sut.BuildAsync(dashboardSettings);
            Assert.IsNotNull(dashboard);
            Assert.IsNotNull(dashboard.SystemErrors);
            Assert.AreEqual(systemErrors.Count, dashboard.SystemErrors.Count());
            Assert.IsNotNull(dashboard.Settings);
            Assert.AreEqual(dashboardSettings, dashboard.Settings);

            systemErrors.ForEach(systemError => Assert.IsTrue(dashboard.SystemErrors.Contains(systemError)));
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();

            Exception exception = new Exception();
            IDashboardFactory sut = CreateSut(provokeException: exception);

            await sut.BuildAsync(dashboardSettings);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(ex => ex == exception)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndExceptionOccurs_AssertGetSystemErrorsAsyncWasCalledOnSystemErrorLogic()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();

            Exception exception = new Exception();
            IDashboardFactory sut = CreateSut(provokeException: exception);

            await sut.BuildAsync(dashboardSettings);

            _systemErrorLogic.Verify(m => m.GetSystemErrorsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndExceptionOccurs_ReturnsDashboard()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();

            Exception exception = new Exception();
            List<ISystemError> systemErrors = BuildSystemErrors(_random.Next(1, 10));
            IDashboardFactory sut = CreateSut(provokeException: exception, systemErrors: systemErrors);

            IDashboard dashboard = await sut.BuildAsync(dashboardSettings);
            Assert.IsNotNull(dashboard);
            Assert.IsNotNull(dashboard.SystemErrors);
            Assert.AreEqual(systemErrors.Count, dashboard.SystemErrors.Count());
            Assert.IsNotNull(dashboard.Settings);
            Assert.AreEqual(dashboardSettings, dashboard.Settings);

            systemErrors.ForEach(systemError => Assert.IsTrue(dashboard.SystemErrors.Contains(systemError)));
        }

        private IDashboardFactory CreateSut(bool dashboardContentBuilder1ShouldBuild = true, bool dashboardContentBuilder2ShouldBuild = true, bool dashboardContentBuilder3ShouldBuild = true, Exception provokeException = null, IEnumerable<ISystemError> systemErrors = null)
        {
            _dashboardContentBuilder1Mock.Setup(m => m.ShouldBuild(It.IsAny<IDashboardSettings>()))
                .Returns(dashboardContentBuilder1ShouldBuild);

            _dashboardContentBuilder2Mock.Setup(m => m.ShouldBuild(It.IsAny<IDashboardSettings>()))
                .Returns(dashboardContentBuilder2ShouldBuild);

            _dashboardContentBuilder3Mock.Setup(m => m.ShouldBuild(It.IsAny<IDashboardSettings>()))
                .Returns(dashboardContentBuilder3ShouldBuild);

            if (provokeException != null)
            {
                _dashboardContentBuilder1Mock.Setup(m => m.BuildAsync(It.IsAny<IDashboardSettings>(), It.IsAny<IDashboard>()))
                    .Throws(provokeException);
            }
            
            _systemErrorLogic.Setup(m => m.GetSystemErrorsAsync())
                .Returns(Task.Run<IEnumerable<ISystemError>>(() => systemErrors ?? BuildSystemErrors()));

            return new OSDevGrp.MyDashboard.Core.Factories.DashboardFactory(
                new List<IDashboardContentBuilder>
                {
                    _dashboardContentBuilder1Mock.Object,
                    _dashboardContentBuilder2Mock.Object,
                    _dashboardContentBuilder3Mock.Object
                },
                _systemErrorLogic.Object,
                _exceptionHandlerMock.Object
            );
        }

        private IDashboardSettings CreateDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = new Mock<IDashboardSettings>();
            return dashboardSettingsMock.Object;
        }

        private List<ISystemError> BuildSystemErrors(int numberOfSystemErrors = 7)
        {
            List<ISystemError> systemErrors = new List<ISystemError>(numberOfSystemErrors);
            while (systemErrors.Count < numberOfSystemErrors)
            {
                Mock<ISystemError> systemErrorMock = new Mock<ISystemError>();
                systemErrorMock.Setup(m => m.Timestamp)
                    .Returns(DateTime.Now);
                systemErrors.Add(systemErrorMock.Object);
            }
            return systemErrors;
        }
   }
}
