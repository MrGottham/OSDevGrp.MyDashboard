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

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.RedditLinkModelExporter
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
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            await sut.ExportAsync(null);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertFullNameWasCalledOnRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            Mock<IRedditLink> redditLinkMock = BuildRedditLinkMock();
            await sut.ExportAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.FullName, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertCreatedTimeWasCalledOnRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            Mock<IRedditLink> redditLinkMock = BuildRedditLinkMock();
            await sut.ExportAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.CreatedTime, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertTitleWasCalledOnRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            Mock<IRedditLink> redditLinkMock = BuildRedditLinkMock();
            await sut.ExportAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.Title, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertSelftextAsTextWasCalledOnRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            Mock<IRedditLink> redditLinkMock = BuildRedditLinkMock();
            await sut.ExportAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.SelftextAsText, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertSubredditWasCalledOnRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            Mock<IRedditLink> redditLinkMock = BuildRedditLinkMock();
            await sut.ExportAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.Subreddit, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithoutRedditSubreddit_AssertFullNameWasNotCalledOnRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            Mock<IRedditSubreddit> redditSubredditMock = BuildRedditSubredditMock();
            IRedditLink redditLink = BuildRedditLink();
            await sut.ExportAsync(redditLink);

            redditSubredditMock.Verify(m => m.FullName, Times.Never);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithoutRedditSubreddit_AssertFullNameWasCalledOnRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            Mock<IRedditSubreddit> redditSubredditMock = BuildRedditSubredditMock();
            IRedditLink redditLink = BuildRedditLink(redditSubreddit: redditSubredditMock.Object);
            await sut.ExportAsync(redditLink);

            redditSubredditMock.Verify(m => m.FullName, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertAuthorWasCalledOnRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            Mock<IRedditLink> redditLinkMock = BuildRedditLinkMock();
            await sut.ExportAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.Author, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertUrlWasCalledOnRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            Mock<IRedditLink> redditLinkMock = BuildRedditLinkMock();
            await sut.ExportAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.Url, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertImagesWasCalledOnRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            Mock<IRedditLink> redditLinkMock = BuildRedditLinkMock();
            await sut.ExportAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.Images, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithoutImages_AssertThumbnailUrlWasCalledOnRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            Mock<IRedditLink> redditLinkMock = BuildRedditLinkMock();
            await sut.ExportAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.ThumbnailUrl, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithImages_AssertThumbnailUrlWasNotCalledOnRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            IEnumerable<IRedditImage> redditImageCollection = new List<IRedditImage>
            {
                BuildRedditImage(new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}/{Guid.NewGuid().ToString("D")}"))
            };
            Mock<IRedditLink> redditLinkMock = BuildRedditLinkMock(redditImageCollection: redditImageCollection);
            await sut.ExportAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.ThumbnailUrl, Times.Never);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithImages_AssertUrlWasCalledOnFirstRedditImageInRedditImageCollection()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            Mock<IRedditImage>[] redditImageMockCollection = new Mock<IRedditImage>[]
            {
                BuildRedditImageMock(new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}/{Guid.NewGuid().ToString("D")}")),
                BuildRedditImageMock(new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}/{Guid.NewGuid().ToString("D")}")),
                BuildRedditImageMock(new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}/{Guid.NewGuid().ToString("D")}"))
            };
            Mock<IRedditLink> redditLinkMock = BuildRedditLinkMock(redditImageCollection: redditImageMockCollection.Select(redditLinkMock => redditLinkMock.Object).ToArray());
            await sut.ExportAsync(redditLinkMock.Object);

            redditImageMockCollection[0].Verify(m => m.Url, Times.Exactly(2));
            redditImageMockCollection[1].Verify(m => m.Url, Times.Never);
            redditImageMockCollection[2].Verify(m => m.Url, Times.Never);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithIdentifierFromRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            string fullName = Guid.NewGuid().ToString("D");
            IRedditLink redditLink = BuildRedditLink(fullName);
            DashboardItemExportModel result = await sut.ExportAsync(redditLink);

            Assert.IsNotNull(result);
            Assert.AreEqual(fullName, result.Identifier);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithTimestampFromRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            DateTime createdTime = DateTime.Now.AddSeconds(_random.Next(300, 3600) * -1);
            IRedditLink redditLink = BuildRedditLink(createdTime: createdTime);
            DashboardItemExportModel result = await sut.ExportAsync(redditLink);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdTime, result.Timestamp);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithInformationFromRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            string title = Guid.NewGuid().ToString("D");
            IRedditLink redditLink = BuildRedditLink(title: title);
            DashboardItemExportModel result = await sut.ExportAsync(redditLink);

            Assert.IsNotNull(result);
            Assert.AreEqual(title, result.Information);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithoutSelftextAsText_ReturnsDashboardItemExportModelWithoutDetails()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            IRedditLink redditLink = BuildRedditLink();
            DashboardItemExportModel result = await sut.ExportAsync(redditLink);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Details);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithSelftextAsText_ReturnsDashboardItemExportModelWithDetailsFromRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            string selftextAsText = Guid.NewGuid().ToString("D");
            IRedditLink redditLink = BuildRedditLink(selftextAsText: selftextAsText);
            DashboardItemExportModel result = await sut.ExportAsync(redditLink);

            Assert.IsNotNull(result);
            Assert.AreEqual(selftextAsText, result.Details);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithoutRedditSubreddit_ReturnsDashboardItemExportModelWithoutProvider()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            IRedditLink redditLink = BuildRedditLink();
            DashboardItemExportModel result = await sut.ExportAsync(redditLink);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Provider);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithRedditSubreddit_ReturnsDashboardItemExportModelWithProviderFromRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            string fullName = Guid.NewGuid().ToString("D");
            IRedditSubreddit redditSubreddit = BuildRedditSubreddit(fullName);
            IRedditLink redditLink = BuildRedditLink(redditSubreddit: redditSubreddit);
            DashboardItemExportModel result = await sut.ExportAsync(redditLink);

            Assert.IsNotNull(result);
            Assert.AreEqual(fullName, result.Provider);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithoutAuthor_ReturnsDashboardItemExportModelWithoutAuthor()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            IRedditLink redditLink = BuildRedditLink();
            DashboardItemExportModel result = await sut.ExportAsync(redditLink);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Author);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithAuthor_ReturnsDashboardItemExportModelWithAuthorFromRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            string author = Guid.NewGuid().ToString("D");
            IRedditLink redditLink = BuildRedditLink(author: author);
            DashboardItemExportModel result = await sut.ExportAsync(redditLink);

            Assert.IsNotNull(result);
            Assert.AreEqual(author, result.Author);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithoutUrl_ReturnsDashboardItemExportModelWithoutSourceUrl()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            IRedditLink redditLink = BuildRedditLink();
            DashboardItemExportModel result = await sut.ExportAsync(redditLink);

            Assert.IsNotNull(result);
            Assert.IsNull(result.SourceUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithUrl_ReturnsDashboardItemExportModelWithSourceUrlFromRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            Uri url = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}/{Guid.NewGuid().ToString("D")}");
            IRedditLink redditLink = BuildRedditLink(url: url);
            DashboardItemExportModel result = await sut.ExportAsync(redditLink);

            Assert.IsNotNull(result);
            Assert.AreEqual(url.AbsoluteUri, result.SourceUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithoutImagesAndWithoutThumbnailUrl_ReturnsDashboardItemExportModelWithoutImageUrl()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            IRedditLink redditLink = BuildRedditLink();
            DashboardItemExportModel result = await sut.ExportAsync(redditLink);

            Assert.IsNotNull(result);
            Assert.IsNull(result.ImageUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithoutImagesAndWithThumbnailUrl_ReturnsDashboardItemExportModelWithImageUrlFromRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            Uri thumbnailUrl = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}/{Guid.NewGuid().ToString("D")}");
            IRedditLink redditLink = BuildRedditLink(thumbnailUrl: thumbnailUrl);
            DashboardItemExportModel result = await sut.ExportAsync(redditLink);

            Assert.IsNotNull(result);
            Assert.AreEqual(thumbnailUrl.AbsoluteUri, result.ImageUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithEmptyImages_ReturnsDashboardItemExportModelWithoutImageUrl()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            IEnumerable<IRedditImage> redditImageCollection = new List<IRedditImage>();
            IRedditLink redditLink = BuildRedditLink(redditImageCollection: redditImageCollection);
            DashboardItemExportModel result = await sut.ExportAsync(redditLink);

            Assert.IsNotNull(result);
            Assert.IsNull(result.ImageUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditLinkIsWithNonEmptyImages_ReturnsDashboardItemExportModelWithImageUrlFromRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            Uri imageUrl = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}/{Guid.NewGuid().ToString("D")}");
            IEnumerable<IRedditImage> redditImageCollection = new List<IRedditImage>()
            {
                BuildRedditImage(imageUrl)
            };
            IRedditLink redditLink = BuildRedditLink(redditImageCollection: redditImageCollection);
            DashboardItemExportModel result = await sut.ExportAsync(redditLink);

            Assert.IsNotNull(result);
            Assert.AreEqual(imageUrl.AbsoluteUri, result.ImageUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IModelExporter<DashboardItemExportModel, IRedditLink> sut = CreateSut();

            IRedditLink redditLink = BuildRedditLink();
            await sut.ExportAsync(redditLink);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        private IModelExporter<DashboardItemExportModel, IRedditLink> CreateSut()
        {
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<AggregateException>()))
                .Returns(Task.CompletedTask);
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.CompletedTask);

            return new Web.Factories.RedditLinkModelExporter(_exceptionHandlerMock.Object);
        }

        private IRedditLink BuildRedditLink(string fullName = null, DateTime? createdTime = null, string title = null, string selftextAsText = null, IRedditSubreddit redditSubreddit = null, string author = null, Uri url = null, IEnumerable<IRedditImage> redditImageCollection = null, Uri thumbnailUrl = null)
        {
            return BuildRedditLinkMock(fullName, createdTime, title, selftextAsText, redditSubreddit, author, url, redditImageCollection, thumbnailUrl).Object;
        }

        private Mock<IRedditLink> BuildRedditLinkMock(string fullName = null, DateTime? createdTime = null, string title = null, string selftextAsText = null, IRedditSubreddit redditSubreddit = null, string author = null, Uri url = null, IEnumerable<IRedditImage> redditImageCollection = null, Uri thumbnailUrl = null)
        {
            Mock<IRedditLink> redditLinkMock = new Mock<IRedditLink>();
            redditLinkMock.Setup(m => m.FullName)
                .Returns(fullName ?? Guid.NewGuid().ToString("D"));
            redditLinkMock.Setup(m => m.CreatedTime)
                .Returns(createdTime ?? DateTime.Now.AddSeconds(_random.Next(300, 3600) * -1));
            redditLinkMock.Setup(m => m.Title)
                .Returns(title ?? Guid.NewGuid().ToString("D"));
            redditLinkMock.Setup(m => m.SelftextAsText)
                .Returns(selftextAsText);
            redditLinkMock.Setup(m => m.Subreddit)
                .Returns(redditSubreddit);
            redditLinkMock.Setup(m => m.Author)
                .Returns(author);
            redditLinkMock.Setup(m => m.Url)
                .Returns(url);
            redditLinkMock.Setup(m => m.Images)
                .Returns(redditImageCollection);
            redditLinkMock.Setup(m => m.ThumbnailUrl)
                .Returns(thumbnailUrl);
            return redditLinkMock;
        }

        private IRedditSubreddit BuildRedditSubreddit(string fullName = null)
        {
            return BuildRedditSubredditMock(fullName).Object;
        }

        private Mock<IRedditSubreddit> BuildRedditSubredditMock(string fullName = null)
        {
            Mock<IRedditSubreddit> redditSubredditMock = new Mock<IRedditSubreddit>();
            redditSubredditMock.Setup(m => m.FullName)
                .Returns(fullName ?? Guid.NewGuid().ToString("D"));
            return redditSubredditMock;
        }

        private IRedditImage BuildRedditImage(Uri imageUrl = null)
        {
            return BuildRedditImageMock(imageUrl).Object;
        }

        private Mock<IRedditImage> BuildRedditImageMock(Uri imageUrl = null)
        {
            Mock<IRedditImage> redditImageMock = new Mock<IRedditImage>();
            redditImageMock.Setup(m => m.Url)
                .Returns(imageUrl);
            return redditImageMock;
        }
    }
}