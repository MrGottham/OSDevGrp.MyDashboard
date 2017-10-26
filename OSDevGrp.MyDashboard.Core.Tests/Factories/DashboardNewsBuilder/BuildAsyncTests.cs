using System;
using System.Collections.Generic;
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
    public class BuildAsyncTests
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
        public void BuildAsync_WhenDashboardSettingsIsNull_ThrowsArgumentNullException()
        {
            IDashboard dashboard = CreateDashboard();

            IDashboardNewsBuilder sut = CreateSut();

            sut.BuildAsync(null, dashboard);
        }

        [TestMethod]
        [ExpectedArgumentNullException("dashboard")]
        public void BuildAsync_WhenDashboardIsNull_ThrowsArgumentNullException()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();

            IDashboardNewsBuilder sut = CreateSut();

            sut.BuildAsync(dashboardSettings, null);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertGetNewsAsyncWasCalledOnNewsLogic()
        {
            int numberOfNews = _random.Next(25, 75);

            IDashboardSettings dashboardSettings = CreateDashboardSettings(numberOfNews);
            IDashboard dashboard = CreateDashboard();

            IDashboardNewsBuilder sut = CreateSut();

            Task buildTask = sut.BuildAsync(dashboardSettings, dashboard);
            buildTask.Wait();

            _newsLogicMock.Verify(m => m.GetNewsAsync(It.Is<int>(value => value == numberOfNews)), Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertReplaceWasCalledOnDashboardWithNews()
        {
            IEnumerable<INews> news = BuildNews(_random.Next(25, 75));

            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IDashboardNewsBuilder sut = CreateSut(news);

            Task buildTask = sut.BuildAsync(dashboardSettings, dashboardMock.Object);
            buildTask.Wait();

            dashboardMock.Verify(m => m.Replace(It.Is<IEnumerable<INews>>(value => value == news)), Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IDashboard dashboard = CreateDashboard();

            Exception exception = new Exception();
            IDashboardNewsBuilder sut = CreateSut(provokedException: exception);

            Task buildTask = sut.BuildAsync(dashboardSettings, dashboard);
            buildTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(ex => ex == exception)), Times.Once);
        }

        private IDashboardNewsBuilder CreateSut(IEnumerable<INews> news = null, Exception provokedException = null)
        {
            if (provokedException != null)
            {
                _newsLogicMock.Setup(m => m.GetNewsAsync(It.IsAny<int>()))
                    .Throws(provokedException);
            }
            else
            {
                _newsLogicMock.Setup(m => m.GetNewsAsync(It.IsAny<int>()))
                    .Returns(Task.Run<IEnumerable<INews>>(() => news ?? BuildNews()));
            }
            
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

        private IDashboard CreateDashboard()
        {
            IMock<IDashboard> dashboardMock = CreateDashboardMock();
            return dashboardMock.Object;
        }

        private Mock<IDashboard> CreateDashboardMock()
        {
            Mock<IDashboard> dashboardMock = new Mock<IDashboard>();
            return dashboardMock;
        }

        private IEnumerable<INews> BuildNews(int numberOfNews = 7)
        {
            IList<INews> news = new List<INews>(numberOfNews);
            while (news.Count < numberOfNews)
            {
                Mock<INews> newsMock = new Mock<INews>();
                newsMock.Setup(m => m.Timestamp)
                    .Returns(DateTime.Now);
                news.Add(newsMock.Object);
            }
            return news;
        }
    }
}