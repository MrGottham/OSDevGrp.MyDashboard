using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.RedditSubredditModelExporter
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
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            await sut.ExportAsync(null);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertFullNameWasCalledOnRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            Mock<IRedditSubreddit> redditSubredditMock = BuildRedditSubredditMock();
            await sut.ExportAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.FullName, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertCreatedTimeWasCalledOnRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            Mock<IRedditSubreddit> redditSubredditMock = BuildRedditSubredditMock();
            await sut.ExportAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.CreatedTime, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertTitleWasCalledOnRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            Mock<IRedditSubreddit> redditSubredditMock = BuildRedditSubredditMock();
            await sut.ExportAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.Title, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertDescriptionAsTextWasCalledOnRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            Mock<IRedditSubreddit> redditSubredditMock = BuildRedditSubredditMock();
            await sut.ExportAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.DescriptionAsText, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditSubredditIsWithoutDescriptionAsText_AssertPublicDescriptionAsTextWasCalledOnRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            Mock<IRedditSubreddit> redditSubredditMock = BuildRedditSubredditMock();
            await sut.ExportAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.PublicDescriptionAsText, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditSubredditIsWithDescriptionAsText_AssertPublicDescriptionAsTextWasNotCalledOnRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            string descriptionAsText = Guid.NewGuid().ToString("D");
            Mock<IRedditSubreddit> redditSubredditMock = BuildRedditSubredditMock(descriptionAsText: descriptionAsText);
            await sut.ExportAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.PublicDescriptionAsText, Times.Never);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertUrlWasCalledOnRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            Mock<IRedditSubreddit> redditSubredditMock = BuildRedditSubredditMock();
            await sut.ExportAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.Url, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertBannerImageUrlWasCalledOnRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            Mock<IRedditSubreddit> redditSubredditMock = BuildRedditSubredditMock();
            await sut.ExportAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.BannerImageUrl, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditSubredditIsWithoutBannerImageUrl_AssertHeaderImageUrllWasCalledOnRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            Mock<IRedditSubreddit> redditSubredditMock = BuildRedditSubredditMock();
            await sut.ExportAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.HeaderImageUrl, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditSubredditIsWithBannerImageUrl_AssertHeaderImageUrllWasNotCalledOnRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            Uri bannerImageUrl = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}/{Guid.NewGuid().ToString("D")}");
            Mock<IRedditSubreddit> redditSubredditMock = BuildRedditSubredditMock(bannerImageUrl: bannerImageUrl);
            await sut.ExportAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.HeaderImageUrl, Times.Never);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithIdentifierFromRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            string fullName = Guid.NewGuid().ToString("D");
            IRedditSubreddit redditSubreddit = BuildRedditSubreddit(fullName);
            DashboardItemExportModel result = await sut.ExportAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.AreEqual(fullName, result.Identifier);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithTimestampFromRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            DateTime createdTime = DateTime.Now.AddSeconds(_random.Next(300, 3600) * -1);
            IRedditSubreddit redditSubreddit = BuildRedditSubreddit(createdTime: createdTime);
            DashboardItemExportModel result = await sut.ExportAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdTime, result.Timestamp);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithInformationFromRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            string title = Guid.NewGuid().ToString("D");
            IRedditSubreddit redditSubreddit = BuildRedditSubreddit(title: title);
            DashboardItemExportModel result = await sut.ExportAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.AreEqual(title, result.Information);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditSubredditIsWithoutDescriptionAsTextAndWithoutPublicDescriptionAsText_ReturnsDashboardItemExportModelWithoutDetails()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            string title = Guid.NewGuid().ToString("D");
            IRedditSubreddit redditSubreddit = BuildRedditSubreddit(title: title);
            DashboardItemExportModel result = await sut.ExportAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Details);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditSubredditIsWithoutDescriptionAsTextAndWithPublicDescriptionAsText_ReturnsDashboardItemExportModelWithDetailsFromRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            string publicDescriptionAsText = Guid.NewGuid().ToString("D");
            IRedditSubreddit redditSubreddit = BuildRedditSubreddit(publicDescriptionAsText: publicDescriptionAsText);
            DashboardItemExportModel result = await sut.ExportAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.AreEqual(publicDescriptionAsText, result.Details);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditSubredditIsWithDescriptionAsText_ReturnsDashboardItemExportModelWithDetailsFromRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            string descriptionAsText = Guid.NewGuid().ToString("D");
            IRedditSubreddit redditSubreddit = BuildRedditSubreddit(descriptionAsText: descriptionAsText);
            DashboardItemExportModel result = await sut.ExportAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.AreEqual(descriptionAsText, result.Details);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithProvider()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            IRedditSubreddit redditSubreddit = BuildRedditSubreddit();
            DashboardItemExportModel result = await sut.ExportAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.AreEqual("Reddit", result.Provider);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithoutAuthor()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            IRedditSubreddit redditSubreddit = BuildRedditSubreddit();
            DashboardItemExportModel result = await sut.ExportAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Author);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditSubredditIsWithoutUrl_ReturnsDashboardItemExportModelWithoutSourceUrl()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            IRedditSubreddit redditSubreddit = BuildRedditSubreddit();
            DashboardItemExportModel result = await sut.ExportAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.IsNull(result.SourceUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditSubredditIsWithUrl_ReturnsDashboardItemExportModelWithSourceUrlFromRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            Uri url = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}/{Guid.NewGuid().ToString("D")}");
            IRedditSubreddit redditSubreddit = BuildRedditSubreddit(url: url);
            DashboardItemExportModel result = await sut.ExportAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.AreEqual(url.AbsoluteUri, result.SourceUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditSubredditIsWithoutBannerImageUrlAndWithoutHeaderImageUrl_ReturnsDashboardItemExportModelWithoutImageUrl()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            IRedditSubreddit redditSubreddit = BuildRedditSubreddit();
            DashboardItemExportModel result = await sut.ExportAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.IsNull(result.ImageUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditSubredditIsWithoutBannerImageUrlAndWithHeaderImageUrl_ReturnsDashboardItemExportModelWithImageUrlFromRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            Uri headerImageUrl = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}/{Guid.NewGuid().ToString("D")}");
            IRedditSubreddit redditSubreddit = BuildRedditSubreddit(headerImageUrl: headerImageUrl);
            DashboardItemExportModel result = await sut.ExportAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.AreEqual(headerImageUrl.AbsoluteUri, result.ImageUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenRedditSubredditIsWithBannerImageUrl_ReturnsDashboardItemExportModelWithImageUrlFromRedditSubreddit()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            Uri bannerImageUrl = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}/{Guid.NewGuid().ToString("D")}");
            IRedditSubreddit redditSubreddit = BuildRedditSubreddit(bannerImageUrl: bannerImageUrl);
            DashboardItemExportModel result = await sut.ExportAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.AreEqual(bannerImageUrl.AbsoluteUri, result.ImageUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IModelExporter<DashboardItemExportModel, IRedditSubreddit> sut = CreateSut();

            IRedditSubreddit redditSubreddit = BuildRedditSubreddit();
            await sut.ExportAsync(redditSubreddit);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        private IModelExporter<DashboardItemExportModel, IRedditSubreddit> CreateSut()
        {
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<AggregateException>()))
                .Returns(Task.CompletedTask);
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.CompletedTask);

            return new Web.Factories.RedditSubredditModelExporter(_exceptionHandlerMock.Object);
        }

        private IRedditSubreddit BuildRedditSubreddit(string fullName = null, DateTime? createdTime = null, string title = null, string descriptionAsText = null, string publicDescriptionAsText = null, Uri url = null, Uri bannerImageUrl = null, Uri headerImageUrl = null)
        {
            return BuildRedditSubredditMock(fullName, createdTime, title, descriptionAsText, publicDescriptionAsText, url, bannerImageUrl, headerImageUrl).Object;
        }

        private Mock<IRedditSubreddit> BuildRedditSubredditMock(string fullName = null, DateTime? createdTime = null, string title = null, string descriptionAsText = null, string publicDescriptionAsText = null, Uri url = null, Uri bannerImageUrl = null, Uri headerImageUrl = null)
        {
            Mock<IRedditSubreddit> redditSubredditMock = new Mock<IRedditSubreddit>();
            redditSubredditMock.Setup(m => m.FullName)
                .Returns(fullName ?? Guid.NewGuid().ToString("D"));
            redditSubredditMock.Setup(m => m.CreatedTime)
                .Returns(createdTime ?? DateTime.Now.AddSeconds(_random.Next(300, 3600) * -1));
            redditSubredditMock.Setup(m => m.Title)
                .Returns(title ?? Guid.NewGuid().ToString("D"));
            redditSubredditMock.Setup(m => m.DescriptionAsText)
                .Returns(descriptionAsText);
            redditSubredditMock.Setup(m => m.PublicDescriptionAsText)
                .Returns(publicDescriptionAsText);
            redditSubredditMock.Setup(m => m.Url)
                .Returns(url);
            redditSubredditMock.Setup(m => m.BannerImageUrl)
                .Returns(bannerImageUrl);
            redditSubredditMock.Setup(m => m.HeaderImageUrl)
                .Returns(headerImageUrl);
            return redditSubredditMock;
        }
    }
}