using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;
using System;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.NewsModelExporter
{
    [TestClass]
    public class ExportAsyncTests
    {
        #region Private variables

        private Mock<IExceptionHandler> _exceptionHandlerMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullException("input")]
        public async Task ExportAsync_WhenInputIsNull_ThrowsArgumentNullException()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            await sut.ExportAsync(null);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertIdentifierWasCalledOnNews()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            Mock<INews> newsMock = BuildNewsMock();
            await sut.ExportAsync(newsMock.Object);

            newsMock.Verify(m => m.Identifier, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertTimestampWasCalledOnNews()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            Mock<INews> newsMock = BuildNewsMock();
            await sut.ExportAsync(newsMock.Object);

            newsMock.Verify(m => m.Timestamp, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertInformationWasCalledOnNews()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            Mock<INews> newsMock = BuildNewsMock();
            await sut.ExportAsync(newsMock.Object);

            newsMock.Verify(m => m.Information, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertDetailsWasCalledOnNews()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            Mock<INews> newsMock = BuildNewsMock();
            await sut.ExportAsync(newsMock.Object);

            newsMock.Verify(m => m.Details, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertProviderWasCalledOnNews()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            Mock<INews> newsMock = BuildNewsMock();
            await sut.ExportAsync(newsMock.Object);

            newsMock.Verify(m => m.Provider, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenNewsIsWithoutProvider_AssertNameWasNotCalledOnProvider()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            Mock<INewsProvider> newsProviderMock = BuildNewsProviderMock();
            INews news = BuildNews();
            await sut.ExportAsync(news);

            newsProviderMock.Verify(m => m.Name, Times.Never);
        }

        [TestMethod]
        public async Task ExportAsync_WhenNewsIsWithProvider_AssertNameWasCalledOnProvider()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            Mock<INewsProvider> newsProviderMock = BuildNewsProviderMock();
            INews news = BuildNews(newsProvider: newsProviderMock.Object);
            await sut.ExportAsync(news);

            newsProviderMock.Verify(m => m.Name, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertLinkWasCalledOnNews()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            Mock<INews> newsMock = BuildNewsMock();
            await sut.ExportAsync(newsMock.Object);

            newsMock.Verify(m => m.Link, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertMediaUrlWasCalledOnNews()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            Mock<INews> newsMock = BuildNewsMock();
            await sut.ExportAsync(newsMock.Object);

            newsMock.Verify(m => m.MediaUrl, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertAuthorWasCalledOnNews()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            Mock<INews> newsMock = BuildNewsMock();
            await sut.ExportAsync(newsMock.Object);

            newsMock.Verify(m => m.Author, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenNewsIsWithoutAuthor_AssertNameWasNotCalledOnAuthor()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            Mock<IAuthor> authorMock = BuildAuthorMock();
            INews news = BuildNews();
            await sut.ExportAsync(news);

            authorMock.Verify(m => m.Name, Times.Never);
        }

        [TestMethod]
        public async Task ExportAsync_WhenNewsIsWithAuthor_AssertNameWasCalledOnAuthor()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            Mock<IAuthor> authorMock = BuildAuthorMock();
            INews news = BuildNews(author: authorMock.Object);
            await sut.ExportAsync(news);

            authorMock.Verify(m => m.Name, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithIdentifierFromNews()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            string identifier = Guid.NewGuid().ToString("D");
            INews news = BuildNews(identifier);
            DashboardItemExportModel result = await sut.ExportAsync(news);

            Assert.IsNotNull(result);
            Assert.AreEqual(identifier, result.Identifier);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithTimestampFromNews()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            DateTime timestamp = DateTime.Now.AddSeconds(_random.Next(300, 3600) * -1);
            INews news = BuildNews(timestamp: timestamp);
            DashboardItemExportModel result = await sut.ExportAsync(news);

            Assert.IsNotNull(result);
            Assert.AreEqual(timestamp, result.Timestamp);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithInformationFromNews()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            string information = Guid.NewGuid().ToString("D");
            INews news = BuildNews(information: information);
            DashboardItemExportModel result = await sut.ExportAsync(news);

            Assert.IsNotNull(result);
            Assert.AreEqual(information, result.Information);
        }

        [TestMethod]
        public async Task ExportAsync_WhenNewsIsWithoutDetails_ReturnsDashboardItemExportModelWithoutDetails()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            INews news = BuildNews();
            DashboardItemExportModel result = await sut.ExportAsync(news);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Details);
        }

        [TestMethod]
        public async Task ExportAsync_WhenNewsIsWithDetails_ReturnsDashboardItemExportModelWithDetailsFromNews()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            string details = Guid.NewGuid().ToString("D");
            INews news = BuildNews(details: details);
            DashboardItemExportModel result = await sut.ExportAsync(news);

            Assert.IsNotNull(result);
            Assert.AreEqual(details, result.Details);
        }

        [TestMethod]
        public async Task ExportAsync_WhenNewsIsWithoutProvider_ReturnsDashboardItemExportModelWithoutProvider()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            INews news = BuildNews();
            DashboardItemExportModel result = await sut.ExportAsync(news);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Provider);
        }

        [TestMethod]
        public async Task ExportAsync_WhenNewsIsWithProvider_ReturnsDashboardItemExportModelWithProviderFromNews()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            string name = Guid.NewGuid().ToString("D");
            INewsProvider newsProvider = BuildNewsProvider(name);
            INews news = BuildNews(newsProvider: newsProvider);
            DashboardItemExportModel result = await sut.ExportAsync(news);

            Assert.IsNotNull(result);
            Assert.AreEqual(name, result.Provider);
        }

        [TestMethod]
        public async Task ExportAsync_WhenNewsIsWithoutLink_ReturnsDashboardItemExportModelWithoutSourceUrl()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            INews news = BuildNews();
            DashboardItemExportModel result = await sut.ExportAsync(news);

            Assert.IsNotNull(result);
            Assert.IsNull(result.SourceUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenNewsIsWithLink_ReturnsDashboardItemExportModelWithSourceUrlFromNews()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            Uri link = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}/{Guid.NewGuid().ToString("D")}");
            INews news = BuildNews(link: link);
            DashboardItemExportModel result = await sut.ExportAsync(news);

            Assert.IsNotNull(result);
            Assert.AreEqual(link.AbsoluteUri, result.SourceUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenNewsIsWithoutMediaUrl_ReturnsDashboardItemExportModelWithoutImageUrl()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            INews news = BuildNews();
            DashboardItemExportModel result = await sut.ExportAsync(news);

            Assert.IsNotNull(result);
            Assert.IsNull(result.ImageUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenNewsIsWithMediaUrl_ReturnsDashboardItemExportModelWithImageUrlFromNews()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            Uri mediaUrl = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}/{Guid.NewGuid().ToString("D")}.png");
            INews news = BuildNews(mediaUrl: mediaUrl);
            DashboardItemExportModel result = await sut.ExportAsync(news);

            Assert.IsNotNull(result);
            Assert.AreEqual(mediaUrl.AbsoluteUri, result.ImageUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenNewsIsWithoutAuthor_ReturnsDashboardItemExportModelWithoutAuthor()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            INews news = BuildNews();
            DashboardItemExportModel result = await sut.ExportAsync(news);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Author);
        }

        [TestMethod]
        public async Task ExportAsync_WhenNewsIsWithAuthor_ReturnsDashboardItemExportModelWithAuthorFromNews()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            string name = Guid.NewGuid().ToString("D");
            IAuthor author = BuildAuthor(name);
            INews news = BuildNews(author: author);
            DashboardItemExportModel result = await sut.ExportAsync(news);

            Assert.IsNotNull(result);
            Assert.AreEqual(name, result.Author);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithoutImageUrl()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            INews news = BuildNews();
            DashboardItemExportModel result = await sut.ExportAsync(news);

            Assert.IsNotNull(result);
            Assert.IsNull(result.ImageUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IModelExporter<DashboardItemExportModel, INews> sut = CreateSut();

            INews news = BuildNews();
            await sut.ExportAsync(news);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        private IModelExporter<DashboardItemExportModel, INews> CreateSut()
        {
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<AggregateException>()))
                .Returns(Task.CompletedTask);
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.CompletedTask);

            return new Web.Factories.NewsModelExporter(_exceptionHandlerMock.Object);
        }

        private INews BuildNews(string identifier = null, DateTime? timestamp = null, string information = null, string details = null, INewsProvider newsProvider = null, Uri link = null, IAuthor author = null, Uri mediaUrl = null)
        {
            return BuildNewsMock(identifier, timestamp, information, details, newsProvider, link, author, mediaUrl).Object;
        }

        private Mock<INews> BuildNewsMock(string identifier = null, DateTime? timestamp = null, string information = null, string details = null, INewsProvider newsProvider = null, Uri link = null, IAuthor author = null, Uri mediaUrl = null)
        {
            Mock<INews> newsMock = new Mock<INews>();
            newsMock.Setup(m => m.Identifier)
                .Returns(identifier ?? Guid.NewGuid().ToString("D"));
            newsMock.Setup(m => m.Timestamp)
                .Returns(timestamp ?? DateTime.Now.AddSeconds(_random.Next(10, 300) * -1));
            newsMock.Setup(m => m.Information)
                .Returns(information ?? Guid.NewGuid().ToString("D"));
            newsMock.Setup(m => m.Details)
                .Returns(details);
            newsMock.Setup(m => m.Provider)
                .Returns(newsProvider);
            newsMock.Setup(m => m.Link)
                .Returns(link);
            newsMock.Setup(m => m.Author)
                .Returns(author);
            newsMock.Setup(m => m.MediaUrl)
                .Returns(mediaUrl);
            return newsMock;
        }

        private INewsProvider BuildNewsProvider(string name = null)
        {
            return BuildNewsProviderMock(name).Object;
        }

        private Mock<INewsProvider> BuildNewsProviderMock(string name = null)
        {
            Mock<INewsProvider> newsProviderMock = new Mock<INewsProvider>();
            newsProviderMock.Setup(m => m.Name)
                .Returns(name ?? Guid.NewGuid().ToString("D"));
            return newsProviderMock;
        }

        private IAuthor BuildAuthor(string name = null)
        {
            return BuildAuthorMock(name).Object;
        }

        private Mock<IAuthor> BuildAuthorMock(string name = null)
        {
            Mock<IAuthor> authorMock = new Mock<IAuthor>();
            authorMock.Setup(m => m.Name)
                .Returns(name ?? Guid.NewGuid().ToString("D"));
            return authorMock;
        }
    }
}