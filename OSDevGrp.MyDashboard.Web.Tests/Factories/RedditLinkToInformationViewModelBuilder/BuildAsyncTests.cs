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

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.RedditLinkToInformationViewModelBuilder
{
    [TestClass]
    public class BuildAsyncTests
    {
        #region private variables

        private Mock<IRandomizer> _randomizerMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _randomizerMock = new Mock<IRandomizer>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullExceptionAttribute("input")]
        public async Task BuildAsync_WhenSystemErrorIsNull_ThrowsArgumentNullException()
        {
            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(null);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertIdentifierWasCalledOnRedditLink()
        {
            Mock<IRedditLink> redditLinkMock = CreateRedditLinkMock();

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.Identifier, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertCreatedTimeWasCalledOnRedditLink()
        {
            Mock<IRedditLink> redditLinkMock = CreateRedditLinkMock();

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.CreatedTime, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertTitleWasCalledOnRedditLink()
        {
            Mock<IRedditLink> redditLinkMock = CreateRedditLinkMock();

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.Title, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertSelftextAsTextWasCalledOnRedditLink()
        {
            Mock<IRedditLink> redditLinkMock = CreateRedditLinkMock();

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.SelftextAsText, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertSubredditWasCalledOnRedditLink()
        {
            Mock<IRedditLink> redditLinkMock = CreateRedditLinkMock();

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.Subreddit, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWithRedditSubreddit_AssertDisplayNamePrefixedWasCalledOnRedditSubreddit()
        {
            Mock<IRedditSubreddit> redditSubredditMock = CreateRedditSubredditMock(Guid.NewGuid().ToString("D"));
            IRedditLink redditLink = CreateRedditLink(redditSubreddit: redditSubredditMock.Object);

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLink);

            redditSubredditMock.Verify(m => m.DisplayNamePrefixed, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertAuthorWasCalledOnRedditLink()
        {
            Mock<IRedditLink> redditLinkMock = CreateRedditLinkMock();

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.Author, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertUrlWasCalledOnRedditLink()
        {
            Mock<IRedditLink> redditLinkMock = CreateRedditLinkMock();

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.Url, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertImagesWasCalledOnRedditLink()
        {
            Mock<IRedditLink> redditLinkMock = CreateRedditLinkMock();

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.Images, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWhereImagesIsNull_AssertThumbnailUrlWasCalledOnRedditLink()
        {
            const IEnumerable<IRedditImage> redditImages = null;
            Mock<IRedditLink> redditLinkMock = CreateRedditLinkMock(redditImages: redditImages);

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.ThumbnailUrl, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWhereImagesIsEmpty_AssertThumbnailUrlWasCalledOnRedditLink()
        {
            IEnumerable<IRedditImage> redditImages = new List<IRedditImage>(0);
            Mock<IRedditLink> redditLinkMock = CreateRedditLinkMock(redditImages: redditImages);

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.ThumbnailUrl, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWhereImagesIsNotEmpty_AssertThumbnailUrlWasNotCalledOnRedditLink()
        {
            IEnumerable<IRedditImage> redditImages = CreateRedditImageCollection();
            Mock<IRedditLink> redditLinkMock = CreateRedditLinkMock(redditImages: redditImages);

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLinkMock.Object);

            redditLinkMock.Verify(m => m.ThumbnailUrl, Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWhereImagesIsNotEmpty_AssertUrlWasCalledOnEachRedditImage()
        {
            Mock<IRedditImage> redditImage1Mock = CreateRedditImageMock(CreateImageUri());
            Mock<IRedditImage> redditImage2Mock = CreateRedditImageMock(CreateImageUri());
            Mock<IRedditImage> redditImage3Mock = CreateRedditImageMock(CreateImageUri());
            IEnumerable<IRedditImage> redditImages = CreateRedditImageCollection(
                redditImage1Mock.Object,
                redditImage2Mock.Object,
                redditImage3Mock.Object);
            IRedditLink redditLink = CreateRedditLink(redditImages: redditImages);

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLink);

            redditImage1Mock.Verify(m => m.Url, Times.Exactly(3));
            redditImage2Mock.Verify(m => m.Url, Times.Exactly(2));
            redditImage3Mock.Verify(m => m.Url, Times.Exactly(2));
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWhereImagesIsNullAndThumbnailUrlIsNull_AssertNextWasNotCalledOnRandomizer()
        {
            const IEnumerable<IRedditImage> redditImages = null;
            const Uri thumbnailUrl = null;
            IRedditLink redditLink = CreateRedditLink(redditImages: redditImages, thumbnailUrl: thumbnailUrl);

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLink);

            _randomizerMock.Verify(m => m.Next(
                    It.IsAny<int>(), 
                    It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWhereImagesIsEmptyAndThumbnailUrlIsNull_AssertNextWasNotCalledOnRandomizer()
        {
            IEnumerable<IRedditImage> redditImages = new List<IRedditImage>(0);
            const Uri thumbnailUrl = null;
            IRedditLink redditLink = CreateRedditLink(redditImages: redditImages, thumbnailUrl: thumbnailUrl);

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLink);

            _randomizerMock.Verify(m => m.Next(
                    It.IsAny<int>(), 
                    It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWhereImagesIsNullAndThumbnailUrlIsNotNull_AssertNextWasCalledOnRandomizer()
        {
            const IEnumerable<IRedditImage> redditImages = null;
            Uri thumbnailUrl = CreateImageUri();
            IRedditLink redditLink = CreateRedditLink(redditImages: redditImages, thumbnailUrl: thumbnailUrl);

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLink);

            _randomizerMock.Verify(m => m.Next(
                    It.Is<int>(value => value == 0), 
                    It.Is<int>(value => value == 0)),
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWhereImagesIsEmptyAndThumbnailUrlIsNotNull_AssertNextWasCalledOnRandomizer()
        {
            IEnumerable<IRedditImage> redditImages = new List<IRedditImage>(0);
            Uri thumbnailUrl = CreateImageUri();
            IRedditLink redditLink = CreateRedditLink(redditImages: redditImages, thumbnailUrl: thumbnailUrl);

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLink);

            _randomizerMock.Verify(m => m.Next(
                    It.Is<int>(value => value == 0), 
                    It.Is<int>(value => value == 0)),
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWhereImagesIsNotEmptyAndThumbnailUrlIsNull_AssertNextWasCalledOnRandomizer()
        {
            IEnumerable<IRedditImage> redditImages = CreateRedditImageCollection();
            const Uri thumbnailUrl = null;
            IRedditLink redditLink = CreateRedditLink(redditImages: redditImages, thumbnailUrl: thumbnailUrl);

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLink);

            _randomizerMock.Verify(m => m.Next(
                    It.Is<int>(value => value == 0), 
                    It.Is<int>(value => value ==redditImages.Count() - 1)),
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWhereImagesIsNotEmptyAndThumbnailUrlIsNotNull_AssertNextWasCalledOnRandomizer()
        {
            IEnumerable<IRedditImage> redditImages = CreateRedditImageCollection();
            Uri thumbnailUrl = CreateImageUri();
            IRedditLink redditLink = CreateRedditLink(redditImages: redditImages, thumbnailUrl: thumbnailUrl);

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            await sut.BuildAsync(redditLink);

            _randomizerMock.Verify(m => m.Next(
                    It.Is<int>(value => value == 0), 
                    It.Is<int>(value => value ==redditImages.Count() - 1)),
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel()
        {
            bool hasRedditSubreddit = _random.Next(1, 100) > 50;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            Uri thumbnailUrl = _random.Next(1, 100) > 50 ? CreateImageUri() : null;
            IEnumerable<IRedditImage> redditImages = _random.Next(1, 100) > 50 ? CreateRedditImageCollection() : null;
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            string redditSubredditDisplayNamePrefixed = hasRedditSubreddit ? Guid.NewGuid().ToString("D") : null;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWithTitle_ReturnsInitializedInformationViewModel()
        {
            bool hasRedditSubreddit = _random.Next(1, 100) > 50;
            string title = Guid.NewGuid().ToString("D");
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            Uri thumbnailUrl = _random.Next(1, 100) > 50 ? CreateImageUri() : null;
            IEnumerable<IRedditImage> redditImages = _random.Next(1, 100) > 50 ? CreateRedditImageCollection() : null;
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            string redditSubredditDisplayNamePrefixed = hasRedditSubreddit ? Guid.NewGuid().ToString("D") : null;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWithoutTitle_ReturnsInitializedInformationViewModel()
        {
            bool hasRedditSubreddit = _random.Next(1, 100) > 50;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            Uri thumbnailUrl = _random.Next(1, 100) > 50 ? CreateImageUri() : null;
            IEnumerable<IRedditImage> redditImages = _random.Next(1, 100) > 50 ? CreateRedditImageCollection() : null;
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            string redditSubredditDisplayNamePrefixed = hasRedditSubreddit ? Guid.NewGuid().ToString("D") : null;
            foreach (string title in new[] {null, string.Empty, " ", "  "})
            {
                await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
            }
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWithSelftextAsText_ReturnsInitializedInformationViewModel()
        {
            bool hasRedditSubreddit = _random.Next(1, 100) > 50;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = Guid.NewGuid().ToString("D");
            string author = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            Uri thumbnailUrl = _random.Next(1, 100) > 50 ? CreateImageUri() : null;
            IEnumerable<IRedditImage> redditImages = _random.Next(1, 100) > 50 ? CreateRedditImageCollection() : null;
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            string redditSubredditDisplayNamePrefixed = hasRedditSubreddit ? Guid.NewGuid().ToString("D") : null;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWithoutSelftextAsText_ReturnsInitializedInformationViewModel()
        {
            bool hasRedditSubreddit = _random.Next(1, 100) > 50;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            Uri thumbnailUrl = _random.Next(1, 100) > 50 ? CreateImageUri() : null;
            IEnumerable<IRedditImage> redditImages = _random.Next(1, 100) > 50 ? CreateRedditImageCollection() : null;
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            string redditSubredditDisplayNamePrefixed = hasRedditSubreddit ? Guid.NewGuid().ToString("D") : null;
            foreach (string selftextAsText in new[] {null, string.Empty, " ", "  "})
            {
                await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
            }
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWithRedditSubredditWithDisplayNamePrefix_ReturnsInitializedInformationViewModel()
        {
            const bool hasRedditSubreddit = true;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            Uri thumbnailUrl = _random.Next(1, 100) > 50 ? CreateImageUri() : null;
            IEnumerable<IRedditImage> redditImages = _random.Next(1, 100) > 50 ? CreateRedditImageCollection() : null;
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            string redditSubredditDisplayNamePrefixed = Guid.NewGuid().ToString("D");
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
        }

        public async Task BuildAsync_WhenCalledWithRedditLinkWithRedditSubredditWithoutDisplayNamePrefix_ReturnsInitializedInformationViewModel()
        {
            const bool hasRedditSubreddit = true;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            Uri thumbnailUrl = _random.Next(1, 100) > 50 ? CreateImageUri() : null;
            IEnumerable<IRedditImage> redditImages = _random.Next(1, 100) > 50 ? CreateRedditImageCollection() : null;
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            foreach (string redditSubredditDisplayNamePrefixed in new[] {null, string.Empty, " ", "  "})
            {
                await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
            }
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWithoutRedditSubreddit_ReturnsInitializedInformationViewModel()
        {
            const bool hasRedditSubreddit = false;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            Uri thumbnailUrl = _random.Next(1, 100) > 50 ? CreateImageUri() : null;
            IEnumerable<IRedditImage> redditImages = _random.Next(1, 100) > 50 ? CreateRedditImageCollection() : null;
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            const string redditSubredditDisplayNamePrefixed = null;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWithAuthorAndWithRedditSubredditWithDisplayNamePrefix_ReturnsInitializedInformationViewModel()
        {
            const bool hasRedditSubreddit = true;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = Guid.NewGuid().ToString("D");
            Uri thumbnailUrl = _random.Next(1, 100) > 50 ? CreateImageUri() : null;
            IEnumerable<IRedditImage> redditImages = _random.Next(1, 100) > 50 ? CreateRedditImageCollection() : null;
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            string redditSubredditDisplayNamePrefixed = Guid.NewGuid().ToString("D");
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWithAuthorAndWithRedditSubredditWithoutDisplayNamePrefix_ReturnsInitializedInformationViewModel()
        {
            const bool hasRedditSubreddit = true;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = Guid.NewGuid().ToString("D");
            Uri thumbnailUrl = _random.Next(1, 100) > 50 ? CreateImageUri() : null;
            IEnumerable<IRedditImage> redditImages = _random.Next(1, 100) > 50 ? CreateRedditImageCollection() : null;
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            foreach (string redditSubredditDisplayNamePrefixed in new[] {null, string.Empty, " ", "  "})
            {
                await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
            }
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWithAuthorAndWithoutRedditSubreddit_ReturnsInitializedInformationViewModel()
        {
            const bool hasRedditSubreddit = false;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = Guid.NewGuid().ToString("D");
            Uri thumbnailUrl = _random.Next(1, 100) > 50 ? CreateImageUri() : null;
            IEnumerable<IRedditImage> redditImages = _random.Next(1, 100) > 50 ? CreateRedditImageCollection() : null;
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            const string redditSubredditDisplayNamePrefixed = null;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWithoutAuthor_ReturnsInitializedInformationViewModel()
        {
            bool hasRedditSubreddit = _random.Next(1, 100) > 50;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string redditSubredditDisplayNamePrefixed = hasRedditSubreddit ? Guid.NewGuid().ToString("D") : null;
            Uri thumbnailUrl = _random.Next(1, 100) > 50 ? CreateImageUri() : null;
            IEnumerable<IRedditImage> redditImages = _random.Next(1, 100) > 50 ? CreateRedditImageCollection() : null;
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            foreach (string author in new[] {null, string.Empty, " ", "  "})
            {
                await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
            }
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWithUrl_ReturnsInitializedInformationViewModel()
        {
            bool hasRedditSubreddit = _random.Next(1, 100) > 50;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            Uri thumbnailUrl = _random.Next(1, 100) > 50 ? CreateImageUri() : null;
            IEnumerable<IRedditImage> redditImages = _random.Next(1, 100) > 50 ? CreateRedditImageCollection() : null;
            Uri url = CreateUri();
            string redditSubredditDisplayNamePrefixed = hasRedditSubreddit ? Guid.NewGuid().ToString("D") : null;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWithoutUrl_ReturnsInitializedInformationViewModel()
        {
            bool hasRedditSubreddit = _random.Next(1, 100) > 50;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            Uri thumbnailUrl = _random.Next(1, 100) > 50 ? CreateImageUri() : null;
            IEnumerable<IRedditImage> redditImages = _random.Next(1, 100) > 50 ? CreateRedditImageCollection() : null;
            const Uri url = null;
            string redditSubredditDisplayNamePrefixed = hasRedditSubreddit ? Guid.NewGuid().ToString("D") : null;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWhereImagesIsNullAndThumbnailUrlIsNull_ReturnsInitializedInformationViewModel()
        {
            bool hasRedditSubreddit = _random.Next(1, 100) > 50;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            const Uri thumbnailUrl = null;
            const IEnumerable<IRedditImage> redditImages = null;
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            string redditSubredditDisplayNamePrefixed = hasRedditSubreddit ? Guid.NewGuid().ToString("D") : null;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWhereImagesIsEmptyAndThumbnailUrlIsNull_ReturnsInitializedInformationViewModel()
        {
            bool hasRedditSubreddit = _random.Next(1, 100) > 50;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            const Uri thumbnailUrl = null;
            IEnumerable<IRedditImage> redditImages = new List<IRedditImage>(0);
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            string redditSubredditDisplayNamePrefixed = hasRedditSubreddit ? Guid.NewGuid().ToString("D") : null;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWhereImagesIsNullAndThumbnailUrlIsNotNull_ReturnsInitializedInformationViewModel()
        {
            bool hasRedditSubreddit = _random.Next(1, 100) > 50;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            Uri thumbnailUrl = CreateImageUri();
            const IEnumerable<IRedditImage> redditImages = null;
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            string redditSubredditDisplayNamePrefixed = hasRedditSubreddit ? Guid.NewGuid().ToString("D") : null;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWhereImagesIsEmptyAndThumbnailUrlIsNotNull_ReturnsInitializedInformationViewModel()
        {
            bool hasRedditSubreddit = _random.Next(1, 100) > 50;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            Uri thumbnailUrl = CreateImageUri();
            IEnumerable<IRedditImage> redditImages = new List<IRedditImage>(0);
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            string redditSubredditDisplayNamePrefixed = hasRedditSubreddit ? Guid.NewGuid().ToString("D") : null;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWhereImagesIsNotEmptyAndThumbnailUrlIsNull_ReturnsInitializedInformationViewModel()
        {
            bool hasRedditSubreddit = _random.Next(1, 100) > 50;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            const Uri thumbnailUrl = null;
            IEnumerable<IRedditImage> redditImages = CreateRedditImageCollection();
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            string redditSubredditDisplayNamePrefixed = hasRedditSubreddit ? Guid.NewGuid().ToString("D") : null;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditLinkWhereImagesIsNotEmptyAndThumbnailUrlIsNotNull_ReturnsInitializedInformationViewModel()
        {
            bool hasRedditSubreddit = _random.Next(1, 100) > 50;
            string title = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string selftextAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string author = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString("D") : null;
            Uri thumbnailUrl = CreateImageUri();
            IEnumerable<IRedditImage> redditImages = CreateRedditImageCollection();
            Uri url = _random.Next(1, 100) > 50 ? CreateUri() : null;
            string redditSubredditDisplayNamePrefixed = hasRedditSubreddit ? Guid.NewGuid().ToString("D") : null;
            await BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(hasRedditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url, redditSubredditDisplayNamePrefixed);
        }

        private async Task BuildAsync_WhenCalled_ReturnsInitializedInformationViewModel(bool hasRedditSubreddit, string title, string selftextAsText, string author, Uri thumbnailUrl, IEnumerable<IRedditImage> redditImages, Uri url, string redditSubredditDisplayNamePrefixed)
        {
            string identifier = Guid.NewGuid().ToString("D");
            DateTime createdTime = DateTime.Now.AddMinutes(_random.Next(60, 300) * -1);
            IRedditSubreddit redditSubreddit = hasRedditSubreddit ? CreateRedditSubreddit(displayNamePrefixed: redditSubredditDisplayNamePrefixed) : null;
            IRedditLink redditLink = CreateRedditLink(
                identifier: identifier,
                createdTime: createdTime,
                redditSubreddit: redditSubreddit,
                title: title,
                selftextAsText: selftextAsText,
                author: author,
                thumbnailUrl: thumbnailUrl,
                redditImages: redditImages,
                url: url);

            IViewModelBuilder<InformationViewModel, IRedditLink> sut = CreateSut();

            InformationViewModel result = await sut.BuildAsync(redditLink);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.InformationIdentifier);
            Assert.AreEqual(identifier, result.InformationIdentifier);
            Assert.AreEqual(createdTime, result.Timestamp);
            if (string.IsNullOrWhiteSpace(title))
            {
                Assert.IsNull(result.Header);
            }
            else
            {
                Assert.IsNotNull(result.Header);
                Assert.AreEqual(title, result.Header);
            }
            if (string.IsNullOrWhiteSpace(selftextAsText))
            {
                Assert.IsNull(result.Details);
            }
            else
            {
                Assert.IsNotNull(result.Details);
                Assert.AreEqual(selftextAsText, result.Details);
            }
            Assert.IsNull(result.Summary);
            if (redditImages != null && redditImages.Any(image => image.Url != null))
            {
                Assert.IsNotNull(result.ImageUrl);
                Assert.IsTrue(redditImages.Where(image => image.Url != null).Select(image => image.Url.AbsoluteUri).Contains(result.ImageUrl));
            }
            else if (thumbnailUrl != null)
            {
                Assert.IsNotNull(result.ImageUrl);
                Assert.AreEqual(thumbnailUrl.AbsoluteUri, result.ImageUrl);
            }
            else
            {
                Assert.IsNull(result.ImageUrl);
            }
            if (hasRedditSubreddit == false || string.IsNullOrWhiteSpace(redditSubredditDisplayNamePrefixed))
            {
                Assert.IsNull(result.Provider);
            }
            else
            {
                Assert.IsNotNull(result.Provider);
                Assert.AreEqual(redditSubredditDisplayNamePrefixed, result.Provider);
            }
            if (string.IsNullOrWhiteSpace(author))
            {
                Assert.IsNull(result.Author);
            }
            else if (hasRedditSubreddit == false || string.IsNullOrWhiteSpace(redditSubredditDisplayNamePrefixed))
            {
                Assert.IsNotNull(result.Author);
                Assert.AreEqual(author, result.Author);
            }
            else
            {
                Assert.IsNotNull(result.Author);
                Assert.AreEqual($"{author} @ {redditSubredditDisplayNamePrefixed}", result.Author);
            }
            if (url == null)
            {
                Assert.IsNotNull(result.ExternalUrl);
                Assert.AreEqual("#", result.ExternalUrl);
            }
            else
            {
                Assert.IsNotNull(result.ExternalUrl);
                Assert.AreEqual(url.AbsoluteUri, result.ExternalUrl);
            }
        }

        private IViewModelBuilder<InformationViewModel, IRedditLink> CreateSut()
        {
            _randomizerMock.Setup(m => m.Next(It.IsAny<int>(), It.IsAny<int>()))
                .Returns<int, int>((minValue, maxValue) => _random.Next(minValue, maxValue));

            return new OSDevGrp.MyDashboard.Web.Factories.RedditLinkToInformationViewModelBuilder(_randomizerMock.Object);
        }

        private IRedditLink CreateRedditLink(string identifier = null, DateTime? createdTime = null, IRedditSubreddit redditSubreddit = null, string title = null, string selftextAsText = null, string author = null, Uri thumbnailUrl = null, IEnumerable<IRedditImage> redditImages = null, Uri url = null)
        {
            return CreateRedditLinkMock(identifier, createdTime, redditSubreddit, title, selftextAsText, author, thumbnailUrl, redditImages, url).Object;
        }

        private Mock<IRedditLink> CreateRedditLinkMock(string identifier = null, DateTime? createdTime = null, IRedditSubreddit redditSubreddit = null, string title = null, string selftextAsText = null, string author = null, Uri thumbnailUrl = null, IEnumerable<IRedditImage> redditImages = null, Uri url = null)
        {
            Mock<IRedditLink> redditLinkMock = new Mock<IRedditLink>();
            redditLinkMock.Setup(m => m.Identifier)
                .Returns(identifier ?? Guid.NewGuid().ToString("D"));
            redditLinkMock.Setup(m => m.CreatedTime)
                .Returns(createdTime ?? DateTime.Now.AddMinutes(_random.Next(60, 300) * -1));
            redditLinkMock.Setup(m => m.Subreddit)
                .Returns(redditSubreddit);
            redditLinkMock.Setup(m => m.Title)
                .Returns(title);
            redditLinkMock.Setup(m => m.SelftextAsText)
                .Returns(selftextAsText);
            redditLinkMock.Setup(m => m.Author)
                .Returns(author);
            redditLinkMock.Setup(m => m.ThumbnailUrl)
                .Returns(thumbnailUrl);
            redditLinkMock.Setup(m => m.Images)
                .Returns(redditImages);
            redditLinkMock.Setup(m => m.Url)
                .Returns(url);
            return redditLinkMock;
        }

        private IRedditSubreddit CreateRedditSubreddit(string displayNamePrefixed = null)
        {
            return CreateRedditSubredditMock(displayNamePrefixed).Object;
        }

        private Mock<IRedditSubreddit> CreateRedditSubredditMock(string displayNamePrefixed = null)
        {
            Mock<IRedditSubreddit> redditSubredditMock = new Mock<IRedditSubreddit>();
            redditSubredditMock.Setup(m => m.DisplayNamePrefixed)
                .Returns(displayNamePrefixed);
            return redditSubredditMock;
        }

        private IEnumerable<IRedditImage> CreateRedditImageCollection()
        {
            return CreateRedditImageCollection(
                CreateRedditImage(CreateImageUri()),
                CreateRedditImage(CreateImageUri()),
                CreateRedditImage(CreateImageUri()));
        }

        private IEnumerable<IRedditImage> CreateRedditImageCollection(params IRedditImage[] src)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }
            return src;
        }

        private IRedditImage CreateRedditImage(Uri imageUrl = null)
        {
            return CreateRedditImageMock(imageUrl).Object;
        }

        private Mock<IRedditImage> CreateRedditImageMock(Uri imageUrl = null)
        {
            Mock<IRedditImage> redditImageMock = new Mock<IRedditImage>();
            redditImageMock.Setup(m => m.Url)
                .Returns(imageUrl);
            return redditImageMock;
        }

        private Uri CreateUri()
        {
            return new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");
        }

        private Uri CreateImageUri()
        {
            return new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}.png");
        }
    } 
}