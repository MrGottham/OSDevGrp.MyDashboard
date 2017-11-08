using System;
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
        public void BuildAsync_WhenCalled_AssertConvertNewLinesWasCalledOnHtmlHelperWithInformation()
        {
            string information = Guid.NewGuid().ToString("D");
            Mock<INews> newsMock = CreateNewsMock(information: information);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(newsMock.Object);
            buildTask.Wait();

            _htmlHelperMock.Verify(m => m.ConvertNewLines(It.Is<string>(value => value == information)), Times.Once);
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
        public void BuildAsync_WhenCalled_AssertConvertNewLinesWasCalledOnHtmlHelperWithDetails()
        {
            string details = Guid.NewGuid().ToString("D");
            Mock<INews> newsMock = CreateNewsMock(details: details);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(newsMock.Object);
            buildTask.Wait();

            _htmlHelperMock.Verify(m => m.ConvertNewLines(It.Is<string>(value => value == details)), Times.Once);
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
        public void BuildAsync_WhenCalled_AssertConvertNewLinesWasCalledOnHtmlHelperWithNameOfProvider()
        {
            string providerName = Guid.NewGuid().ToString("D");
            INewsProvider provider = CreateNewsProvider(providerName);
            Mock<INews> newsMock = CreateNewsMock(provider: provider);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(newsMock.Object);
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
        public void BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel()
        {
            bool hasLink = _random.Next(0, 100) > 50;
            bool hasAuthor = _random.Next(0, 100) > 50;
            BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledWhereNewsHasNoLink_ReturnsInitializedInformationViewModel()
        {
            const bool hasLink = false;
            bool hasAuthor = _random.Next(0, 100) > 50;
            BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledWhereNewsHasLink_ReturnsInitializedInformationViewModel()
        {
            const bool hasLink = true;
            bool hasAuthor = _random.Next(0, 100) > 50;
            BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasLink, hasAuthor);
        }

        private void BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(bool hasLink, bool hasAuthor)
        {
            string identifier = Guid.NewGuid().ToString("D");
            DateTime timestamp = DateTime.Now.AddTicks(_random.Next(-5000, 5000));
            string information = Guid.NewGuid().ToString("D");
            string details = Guid.NewGuid().ToString("D");
            string providerName = Guid.NewGuid().ToString("D");
            INewsProvider provider = CreateNewsProvider(providerName);
            Uri link = null;
            if (hasLink)
            {
                link = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");
            }
            IAuthor author = null;
            if (hasAuthor)
            {
                string authorName = Guid.NewGuid().ToString("D");
                author = CreateAuthor(authorName);
            }
            INews news = CreateNews(identifier, timestamp, information, details, provider, link, author);

            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            Task<InformationViewModel> buildTask = sut.BuildAsync(news);
            buildTask.Wait();

            InformationViewModel result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.InformationIdentifier);
            Assert.AreEqual(identifier, result.InformationIdentifier);
            Assert.AreEqual(timestamp, result.Timestamp);
            Assert.IsNotNull(result.Header);
            Assert.AreEqual($"HtmlHelper.ConvertNewLines:{information}", result.Header);
            Assert.IsNull(result.Summary);
            Assert.IsNotNull(result.Details);
            Assert.AreEqual($"HtmlHelper.ConvertNewLines:{details}", result.Details);
            Assert.IsNull(result.ImageUrl);
            Assert.IsNotNull(result.Provider);
            Assert.AreEqual($"HtmlHelper.ConvertNewLines:{providerName}", result.Provider);
            if (hasLink && link != null)
            {
                Assert.IsNotNull(result.ExternalUrl);
                Assert.AreEqual(link.AbsoluteUri, result.ExternalUrl);
            }
            else
            {
                Assert.IsNull(result.ExternalUrl);
            }
        }

        private IViewModelBuilder<InformationViewModel, INews> CreateSut()
        {
            _htmlHelperMock.Setup(m => m.ConvertNewLines(It.IsAny<string>()))
                .Returns<string>(value => $"HtmlHelper.ConvertNewLines:{value}");
            
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