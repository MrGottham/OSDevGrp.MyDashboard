using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;

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
        [ExpectedArgumentNullExceptionAttribute("input")]
        public void BuildAsync_WhenSystemErrorIsNull_ThrowsArgumentNullException()
        {
            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            sut.BuildAsync(null);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertIdentifierWasCalledOnNews()
        {
            Mock<INews> newsMock = CreateNewsMock();

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(newsMock.Object);
            buildTask.Wait();

            newsMock.Verify(m => m.Identifier, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertTimestampWasCalledOnNews()
        {
            Mock<INews> newsMock = CreateNewsMock();

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(newsMock.Object);
            buildTask.Wait();

            newsMock.Verify(m => m.Timestamp, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertInformationWasCalledOnNews()
        {
            Mock<INews> newsMock = CreateNewsMock();

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(newsMock.Object);
            buildTask.Wait();

            newsMock.Verify(m => m.Information, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertExtractImagesWasCalledOnHtmlHelperWithInformation()
        {
            IList<Uri> imageUrlCollection = null;

            string information = Guid.NewGuid().ToString("D");
            INews news = CreateNews(information: information);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(news);
            buildTask.Wait();

            _htmlHelperMock.Verify(m => m.ExtractImages(It.Is<string>(value => value == information), out imageUrlCollection), Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertDetailsWasCalledOnNews()
        {
            Mock<INews> newsMock = CreateNewsMock();

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(newsMock.Object);
            buildTask.Wait();

            newsMock.Verify(m => m.Details, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertExtractImagesWasCalledOnHtmlHelperWithDetails()
        {
            IList<Uri> imageUrlCollection = null;

            string details = Guid.NewGuid().ToString("D");
            INews news = CreateNews(details: details);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(news);
            buildTask.Wait();

            _htmlHelperMock.Verify(m => m.ExtractImages(It.Is<string>(value => value == details), out imageUrlCollection), Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertProviderWasCalledOnNews()
        {
            Mock<INews> newsMock = CreateNewsMock();

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(newsMock.Object);
            buildTask.Wait();

            newsMock.Verify(m => m.Provider, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertNameWasCalledOnProvider()
        {
            Mock<INewsProvider> provider = CreateNewsProviderMock();
            INews news = CreateNews(provider: provider.Object);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(news);
            buildTask.Wait();

            provider.Verify(m => m.Name, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertConvertNewLinesWasCalledOnHtmlHelperWithNameOfProvider()
        {
            string providerName = Guid.NewGuid().ToString("D");
            INewsProvider provider = CreateNewsProvider(providerName);
            INews news = CreateNews(provider: provider);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(news);
            buildTask.Wait();

            _htmlHelperMock.Verify(m => m.ConvertNewLines(It.Is<string>(value => value == providerName)), Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertLinkWasCalledOnNews()
        {
            Mock<INews> newsMock = CreateNewsMock();

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(newsMock.Object);
            buildTask.Wait();

            newsMock.Verify(m => m.Link, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertAuthorWasCalledOnNews()
        {
            Mock<INews> newsMock = CreateNewsMock();

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(newsMock.Object);
            buildTask.Wait();

            newsMock.Verify(m => m.Author, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledWhereNewsHasAuthor_AssertNameWasCalledOnAuthor()
        {
            Mock<IAuthor> authorMock = CreateAuthorMock();
            INews news = CreateNews(author: authorMock.Object);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(news);
            buildTask.Wait();

            authorMock.Verify(m => m.Name, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledWhereNewsHasAuthor_AssertConvertNewLinesWasCalledOnHtmlHelperWithNameOfAuthor()
        {
            string autherName = Guid.NewGuid().ToString("D");
            IAuthor author = CreateAuthor(autherName);
            INews news = CreateNews(author: author);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(news);
            buildTask.Wait();

            _htmlHelperMock.Verify(m => m.ConvertNewLines(It.Is<string>(value => value == autherName)), Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel()
        {
            bool hasLink = _random.Next(0, 100) > 50;
            bool hasAuthor = _random.Next(0, 100) > 50;
            bool hasImage = _random.Next(0, 100) > 50;
            BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor, hasImage);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledWhereNewsHasLink_ReturnsInitializedInformationViewModel()
        {
            const bool hasLink = true;
            bool hasAuthor = _random.Next(0, 100) > 50;
            bool hasImage = _random.Next(0, 100) > 50;
            BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor, hasImage);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledWhereNewsHasNoLink_ReturnsInitializedInformationViewModel()
        {
            const bool hasLink = false;
            bool hasAuthor = _random.Next(0, 100) > 50;
            bool hasImage = _random.Next(0, 100) > 50;
            BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor, hasImage);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledWhereNewsHasAuthor_ReturnsInitializedInformationViewModel()
        {
            bool hasLink = _random.Next(0, 100) > 50;
            const bool hasAuthor = true;
            bool hasImage = _random.Next(0, 100) > 50;
            BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor, hasImage);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledWhereNewsHasNoAuthor_ReturnsInitializedInformationViewModel()
        {
            bool hasLink = _random.Next(0, 100) > 50;
            const bool hasAuthor = false;
            bool hasImage = _random.Next(0, 100) > 50;
            BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor, hasImage);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledWhereNewsIncludesImage_ReturnsInitializedInformationViewModel()
        {
            bool hasLink = _random.Next(0, 100) > 50;
            bool hasAuthor = _random.Next(0, 100) > 50;
            const bool hasImage = true;
            BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor, hasImage);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledWhereNewsDoesNotIncludeImage_ReturnsInitializedInformationViewModel()
        {
            bool hasLink = _random.Next(0, 100) > 50;
            bool hasAuthor = _random.Next(0, 100) > 50;
            const bool hasImage = false;
            BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor, hasImage);
        }

        private void BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(bool hasLink, bool hasAuthor, bool hasImage)
        {
            string identifier = Guid.NewGuid().ToString("D");
            DateTime timestamp = DateTime.Now.AddTicks(_random.Next(-5000, 5000));
            string information = Guid.NewGuid().ToString("D");
            string details = Guid.NewGuid().ToString("D");
            string providerName = Guid.NewGuid().ToString("D");
            INewsProvider provider = CreateNewsProvider(providerName);
            Uri link = hasLink ? new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}") : null;
            string authorName = hasAuthor ? null : Guid.NewGuid().ToString("D");
            INews news = CreateNews(
                identifier, 
                timestamp,
                information,
                details,
                provider,
                link,
                string.IsNullOrWhiteSpace(authorName) == false ? CreateAuthor(authorName) : null);

            Uri imageUrl = null;
            if (hasImage)
            {
                imageUrl = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");
            }
            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut(imageUrl: imageUrl);

            Task<InformationViewModel> buildTask = sut.BuildAsync(news);
            buildTask.Wait();

            InformationViewModel result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.InformationIdentifier);
            Assert.AreEqual(identifier, result.InformationIdentifier);
            Assert.AreEqual(timestamp, result.Timestamp);
            Assert.IsNotNull(result.Header);
            Assert.AreEqual($"HtmlHelper.ExtractImages:{information}", result.Header);
            Assert.IsNull(result.Summary);
            Assert.IsNotNull(result.Details);
            Assert.AreEqual($"HtmlHelper.ExtractImages:{details}", result.Details);
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
            if (imageUrl != null)
            {
                Assert.IsNotNull(result.ImageUrl);
                Assert.AreEqual(imageUrl.AbsoluteUri, result.ImageUrl);
            }
            else
            {
                Assert.IsNull(result.ImageUrl);
            }
        }

        private IViewModelBuilder<InformationViewModel, INews> CreateSut(Uri imageUrl = null)
        {
            IList<Uri> imageUrlCollection = new List<Uri>();
            if (imageUrl != null)
            {
                imageUrlCollection.Add(imageUrl);
            }

            _htmlHelperMock.Setup(m => m.ConvertNewLines(It.IsAny<string>()))
                .Returns<string>(value => $"HtmlHelper.ConvertNewLines:{value}");
            _htmlHelperMock.Setup(m => m.ExtractImages(It.IsAny<string>(), out imageUrlCollection))
                .Returns<string, IList<Uri>>((value, urlCollection) => $"HtmlHelper.ExtractImages:{value}");
            
            return new OSDevGrp.MyDashboard.Web.Factories.NewsToInformationViewModelBuilder(_htmlHelperMock.Object);
        }

        private INews CreateNews(string identifier = null, DateTime? timestamp = null, string information = null, string details = null, INewsProvider provider = null, Uri link = null, IAuthor author = null)
        {
            return CreateNewsMock(identifier, timestamp, information, details, provider, link, author).Object;
        }

        private Mock<INews> CreateNewsMock(string identifier = null, DateTime? timestamp = null, string information = null, string details = null, INewsProvider provider = null, Uri link = null, IAuthor author = null)
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