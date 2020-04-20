using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.RedditSubredditToObjectViewModelBuilder
{
    [TestClass]
    public class BuildAsyncTests
    {
        #region private variables

        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullExceptionAttribute("input")]
        public async Task BuildAsync_WhenRedditSubredditIsNull_ThrowsArgumentNullException()
        {
            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            await sut.BuildAsync(null);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertIdentifierWasCalledOnRedditSubreddit()
        {
            Mock<IRedditSubreddit> redditSubredditMock = CreateRedditSubredditMock();

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            await sut.BuildAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.Identifier, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertCreatedTimeWasCalledOnRedditSubreddit()
        {
            Mock<IRedditSubreddit> redditSubredditMock = CreateRedditSubredditMock();

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            await sut.BuildAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.CreatedTime, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertDisplayNamePrefixedWasCalledOnRedditSubreddit()
        {
            Mock<IRedditSubreddit> redditSubredditMock = CreateRedditSubredditMock();

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            await sut.BuildAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.DisplayNamePrefixed, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertPublicDescriptionAsTextWasCalledOnRedditSubreddit()
        {
            Mock<IRedditSubreddit> redditSubredditMock = CreateRedditSubredditMock();

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            await sut.BuildAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.PublicDescriptionAsText, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertSubscribersWasCalledOnRedditSubreddit()
        {
            Mock<IRedditSubreddit> redditSubredditMock = CreateRedditSubredditMock();

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            await sut.BuildAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.Subscribers, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertUrlWasCalledOnRedditSubreddit()
        {
            Mock<IRedditSubreddit> redditSubredditMock = CreateRedditSubredditMock();

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            await sut.BuildAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.Url, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertBannerImageUrlWasCalledOnRedditSubreddit()
        {
            Mock<IRedditSubreddit> redditSubredditMock = CreateRedditSubredditMock();

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            await sut.BuildAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.BannerImageUrl, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereRedditSubredditHasBannerImageUrl_AssertHeaderImageUrlWasNotCalledOnRedditSubreddit()
        {
            Uri bannerImageUrl = new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png");
            Mock<IRedditSubreddit> redditSubredditMock = CreateRedditSubredditMock(bannerImageUrl: bannerImageUrl);

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            await sut.BuildAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.HeaderImageUrl, Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereRedditSubredditDoesNotHaveBannerImageUrl_AssertHeaderImageUrlWasCalledOnRedditSubreddit()
        {
            const Uri bannerImageUrl = null;
            Mock<IRedditSubreddit> redditSubredditMock = CreateRedditSubredditMock(bannerImageUrl: bannerImageUrl);

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            await sut.BuildAsync(redditSubredditMock.Object);

            redditSubredditMock.Verify(m => m.HeaderImageUrl, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_ReturnsInitializedObjectViewModel()
        {
            string identifier = Guid.NewGuid().ToString("D");
            DateTime createdTime = DateTime.Now.AddDays(_random.Next(1, 365) * -1).AddMinutes(_random.Next(-120, 120));
            string displayNamePrefixed = Guid.NewGuid().ToString("D");
            string publicDescriptionAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString() : null;
            long subscribers = _random.Next(2500, 5000);
            Uri url = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}") : null;
            Uri bannerImageUrl = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png") : null;
            Uri headerImageUrl = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png") : null;
            IRedditSubreddit redditSubreddit = CreateRedditSubreddit(identifier, createdTime, displayNamePrefixed, publicDescriptionAsText, subscribers, url, bannerImageUrl, headerImageUrl);

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            ObjectViewModel<IRedditSubreddit> result = await sut.BuildAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ObjectIdentifier);
            Assert.AreEqual(identifier, result.ObjectIdentifier);
            Assert.IsNotNull(result.Object);
            Assert.AreEqual(redditSubreddit, result.Object);
            Assert.AreEqual(createdTime, result.Timestamp);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(displayNamePrefixed, publicDescriptionAsText, subscribers, url, bannerImageUrl ?? headerImageUrl), result.Html);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_ReturnsInitializedObjectViewModelWithCorrectHtml()
        {
            string displayNamePrefixed = Guid.NewGuid().ToString("D");
            string publicDescriptionAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString() : null;
            long subscribers = _random.Next(2500, 5000);
            Uri url = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}") : null;
            Uri bannerImageUrl = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png") : null;
            Uri headerImageUrl = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png") : null;
            IRedditSubreddit redditSubreddit = CreateRedditSubreddit(displayNamePrefixed: displayNamePrefixed, publicDescriptionAsText: publicDescriptionAsText, subscribers: subscribers, url: url, bannerImageUrl: bannerImageUrl, headerImageUrl: headerImageUrl);

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            ObjectViewModel<IRedditSubreddit> result = await sut.BuildAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(displayNamePrefixed, publicDescriptionAsText, subscribers, url, bannerImageUrl ?? headerImageUrl), result.Html);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereRedditSubredditHasPublicDescriptionAsText_ReturnsInitializedObjectViewModelWithCorrectHtml()
        {
            string displayNamePrefixed = Guid.NewGuid().ToString("D");
            string publicDescriptionAsText = Guid.NewGuid().ToString();
            long subscribers = _random.Next(2500, 5000);
            Uri url = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}") : null;
            Uri bannerImageUrl = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png") : null;
            Uri headerImageUrl = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png") : null;
            IRedditSubreddit redditSubreddit = CreateRedditSubreddit(displayNamePrefixed: displayNamePrefixed, publicDescriptionAsText: publicDescriptionAsText, subscribers: subscribers, url: url, bannerImageUrl: bannerImageUrl, headerImageUrl: headerImageUrl);

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            ObjectViewModel<IRedditSubreddit> result = await sut.BuildAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(displayNamePrefixed, publicDescriptionAsText, subscribers, url, bannerImageUrl ?? headerImageUrl), result.Html);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereRedditSubredditDoesNotHavePublicDescriptionAsText_ReturnsInitializedObjectViewModelWithCorrectHtml()
        {
            string displayNamePrefixed = Guid.NewGuid().ToString("D");
            const string publicDescriptionAsText = null;
            long subscribers = _random.Next(2500, 5000);
            Uri url = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}") : null;
            Uri bannerImageUrl = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png") : null;
            Uri headerImageUrl = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png") : null;
            IRedditSubreddit redditSubreddit = CreateRedditSubreddit(displayNamePrefixed: displayNamePrefixed, publicDescriptionAsText: publicDescriptionAsText, subscribers: subscribers, url: url, bannerImageUrl: bannerImageUrl, headerImageUrl: headerImageUrl);

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            ObjectViewModel<IRedditSubreddit> result = await sut.BuildAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(displayNamePrefixed, publicDescriptionAsText, subscribers, url, bannerImageUrl ?? headerImageUrl), result.Html);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereRedditSubredditHasUrl_ReturnsInitializedObjectViewModelWithCorrectHtml()
        {
            string displayNamePrefixed = Guid.NewGuid().ToString("D");
            string publicDescriptionAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString() : null;
            long subscribers = _random.Next(2500, 5000);
            Uri url = new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}");
            Uri bannerImageUrl = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png") : null;
            Uri headerImageUrl = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png") : null;
            IRedditSubreddit redditSubreddit = CreateRedditSubreddit(displayNamePrefixed: displayNamePrefixed, publicDescriptionAsText: publicDescriptionAsText, subscribers: subscribers, url: url, bannerImageUrl: bannerImageUrl, headerImageUrl: headerImageUrl);

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            ObjectViewModel<IRedditSubreddit> result = await sut.BuildAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(displayNamePrefixed, publicDescriptionAsText, subscribers, url, bannerImageUrl ?? headerImageUrl), result.Html);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereRedditSubredditDoesNotHaveUrl_ReturnsInitializedObjectViewModelWithCorrectHtml()
        {
            string displayNamePrefixed = Guid.NewGuid().ToString("D");
            string publicDescriptionAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString() : null;
            long subscribers = _random.Next(2500, 5000);
            const Uri url = null;
            Uri bannerImageUrl = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png") : null;
            Uri headerImageUrl = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png") : null;
            IRedditSubreddit redditSubreddit = CreateRedditSubreddit(displayNamePrefixed: displayNamePrefixed, publicDescriptionAsText: publicDescriptionAsText, subscribers: subscribers, url: url, bannerImageUrl: bannerImageUrl, headerImageUrl: headerImageUrl);

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            ObjectViewModel<IRedditSubreddit> result = await sut.BuildAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(displayNamePrefixed, publicDescriptionAsText, subscribers, url, bannerImageUrl ?? headerImageUrl), result.Html);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereRedditSubredditHasBothBannerImageUrlAndHeaderImageUrl_ReturnsInitializedObjectViewModelWithCorrectHtml()
        {
            string displayNamePrefixed = Guid.NewGuid().ToString("D");
            string publicDescriptionAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString() : null;
            long subscribers = _random.Next(2500, 5000);
            Uri url = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}") : null;
            Uri bannerImageUrl = new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png");
            Uri headerImageUrl = new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png");
            IRedditSubreddit redditSubreddit = CreateRedditSubreddit(displayNamePrefixed: displayNamePrefixed, publicDescriptionAsText: publicDescriptionAsText, subscribers: subscribers, url: url, bannerImageUrl: bannerImageUrl, headerImageUrl: headerImageUrl);

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            ObjectViewModel<IRedditSubreddit> result = await sut.BuildAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(displayNamePrefixed, publicDescriptionAsText, subscribers, url, bannerImageUrl), result.Html);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereRedditSubredditHasOnlyBannerImageUrl_ReturnsInitializedObjectViewModelWithCorrectHtml()
        {
            string displayNamePrefixed = Guid.NewGuid().ToString("D");
            string publicDescriptionAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString() : null;
            long subscribers = _random.Next(2500, 5000);
            Uri url = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}") : null;
            Uri bannerImageUrl = new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png");
            const Uri headerImageUrl = null;
            IRedditSubreddit redditSubreddit = CreateRedditSubreddit(displayNamePrefixed: displayNamePrefixed, publicDescriptionAsText: publicDescriptionAsText, subscribers: subscribers, url: url, bannerImageUrl: bannerImageUrl, headerImageUrl: headerImageUrl);

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            ObjectViewModel<IRedditSubreddit> result = await sut.BuildAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(displayNamePrefixed, publicDescriptionAsText, subscribers, url, bannerImageUrl), result.Html);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereRedditSubredditHasOnlyHeaderImageUrl_ReturnsInitializedObjectViewModelWithCorrectHtml()
        {
            string displayNamePrefixed = Guid.NewGuid().ToString("D");
            string publicDescriptionAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString() : null;
            long subscribers = _random.Next(2500, 5000);
            Uri url = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}") : null;
            const Uri bannerImageUrl = null;
            Uri headerImageUrl = new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}.png");
            IRedditSubreddit redditSubreddit = CreateRedditSubreddit(displayNamePrefixed: displayNamePrefixed, publicDescriptionAsText: publicDescriptionAsText, subscribers: subscribers, url: url, bannerImageUrl: bannerImageUrl, headerImageUrl: headerImageUrl);

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            ObjectViewModel<IRedditSubreddit> result = await sut.BuildAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(displayNamePrefixed, publicDescriptionAsText, subscribers, url, headerImageUrl), result.Html);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereRedditSubredditDoesNotHaveBannerImageUrlNorHeaderImageUrl_ReturnsInitializedObjectViewModelWithCorrectHtml()
        {
            string displayNamePrefixed = Guid.NewGuid().ToString("D");
            string publicDescriptionAsText = _random.Next(1, 100) > 50 ? Guid.NewGuid().ToString() : null;
            long subscribers = _random.Next(2500, 5000);
            Uri url = _random.Next(1, 100) > 50 ? new Uri($"http://locahost/{Guid.NewGuid().ToString("D")}") : null;
            const Uri bannerImageUrl = null;
            const Uri headerImageUrl = null;
            IRedditSubreddit redditSubreddit = CreateRedditSubreddit(displayNamePrefixed: displayNamePrefixed, publicDescriptionAsText: publicDescriptionAsText, subscribers: subscribers, url: url, bannerImageUrl: bannerImageUrl, headerImageUrl: headerImageUrl);

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            ObjectViewModel<IRedditSubreddit> result = await sut.BuildAsync(redditSubreddit);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(displayNamePrefixed, publicDescriptionAsText, subscribers, url, null), result.Html);
        }

        private IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Factories.RedditSubredditToObjectViewModelBuilder();
        }

        private IRedditSubreddit CreateRedditSubreddit(string identifier = null, DateTime? createdTime = null, string displayNamePrefixed = null, string publicDescriptionAsText = null, long? subscribers = null, Uri url = null, Uri bannerImageUrl = null, Uri headerImageUrl = null)
        {
            return CreateRedditSubredditMock(identifier, createdTime, displayNamePrefixed, publicDescriptionAsText, subscribers, url, bannerImageUrl, headerImageUrl).Object;
        }

        private Mock<IRedditSubreddit> CreateRedditSubredditMock(string identifier = null, DateTime? createdTime = null, string displayNamePrefixed = null, string publicDescriptionAsText = null, long? subscribers = null, Uri url = null, Uri bannerImageUrl = null, Uri headerImageUrl = null)
        {
            Mock<IRedditSubreddit> redditSubredditMock = new Mock<IRedditSubreddit>();
            redditSubredditMock.Setup(m => m.Identifier)
                .Returns(identifier);
            redditSubredditMock.Setup(m => m.CreatedTime)
                .Returns(createdTime ?? DateTime.Now.AddDays(_random.Next(1, 365) * -1).AddMinutes(_random.Next(-120, 120)));
            redditSubredditMock.Setup(m => m.DisplayNamePrefixed)
                .Returns(displayNamePrefixed ?? Guid.NewGuid().ToString("D"));
            redditSubredditMock.Setup(m => m.PublicDescriptionAsText)
                .Returns(publicDescriptionAsText);
            redditSubredditMock.Setup(m => m.Subscribers)
                .Returns(subscribers ?? _random.Next(1000, 2500));
            redditSubredditMock.Setup(m => m.Url)
                .Returns(url);
            redditSubredditMock.Setup(m => m.BannerImageUrl)
                .Returns(bannerImageUrl);
            redditSubredditMock.Setup(m => m.HeaderImageUrl)
                .Returns(headerImageUrl);
            return redditSubredditMock;
        }
 
        private string GetExpectedHtml(string displayNamePrefixed, string publicDescription, long subscribers, Uri url, Uri imageUrl)
        {
            if (string.IsNullOrWhiteSpace(displayNamePrefixed))
            {
                throw new ArgumentNullException(nameof(displayNamePrefixed));
            }

            StringBuilder htmlBuilder = new StringBuilder("<div class=\"card mb-3\">");
            if (imageUrl != null)
            {
                htmlBuilder.Append($"<img class=\"card-img-top img-fluid\" src=\"{imageUrl.AbsoluteUri}\" />");
            }
            htmlBuilder.Append("<div class=\"card-body\">");
            htmlBuilder.Append("<h5 class=\"card-title\">");
            if (url != null)
            {
                htmlBuilder.Append($"<a href=\"#\" onclick=\"javascript:$().openLink('{url.AbsoluteUri}');\">{displayNamePrefixed}</a>");
            }
            else
            {
                htmlBuilder.Append(displayNamePrefixed);
            }
            htmlBuilder.Append("</h5>");
            htmlBuilder.Append("<p class=\"card-text\">");
            if (string.IsNullOrWhiteSpace(publicDescription) == false)
            {
                htmlBuilder.Append($"{publicDescription}<br><br>");
            }
            htmlBuilder.Append($"Subscribers: {subscribers:N0}");
            htmlBuilder.Append("</p>");
            htmlBuilder.Append("</div>");
            htmlBuilder.Append("</div>");

            return htmlBuilder.ToString();
        }
    }
}