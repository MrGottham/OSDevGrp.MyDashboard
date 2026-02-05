using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.NewsToInformationViewModelBuilder
{
    [TestClass]
    public class BuildAsyncTests
    {
        #region private variables

        private Mock<IHtmlHelper> _htmlHelperMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _htmlHelperMock = new Mock<IHtmlHelper>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        public async Task BuildAsync_WhenSystemErrorIsNull_ThrowsArgumentNullException()
        {
            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.BuildAsync(null));

            Assert.AreEqual("input", result.ParamName);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertIdentifierWasCalledOnNews()
        {
            Mock<INews> newsMock = CreateNewsMock();

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            await sut.BuildAsync(newsMock.Object);

            newsMock.Verify(m => m.Identifier, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertTimestampWasCalledOnNews()
        {
            Mock<INews> newsMock = CreateNewsMock();

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            await sut.BuildAsync(newsMock.Object);

            newsMock.Verify(m => m.Timestamp, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertMediaUrlWasCalledOnNews()
        {
            Mock<INews> newsMock = CreateNewsMock();

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            await sut.BuildAsync(newsMock.Object);

            newsMock.Verify(m => m.MediaUrl, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertInformationWasCalledOnNews()
        {
            Mock<INews> newsMock = CreateNewsMock();

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            await sut.BuildAsync(newsMock.Object);

            newsMock.Verify(m => m.Information, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertConvertWasCalledOnHtmlHelperWithInformation()
        {
            string information = Guid.NewGuid().ToString("D");
            INews news = CreateNews(information: information);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            await sut.BuildAsync(news);

            _htmlHelperMock.Verify(m => m.Convert(It.Is<string>(value => value == information), It.Is<bool>(value => value == false), It.Is<bool>(value => value == true)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertExtractImagesWasCalledOnHtmlHelperWithInformation()
        {
            IList<Uri> imageUrlCollection = null;

            string information = Guid.NewGuid().ToString("D");
            INews news = CreateNews(information: information);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            await sut.BuildAsync(news);

            _htmlHelperMock.Verify(m => m.ExtractImages(It.Is<string>(value => value == $"HtmlHelper.Convert:{information}"), out imageUrlCollection), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertDetailsWasCalledOnNews()
        {
            Mock<INews> newsMock = CreateNewsMock();

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            await sut.BuildAsync(newsMock.Object);

            newsMock.Verify(m => m.Details, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertConvertWasCalledOnHtmlHelperWithDetails()
        {
            string details = Guid.NewGuid().ToString("D");
            INews news = CreateNews(details: details);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            await sut.BuildAsync(news);

            _htmlHelperMock.Verify(m => m.Convert(It.Is<string>(value => value == details), It.Is<bool>(value => value == false), It.Is<bool>(value => value == true)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertExtractImagesWasCalledOnHtmlHelperWithDetails()
        {
            IList<Uri> imageUrlCollection = null;

            string details = Guid.NewGuid().ToString("D");
            INews news = CreateNews(details: details);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            await sut.BuildAsync(news);

            _htmlHelperMock.Verify(m => m.ExtractImages(It.Is<string>(value => value == $"HtmlHelper.Convert:{details}"), out imageUrlCollection), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertProviderWasCalledOnNews()
        {
            Mock<INews> newsMock = CreateNewsMock();

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            await sut.BuildAsync(newsMock.Object);

            newsMock.Verify(m => m.Provider, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertNameWasCalledOnProvider()
        {
            Mock<INewsProvider> provider = CreateNewsProviderMock();
            INews news = CreateNews(provider: provider.Object);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            await sut.BuildAsync(news);

            provider.Verify(m => m.Name, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertConvertNewLinesWasCalledOnHtmlHelperWithNameOfProvider()
        {
            string providerName = Guid.NewGuid().ToString("D");
            INewsProvider provider = CreateNewsProvider(providerName);
            INews news = CreateNews(provider: provider);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            await sut.BuildAsync(news);

            _htmlHelperMock.Verify(m => m.ConvertNewLines(It.Is<string>(value => value == providerName)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertLinkWasCalledOnNews()
        {
            Mock<INews> newsMock = CreateNewsMock();

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            await sut.BuildAsync(newsMock.Object);

            newsMock.Verify(m => m.Link, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertAuthorWasCalledOnNews()
        {
            Mock<INews> newsMock = CreateNewsMock();

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            await sut.BuildAsync(newsMock.Object);

            newsMock.Verify(m => m.Author, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereNewsHasAuthor_AssertNameWasCalledOnAuthor()
        {
            Mock<IAuthor> authorMock = CreateAuthorMock();
            INews news = CreateNews(author: authorMock.Object);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            await sut.BuildAsync(news);

            authorMock.Verify(m => m.Name, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereNewsHasAuthor_AssertConvertNewLinesWasCalledOnHtmlHelperWithNameOfAuthor()
        {
            string authorName = Guid.NewGuid().ToString("D");
            IAuthor author = CreateAuthor(authorName);
            INews news = CreateNews(author: author);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            await sut.BuildAsync(news);

            _htmlHelperMock.Verify(m => m.ConvertNewLines(It.Is<string>(value => value == authorName)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel()
        {
            bool hasLink = _random.Next(0, 100) > 50;
            bool hasAuthor = _random.Next(0, 100) > 50;
            bool hasMediaUrl = _random.Next(0, 100) > 50;
            bool hasExtractedImage = _random.Next(0, 100) > 50;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor, hasMediaUrl, hasExtractedImage);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereNewsHasLink_ReturnsInitializedInformationViewModel()
        {
            const bool hasLink = true;
            bool hasAuthor = _random.Next(0, 100) > 50;
            bool hasMediaUrl = _random.Next(0, 100) > 50;
            bool hasExtractedImage = _random.Next(0, 100) > 50;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor, hasMediaUrl, hasExtractedImage);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereNewsHasNoLink_ReturnsInitializedInformationViewModel()
        {
            const bool hasLink = false;
            bool hasAuthor = _random.Next(0, 100) > 50;
            bool hasMediaUrl = _random.Next(0, 100) > 50;
            bool hasExtractedImage = _random.Next(0, 100) > 50;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor, hasMediaUrl, hasExtractedImage);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereNewsHasAuthor_ReturnsInitializedInformationViewModel()
        {
            bool hasLink = _random.Next(0, 100) > 50;
            const bool hasAuthor = true;
            bool hasMediaUrl = _random.Next(0, 100) > 50;
            bool hasExtractedImage = _random.Next(0, 100) > 50;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor, hasMediaUrl, hasExtractedImage);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereNewsHasNoAuthor_ReturnsInitializedInformationViewModel()
        {
            bool hasLink = _random.Next(0, 100) > 50;
            const bool hasAuthor = false;
            bool hasMediaUrl = _random.Next(0, 100) > 50;
            bool hasExtractedImage = _random.Next(0, 100) > 50;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor, hasMediaUrl, hasExtractedImage);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereNewsHasMediaUrlAndIncludesExtractedImage_ReturnsInitializedInformationViewModel()
        {
            bool hasLink = _random.Next(0, 100) > 50;
            bool hasAuthor = _random.Next(0, 100) > 50;
            const bool hasMediaUrl = true;
            const bool hasExtractedImage = true;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink,  hasAuthor, hasMediaUrl, hasExtractedImage);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereNewsHasMediaUrlAndDoesNotIncludeExtractedImage_ReturnsInitializedInformationViewModel()
        {
            bool hasLink = _random.Next(0, 100) > 50;
            bool hasAuthor = _random.Next(0, 100) > 50;
            const bool hasMediaUrl = true;
            const bool hasExtractedImage = false;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor, hasMediaUrl, hasExtractedImage);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereNewsDoesNotHaveMediaUrlButIncludesExtractedImage_ReturnsInitializedInformationViewModel()
        {
            bool hasLink = _random.Next(0, 100) > 50;
            bool hasAuthor = _random.Next(0, 100) > 50;
            const bool hasMediaUrl = false;
            const bool hasExtractedImage = true;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink,  hasAuthor, hasMediaUrl, hasExtractedImage);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereNewsDoesNotHaveMediaUrlAndDoesNotIncludeExtractedImage_ReturnsInitializedInformationViewModel()
        {
            bool hasLink = _random.Next(0, 100) > 50;
            bool hasAuthor = _random.Next(0, 100) > 50;
            const bool hasMediaUrl = false;
            const bool hasExtractedImage = false;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor, hasMediaUrl, hasExtractedImage);
        }

        private async Task BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(bool hasLink, bool hasAuthor, bool hasMediaUrl, bool hasExtractedImage)
        {
            string identifier = Guid.NewGuid().ToString("D");
            DateTime timestamp = DateTime.Now.AddTicks(_random.Next(-5000, 5000));
            string information = Guid.NewGuid().ToString("D");
            string details = Guid.NewGuid().ToString("D");
            string providerName = Guid.NewGuid().ToString("D");
            INewsProvider provider = CreateNewsProvider(providerName);
            Uri link = hasLink ? new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}") : null;
            string authorName = hasAuthor ? null : Guid.NewGuid().ToString("D");
            Uri mediaUrl = hasMediaUrl ? new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}/media.png") : null;
            INews news = CreateNews(
                identifier, 
                timestamp,
                information,
                details,
                provider,
                link,
                string.IsNullOrWhiteSpace(authorName) == false ? CreateAuthor(authorName) : null,
                mediaUrl);

            Uri extractedImageUrl = null;
            if (hasExtractedImage)
            {
                extractedImageUrl = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");
            }
            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut(extractedImageUrl: extractedImageUrl);

            InformationViewModel result = await sut.BuildAsync(news);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.InformationIdentifier);
            Assert.AreEqual(identifier, result.InformationIdentifier);
            Assert.AreEqual(timestamp, result.Timestamp);
            Assert.IsNotNull(result.Header);
            Assert.AreEqual($"HtmlHelper.ExtractImages:HtmlHelper.Convert:{information}", result.Header);
            Assert.IsNull(result.Summary);
            Assert.IsNotNull(result.Details);
            Assert.AreEqual($"HtmlHelper.ExtractImages:HtmlHelper.Convert:{details}", result.Details);
            Assert.IsNotNull(result.Provider);
            Assert.AreEqual($"HtmlHelper.ConvertNewLines:{providerName}", result.Provider);
            if (string.IsNullOrWhiteSpace(authorName) == false)
            {
                Assert.IsNotNull(result.Author);
                Assert.AreEqual($"HtmlHelper.ConvertNewLines:{authorName}", result.Author);
            }
            else
            {
                Assert.IsNull(result.Author);
            }
            if (link != null)
            {
                Assert.IsNotNull(result.ExternalUrl);
                Assert.AreEqual(link.AbsoluteUri, result.ExternalUrl);
            }
            else
            {
                Assert.IsNotNull(result.ExternalUrl);
                Assert.AreEqual("#", result.ExternalUrl);
            }
            if (mediaUrl != null)
            {
                Assert.IsNotNull(result.ImageUrl);
                Assert.AreEqual(mediaUrl.AbsoluteUri, result.ImageUrl);
            }
            else if (extractedImageUrl != null)
            {
                Assert.IsNotNull(result.ImageUrl);
                Assert.AreEqual(extractedImageUrl.AbsoluteUri, result.ImageUrl);
            }
            else
            {
                Assert.IsNull(result.ImageUrl);
            }
        }

        private IViewModelBuilder<InformationViewModel, INews> CreateSut(Uri extractedImageUrl = null)
        {
            IList<Uri> imageUrlCollection = new List<Uri>();
            if (extractedImageUrl != null)
            {
                imageUrlCollection.Add(extractedImageUrl);
            }

            _htmlHelperMock.Setup(m => m.ConvertNewLines(It.IsAny<string>()))
                .Returns<string>(value => $"HtmlHelper.ConvertNewLines:{value}");
            _htmlHelperMock.Setup(m => m.Convert(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns<string, bool, bool>((value, convertNewLines, removeEndingComment) => $"HtmlHelper.Convert:{value}");
            _htmlHelperMock.Setup(m => m.ExtractImages(It.IsAny<string>(), out imageUrlCollection))
                .Returns<string, IList<Uri>>((value, urlCollection) => $"HtmlHelper.ExtractImages:{value}");

            return new Web.Factories.NewsToInformationViewModelBuilder(_htmlHelperMock.Object);
        }

        private INews CreateNews(string identifier = null, DateTime? timestamp = null, string information = null, string details = null, INewsProvider provider = null, Uri link = null, IAuthor author = null, Uri mediaUrl = null)
        {
            return CreateNewsMock(identifier, timestamp, information, details, provider, link, author, mediaUrl).Object;
        }

        private Mock<INews> CreateNewsMock(string identifier = null, DateTime? timestamp = null, string information = null, string details = null, INewsProvider provider = null, Uri link = null, IAuthor author = null, Uri mediaUrl = null)
        {
            Mock<INews> newsMock = new Mock<INews>();
            newsMock.Setup(m => m.Identifier)
                .Returns(identifier);
            newsMock.Setup(m => m.Timestamp)
                .Returns(timestamp ?? DateTime.Now);
            newsMock.Setup(m => m.Information)
                .Returns(information);
            newsMock.Setup(m => m.Details)
                .Returns(details);
            newsMock.Setup(m => m.Provider)
                .Returns(provider ?? CreateNewsProvider());
            newsMock.Setup(m => m.Link)
                .Returns(link);
            newsMock.Setup(m => m.Author)
                .Returns(author);
            newsMock.Setup(m => m.MediaUrl)
                .Returns(mediaUrl);
            return newsMock;
        }

        private INewsProvider CreateNewsProvider(string name = null, Uri uri = null)
        {
            return CreateNewsProviderMock(name, uri).Object;
        }

        private Mock<INewsProvider> CreateNewsProviderMock(string name = null, Uri uri = null)
        {
            Mock<INewsProvider> newsProviderMock = new Mock<INewsProvider>();
            newsProviderMock.Setup(m => m.Name)
                .Returns(name);
            newsProviderMock.Setup(m => m.Uri)
                .Returns(uri);
            return newsProviderMock;
        }

        private IAuthor CreateAuthor(string name = null)
        {
            return CreateAuthorMock(name).Object;
        }

        private Mock<IAuthor> CreateAuthorMock(string name = null)
        {
            Mock<IAuthor> authorMock = new Mock<IAuthor>();
            authorMock.Setup(m => m.Name)
                .Returns(name);
            return authorMock;
        }
    } 
}