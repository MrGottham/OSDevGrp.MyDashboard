using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.DashboardModelExporter
{
    [TestClass]
    public class ExportAsyncTests
    {
        #region Private variables

        private Mock<IModelExporter<DashboardItemExportModel, INews>> _newsModelExporterMock;
        private Mock<IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser>> _redditAuthenticatedUserModelExporterMock;
        private Mock<IModelExporter<DashboardItemExportModel, IRedditSubreddit>> _redditSubredditModelExporterMock;
        private Mock<IModelExporter<DashboardItemExportModel, IRedditLink>> _redditLinkModelExporterMock;  
        private Mock<IExceptionHandler> _exceptionHandlerMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _newsModelExporterMock = new Mock<IModelExporter<DashboardItemExportModel, INews>>();
            _redditAuthenticatedUserModelExporterMock = new Mock<IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser>>();
            _redditSubredditModelExporterMock = new Mock<IModelExporter<DashboardItemExportModel, IRedditSubreddit>>();
            _redditLinkModelExporterMock = new Mock<IModelExporter<DashboardItemExportModel, IRedditLink>>();
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullException("input")]
        public async Task ExportAsync_WhenInputIsNull_ThrowsArgumentNullException()
        {
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut();

            await sut.ExportAsync(null);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertNewsWasCalledOnDashboard()
        {
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut();

            Mock<IDashboard> dashboardMock = BuildDashboardMock();
            await sut.ExportAsync(dashboardMock.Object);

            dashboardMock.Verify(m => m.News, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenDashboardHasNews_AssertExportAsyncWasCalledOnNewsModelExporterForEachNews()
        {
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut();

            IEnumerable<INews> newsCollection = BuildNewsCollection();
            IDashboard dashboard = BuildDashboard(newsCollection: newsCollection);
            await sut.ExportAsync(dashboard);

            foreach (INews news in newsCollection)
            {
                _newsModelExporterMock.Verify(m => m.ExportAsync(It.Is<INews>(value => value == news)), Times.Once);
            }
        }

        [TestMethod]
        public async Task ExportAsync_WhenDashboardDoesNotHaveNews_AssertExportAsyncWasNotCalledOnNewsModelExporter()
        {
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut();

            IDashboard dashboard = BuildDashboard(false);
            await sut.ExportAsync(dashboard);

            _newsModelExporterMock.Verify(m => m.ExportAsync(It.IsAny<INews>()), Times.Never);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertRedditAuthenticatedUserWasCalledOnDashboard()
        {
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut();

            Mock<IDashboard> dashboardMock = BuildDashboardMock();
            await sut.ExportAsync(dashboardMock.Object);

            dashboardMock.Verify(m => m.RedditAuthenticatedUser, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenDashboardHasRedditAuthenticatedUser_AssertExportAsyncWasCalledOnRedditAuthenticatedUserModelExporter()
        {
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut();

            IRedditAuthenticatedUser redditAuthenticatedUser = BuildRedditAuthenticatedUser();
            IDashboard dashboard = BuildDashboard(redditAuthenticatedUser: redditAuthenticatedUser);
            await sut.ExportAsync(dashboard);

            _redditAuthenticatedUserModelExporterMock.Verify(m => m.ExportAsync(It.Is<IRedditAuthenticatedUser>(value => value == redditAuthenticatedUser)), Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenDashboardDoesNotHaveRedditAuthenticatedUser_AssertExportAsyncWasNotCalledOnRedditAuthenticatedUserModelExporter()
        {
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut();

            IDashboard dashboard = BuildDashboard(hasRedditAuthenticatedUser: false);
            await sut.ExportAsync(dashboard);

            _redditAuthenticatedUserModelExporterMock.Verify(m => m.ExportAsync(It.IsAny<IRedditAuthenticatedUser>()), Times.Never);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertRedditSubredditsWasCalledOnDashboard()
        {
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut();

            Mock<IDashboard> dashboardMock = BuildDashboardMock();
            await sut.ExportAsync(dashboardMock.Object);

            dashboardMock.Verify(m => m.RedditSubreddits, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenDashboardHasRedditSubreddits_AssertExportAsyncWasCalledOnRedditSubredditModelExporterForEachRedditSubreddit()
        {
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut();

            IEnumerable<IRedditSubreddit> redditSubredditCollection = BuildRedditSubredditCollection();
            IDashboard dashboard = BuildDashboard(redditSubredditCollection: redditSubredditCollection);
            await sut.ExportAsync(dashboard);

            foreach (IRedditSubreddit redditSubreddit in redditSubredditCollection)
            {
                _redditSubredditModelExporterMock.Verify(m => m.ExportAsync(It.Is<IRedditSubreddit>(value => value == redditSubreddit)), Times.Once);
            }
        }

        [TestMethod]
        public async Task ExportAsync_WhenDashboardDoesNotHaveRedditSubreddits_AssertExportAsyncWasNotCalledOnRedditSubredditModelExporter()
        {
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut();

            IDashboard dashboard = BuildDashboard(hasRedditSubreddits: false);
            await sut.ExportAsync(dashboard);

            _redditSubredditModelExporterMock.Verify(m => m.ExportAsync(It.IsAny<IRedditSubreddit>()), Times.Never);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertRedditLinksWasCalledOnDashboard()
        {
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut();

            Mock<IDashboard> dashboardMock = BuildDashboardMock();
            await sut.ExportAsync(dashboardMock.Object);

            dashboardMock.Verify(m => m.RedditLinks, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenDashboardHasRedditLinks_AssertExportAsyncWasCalledOnRedditLinkModelExporterForEachRedditLink()
        {
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut();

            IEnumerable<IRedditLink> redditLinkCollection = BuildRedditLinkCollection();
            IDashboard dashboard = BuildDashboard(redditLinkCollection: redditLinkCollection);
            await sut.ExportAsync(dashboard);

            foreach (IRedditLink redditLink in redditLinkCollection)
            {
                _redditLinkModelExporterMock.Verify(m => m.ExportAsync(It.Is<IRedditLink>(value => value == redditLink)), Times.Once);
            }
        }

        [TestMethod]
        public async Task ExportAsync_WhenDashboardDoesNotHaveRedditLinks_AssertExportAsyncWasNotCalledOnRedditLinkModelExporter()
        {
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut();

            IDashboard dashboard = BuildDashboard(hasRedditLinks: false);
            await sut.ExportAsync(dashboard);

            _redditLinkModelExporterMock.Verify(m => m.ExportAsync(It.IsAny<IRedditLink>()), Times.Never);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut();

            IDashboard dashboard = BuildDashboard();
            await sut.ExportAsync(dashboard);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardExportModelWithContent()
        {
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut();

            INews[] newsCollection = BuildNewsCollection().ToArray();
            IRedditAuthenticatedUser redditAuthenticatedUser = BuildRedditAuthenticatedUser();
            IRedditSubreddit[] redditSubredditCollection = BuildRedditSubredditCollection().ToArray();
            IRedditLink[] redditLinkCollection = BuildRedditLinkCollection().ToArray();
            IDashboard dashboard = BuildDashboard(newsCollection: newsCollection, redditAuthenticatedUser: redditAuthenticatedUser, redditSubredditCollection: redditSubredditCollection, redditLinkCollection: redditLinkCollection);
            DashboardExportModel result = await sut.ExportAsync(dashboard);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(newsCollection.Length + 1 + redditSubredditCollection.Length + redditLinkCollection.Length, result.Items.Count);
        }

        [TestMethod]
        public async Task ExportAsync_WhenAggregateExceptionWasThrown_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            AggregateException exception = new AggregateException();
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut(exception: exception);

            INews[] newsCollection = new INews[1] {BuildNews()};
            IDashboard dashboard = BuildDashboard(newsCollection: newsCollection, hasRedditAuthenticatedUser: false, hasRedditSubreddits: false, hasRedditLinks: false);
            await sut.ExportAsync(dashboard);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<AggregateException>(value => value == exception)), Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenAggregateExceptionWasThrown_ReturnsDashboardExportModelWithoutContent()
        {
            AggregateException exception = new AggregateException();
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut(exception: exception);

            IDashboard dashboard = BuildDashboard();
            DashboardExportModel result = await sut.ExportAsync(dashboard);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(0, result.Items.Count);
        }

        [TestMethod]
        public async Task ExportAsync_WhenExceptionWasThrown_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            Exception exception = new Exception();
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut(exception: exception);

            INews[] newsCollection = new INews[1] {BuildNews()};
            IDashboard dashboard = BuildDashboard(newsCollection: newsCollection, hasRedditAuthenticatedUser: false, hasRedditSubreddits: false, hasRedditLinks: false);
            await sut.ExportAsync(dashboard);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(value => value == exception)), Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenExceptionWasThrown_ReturnsDashboardExportModelWithoutContent()
        {
            Exception exception = new Exception();
            IModelExporter<DashboardExportModel, IDashboard> sut = CreateSut(exception: exception);

            IDashboard dashboard = BuildDashboard();
            DashboardExportModel result = await sut.ExportAsync(dashboard);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(0, result.Items.Count);
        }

        private IModelExporter<DashboardExportModel, IDashboard> CreateSut(Exception exception = null)
        {
            if (exception == null)
            {
                _newsModelExporterMock.Setup(m => m.ExportAsync(It.IsAny<INews>()))
                    .Returns(Task.FromResult(BuildDashboardItemExportModel()));
            }
            else
            {
                _newsModelExporterMock.Setup(m => m.ExportAsync(It.IsAny<INews>()))
                    .Throws(exception);
            }

            if (exception == null)
            {
                _redditAuthenticatedUserModelExporterMock.Setup(m => m.ExportAsync(It.IsAny<IRedditAuthenticatedUser>()))
                    .Returns(Task.FromResult(BuildDashboardItemExportModel()));
            }
            else
            {
                _redditAuthenticatedUserModelExporterMock.Setup(m => m.ExportAsync(It.IsAny<IRedditAuthenticatedUser>()))
                    .Throws(exception);
            }

            if (exception == null)
            {
                _redditSubredditModelExporterMock.Setup(m => m.ExportAsync(It.IsAny<IRedditSubreddit>()))
                    .Returns(Task.FromResult(BuildDashboardItemExportModel()));
            }
            else
            {
                _redditSubredditModelExporterMock.Setup(m => m.ExportAsync(It.IsAny<IRedditSubreddit>()))
                    .Throws(exception);
            }

            if (exception == null)
            {
                _redditLinkModelExporterMock.Setup(m => m.ExportAsync(It.IsAny<IRedditLink>()))
                    .Returns(Task.FromResult(BuildDashboardItemExportModel()));
            }
            else
            {
                _redditLinkModelExporterMock.Setup(m => m.ExportAsync(It.IsAny<IRedditLink>()))
                    .Throws(exception);
            }

            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<AggregateException>()))
                .Returns(Task.CompletedTask);
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.CompletedTask);

            return new Web.Factories.DashboardModelExporter(
                new List<IModelExporter>
                {
                    _newsModelExporterMock.Object,
                    _redditAuthenticatedUserModelExporterMock.Object,
                    _redditSubredditModelExporterMock.Object,
                    _redditLinkModelExporterMock.Object
                }, 
                _exceptionHandlerMock.Object);
        }

        private IDashboard BuildDashboard(bool hasNews = true, IEnumerable<INews> newsCollection = null, bool hasRedditAuthenticatedUser = true, IRedditAuthenticatedUser redditAuthenticatedUser = null, bool hasRedditSubreddits = true, IEnumerable<IRedditSubreddit> redditSubredditCollection = null, bool hasRedditLinks = true, IEnumerable<IRedditLink> redditLinkCollection = null)
        {
            return BuildDashboardMock(hasNews, newsCollection, hasRedditAuthenticatedUser, redditAuthenticatedUser, hasRedditSubreddits, redditSubredditCollection, hasRedditLinks, redditLinkCollection).Object;
        }

        private Mock<IDashboard> BuildDashboardMock(bool hasNews = true, IEnumerable<INews> newsCollection = null, bool hasRedditAuthenticatedUser = true, IRedditAuthenticatedUser redditAuthenticatedUser = null, bool hasRedditSubreddits = true, IEnumerable<IRedditSubreddit> redditSubredditCollection = null, bool hasRedditLinks = true, IEnumerable<IRedditLink> redditLinkCollection = null)
        {
            Mock<IDashboard> dashboardMock = new Mock<IDashboard>();
            dashboardMock.Setup(m => m.News)
                .Returns(hasNews ? newsCollection ?? BuildNewsCollection() : null);
            dashboardMock.Setup(m => m.RedditAuthenticatedUser)
                .Returns(hasRedditAuthenticatedUser ? redditAuthenticatedUser ?? BuildRedditAuthenticatedUser() : null);
            dashboardMock.Setup(m => m.RedditSubreddits)
                .Returns(hasRedditSubreddits ? redditSubredditCollection ?? BuildRedditSubredditCollection() : null);
            dashboardMock.Setup(m => m.RedditLinks)
                .Returns(hasRedditLinks ? redditLinkCollection ?? BuildRedditLinkCollection() : null);
            return dashboardMock;
        }

        private IEnumerable<INews> BuildNewsCollection()
        {
            List<INews> newsCollection = new List<INews>(_random.Next(5, 15));
            while (newsCollection.Count < newsCollection.Capacity)
            {
                newsCollection.Add(BuildNews());
            }
            return newsCollection;
        }

        private INews BuildNews()
        {
            return BuildNewsMock().Object;
        }

        private Mock<INews> BuildNewsMock()
        {
            return new Mock<INews>();
        }

        private IRedditAuthenticatedUser BuildRedditAuthenticatedUser()
        {
            return BuildRedditAuthenticatedUserMock().Object;
        }

        private Mock<IRedditAuthenticatedUser> BuildRedditAuthenticatedUserMock()
        {
            return new Mock<IRedditAuthenticatedUser>();
        }

        private IEnumerable<IRedditSubreddit> BuildRedditSubredditCollection()
        {
            List<IRedditSubreddit> redditSubredditCollection = new List<IRedditSubreddit>(_random.Next(5, 15));
            while (redditSubredditCollection.Count < redditSubredditCollection.Capacity)
            {
                redditSubredditCollection.Add(BuildRedditSubreddit());
            }
            return redditSubredditCollection;
        }

        private IRedditSubreddit BuildRedditSubreddit()
        {
            return BuildRedditSubredditMock().Object;
        }

        private Mock<IRedditSubreddit> BuildRedditSubredditMock()
        {
            return new Mock<IRedditSubreddit>();
        }

        private IEnumerable<IRedditLink> BuildRedditLinkCollection()
        {
            List<IRedditLink> redditLinkCollection = new List<IRedditLink>(_random.Next(5, 15));
            while (redditLinkCollection.Count < redditLinkCollection.Capacity)
            {
                redditLinkCollection.Add(BuildRedditLink());
            }
            return redditLinkCollection;
        }

        private IRedditLink BuildRedditLink()
        {
            return BuildRedditLinkMock().Object;
        }

        private Mock<IRedditLink> BuildRedditLinkMock()
        {
            return new Mock<IRedditLink>();
        }

        private DashboardItemExportModel BuildDashboardItemExportModel()
        {
            return new DashboardItemExportModel(Guid.NewGuid().ToString("D"), DateTime.Now.AddSeconds(_random.Next(0, 3600) * -1), Guid.NewGuid().ToString());
        }
    }
}