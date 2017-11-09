using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.DashboardViewModelBuilder
{
    [TestClass]
    public class BuildAsyncTests
    {
        #region Private variables

        private Mock<IViewModelBuilder<InformationViewModel, INews>> _newsToInformationViewModelBuilderMock;
        private Mock<IViewModelBuilder<SystemErrorViewModel, ISystemError>> _systemErrorViewModelBuilderMock;
        private Mock<IHtmlHelper> _htmlHelperMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _newsToInformationViewModelBuilderMock = new Mock<IViewModelBuilder<InformationViewModel, INews>>();
            _systemErrorViewModelBuilderMock = new Mock<IViewModelBuilder<SystemErrorViewModel, ISystemError>>();
            _htmlHelperMock = new Mock<IHtmlHelper>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullExceptionAttribute("input")]
        public void BuildAsync_WhenDashboardIsNull_ThrowsArgumentNullException()
        {
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            sut.BuildAsync(null);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertNewsWasCalledOnDashboardTwice()
        {
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboardMock.Object);
            buildTask.Wait();

            dashboardMock.Verify(m => m.News, Times.Exactly(2));
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertSystemErrorsWasCalledOnDashboardTwice()
        {
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboardMock.Object);
            buildTask.Wait();

            dashboardMock.Verify(m => m.SystemErrors, Times.Exactly(2));
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertBuildAsyncWasCalledOnNewsToInformationViewModelBuilderForEachNewsInDashboard()
        {
            List<INews> newsCollection = CreateNewsCollection(_random.Next(50, 75)).ToList();
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            newsCollection.ForEach(news => _newsToInformationViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<INews>(value => value == news)), Times.Once));
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertBuildAsyncWasCalledOnSystemErrorViewModelBuilderForEachSystemErrorInDashboard()
        {
            List<ISystemError> systemErrorCollection = CreateSystemErrorCollection(_random.Next(50, 75)).ToList();
            IDashboard dashboard = CreateDashboard(systemErrorCollection: systemErrorCollection);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            systemErrorCollection.ForEach(systemError => _systemErrorViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<ISystemError>(value => value == systemError)), Times.Once));
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_ReturnsInitializedDashboardViewModel()
        {
            List<INews> newsCollection = CreateNewsCollection(_random.Next(50, 75)).ToList();
            List<ISystemError> systemErrorCollection = CreateSystemErrorCollection(_random.Next(10, 15)).ToList();
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            DashboardViewModel result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(newsCollection.Count, result.Informations.Count());
            Assert.IsNotNull(result.SystemErrors);
            Assert.AreEqual(systemErrorCollection.Count, result.SystemErrors.Count());
        }

        [TestMethod]
        public void BuildAsync_WhenCalledAndNewsToInformationViewModelBuilderThrowsAggregateException_AddsExceptionToSystemViewModelsInDashboardViewModel()
        {
            IEnumerable<INews> newsCollection = CreateNewsCollection(1);
            IEnumerable<ISystemError> systemErrorCollection = CreateSystemErrorCollection(0);
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection);

            string aggregateExceptionMessage = Guid.NewGuid().ToString();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(aggregateExceptionMessage: aggregateExceptionMessage);

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            DashboardViewModel result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(0, result.Informations.Count());
            Assert.IsNotNull(result.SystemErrors);
            Assert.AreEqual(1, result.SystemErrors.Count());
            
            SystemErrorViewModel systemErrorViewModel = result.SystemErrors.First();
            Assert.IsNotNull(systemErrorViewModel);
            Assert.IsNotNull(systemErrorViewModel.SystemErrorIdentifier);
            Assert.IsTrue(systemErrorViewModel.Timestamp >= DateTime.Now.AddSeconds(-3) && systemErrorViewModel.Timestamp <= DateTime.Now);
            Assert.IsNotNull(systemErrorViewModel.Message);
            Assert.AreEqual($"HtmlHelper.ConvertNewLines:{aggregateExceptionMessage}", systemErrorViewModel.Message);
            Assert.IsNotNull(systemErrorViewModel.Details);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledAndSystemErrorViewModelBuilderThrowsAggregateException_AddsExceptionToSystemViewModelsInDashboardViewModel()
        {
            IEnumerable<INews> newsCollection = CreateNewsCollection(0);
            IEnumerable<ISystemError> systemErrorCollection = CreateSystemErrorCollection(1);
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection);

            string aggregateExceptionMessage = Guid.NewGuid().ToString();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(aggregateExceptionMessage: aggregateExceptionMessage);

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            DashboardViewModel result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(0, result.Informations.Count());
            Assert.IsNotNull(result.SystemErrors);
            Assert.AreEqual(1, result.SystemErrors.Count());
            
            SystemErrorViewModel systemErrorViewModel = result.SystemErrors.First();
            Assert.IsNotNull(systemErrorViewModel);
            Assert.IsNotNull(systemErrorViewModel.SystemErrorIdentifier);
            Assert.IsTrue(systemErrorViewModel.Timestamp >= DateTime.Now.AddSeconds(-3) && systemErrorViewModel.Timestamp <= DateTime.Now);
            Assert.IsNotNull(systemErrorViewModel.Message);
            Assert.AreEqual($"HtmlHelper.ConvertNewLines:{aggregateExceptionMessage}", systemErrorViewModel.Message);
            Assert.IsNotNull(systemErrorViewModel.Details);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledAndAggregateExceptionOccurs_AssertConvertNewLinesWasCalledOnHtmlHelperTwice()
        {
            IEnumerable<INews> newsCollection = CreateNewsCollection(1);
            IEnumerable<ISystemError> systemErrorCollection = CreateSystemErrorCollection(0);
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection);

            string aggregateExceptionMessage = Guid.NewGuid().ToString();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(aggregateExceptionMessage: aggregateExceptionMessage);

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            _htmlHelperMock.Verify(m => m.ConvertNewLines(It.IsAny<string>()), Times.Exactly(2));
        }

        private IViewModelBuilder<DashboardViewModel, IDashboard> CreateSut(string aggregateExceptionMessage = null)
        {
            _newsToInformationViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<INews>()))
                .Returns(Task.Run<InformationViewModel>(() => 
                {
                    if (string.IsNullOrWhiteSpace(aggregateExceptionMessage) == false)
                    {
                        throw new Exception(aggregateExceptionMessage);
                    }
                    return CreateInformationViewModel(DateTime.Now.AddMinutes(_random.Next(1, 30) * -1));
                }));

            _systemErrorViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<ISystemError>()))
                .Returns(Task.Run<SystemErrorViewModel>(() => 
                {
                    if (string.IsNullOrWhiteSpace(aggregateExceptionMessage) == false)
                    {
                        throw new Exception(aggregateExceptionMessage);
                    }
                    return CreateSystemErrorViewModel(DateTime.Now.AddMinutes(_random.Next(1, 30) * -1));
                }));

            _htmlHelperMock.Setup(m => m.ConvertNewLines(It.IsAny<string>()))
                .Returns<string>(value => $"HtmlHelper.ConvertNewLines:{value}");

            return new OSDevGrp.MyDashboard.Web.Factories.DashboardViewModelBuilder(
                _newsToInformationViewModelBuilderMock.Object,
                _systemErrorViewModelBuilderMock.Object,
                _htmlHelperMock.Object);
        }

        private IDashboard CreateDashboard(IEnumerable<INews> newsCollection = null, IEnumerable<ISystemError> systemErrorCollection = null)
        {
            return CreateDashboardMock(newsCollection, systemErrorCollection).Object;
        }

        private Mock<IDashboard> CreateDashboardMock(IEnumerable<INews> newsCollection = null, IEnumerable<ISystemError> systemErrorCollection = null)
        {
            Mock<IDashboard> dashboardMock = new Mock<IDashboard>();
            dashboardMock.Setup(m => m.News)
                .Returns(newsCollection ?? CreateNewsCollection(_random.Next(50, 100)));
            dashboardMock.Setup(m => m.SystemErrors)
                .Returns(systemErrorCollection ?? CreateSystemErrorCollection(_random.Next(5, 10)));
            return dashboardMock;
        }

        private IEnumerable<INews> CreateNewsCollection(int numberOfNews)
        {
            IList<INews> newsCollection = new List<INews>(numberOfNews);
            while (newsCollection.Count < numberOfNews)
            {
                Mock<INews> newsMock = new Mock<INews>();
                newsCollection.Add(newsMock.Object);
            }
            return newsCollection;
        }

        private IEnumerable<ISystemError> CreateSystemErrorCollection(int numberOfSystemErrors)
        {
            IList<ISystemError> systemErrorCollection = new List<ISystemError>(numberOfSystemErrors);
            while (systemErrorCollection.Count < numberOfSystemErrors)
            {
                Mock<ISystemError> systemErrorMock = new Mock<ISystemError>();
                systemErrorCollection.Add(systemErrorMock.Object);
            }
            return systemErrorCollection;
        }

        private InformationViewModel CreateInformationViewModel(DateTime timestamp)
        {
            return new InformationViewModel
            {
                Timestamp = timestamp
            };
        }

        private SystemErrorViewModel CreateSystemErrorViewModel(DateTime timestamp)
        {
            return new SystemErrorViewModel
            {
                Timestamp = timestamp
            };
        }
    } 
}