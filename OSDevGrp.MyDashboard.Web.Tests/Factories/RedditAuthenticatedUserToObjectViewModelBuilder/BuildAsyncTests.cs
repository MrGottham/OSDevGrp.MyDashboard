using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.RedditAuthenticatedUserToObjectViewModelBuilder
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
        public async Task BuildAsync_WhenRedditAuthenticatedUserIsNull_ThrowsArgumentNullException()
        {
            IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser> sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.BuildAsync(null));

            Assert.AreEqual("input", result.ParamName);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertIdentifierWasCalledOnRedditAuthenticatedUser()
        {
            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = CreateRedditAuthenticatedUserMock();

            IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser> sut = CreateSut();

            await sut.BuildAsync(redditAuthenticatedUserMock.Object);

            redditAuthenticatedUserMock.Verify(m => m.Identifier, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertCreatedTimeWasCalledOnRedditAuthenticatedUser()
        {
            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = CreateRedditAuthenticatedUserMock();

            IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser> sut = CreateSut();

            await sut.BuildAsync(redditAuthenticatedUserMock.Object);

            redditAuthenticatedUserMock.Verify(m => m.CreatedTime, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertUserNameWasCalledOnRedditAuthenticatedUser()
        {
            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = CreateRedditAuthenticatedUserMock();

            IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser> sut = CreateSut();

            await sut.BuildAsync(redditAuthenticatedUserMock.Object);

            redditAuthenticatedUserMock.Verify(m => m.UserName, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertCommentKarmaWasCalledOnRedditAuthenticatedUser()
        {
            int commentKarma = GetKarmaValue();
            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = CreateRedditAuthenticatedUserMock(commentKarma: commentKarma);

            IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser> sut = CreateSut();

            await sut.BuildAsync(redditAuthenticatedUserMock.Object);

            redditAuthenticatedUserMock.Verify(m => m.CommentKarma, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertLinkKarmaWasCalledOnRedditAuthenticatedUser()
        {
            int linkKarma = GetKarmaValue();
            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = CreateRedditAuthenticatedUserMock(linkKarma: linkKarma);

            IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser> sut = CreateSut();

            await sut.BuildAsync(redditAuthenticatedUserMock.Object);

            redditAuthenticatedUserMock.Verify(m => m.LinkKarma, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertHasUnreadMailWasCalledOnRedditAuthenticatedUser()
        {
            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = CreateRedditAuthenticatedUserMock();

            IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser> sut = CreateSut();

            await sut.BuildAsync(redditAuthenticatedUserMock.Object);

            redditAuthenticatedUserMock.Verify(m => m.HasUnreadMail, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_ReturnsInitializedObjectViewModel()
        {
            string identifier = Guid.NewGuid().ToString("D");
            DateTime createdTime = DateTime.Now.AddDays(_random.Next(1, 365) * -1).AddMinutes(_random.Next(-120, 120));
            string userName = Guid.NewGuid().ToString("D");
            int commentKarma = GetKarmaValue();
            int linkKarma = GetKarmaValue();
            bool hasUnreadMail = _random.Next(1, 100) > 50;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(identifier, createdTime, userName, commentKarma, linkKarma, hasUnreadMail);

            IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser> sut = CreateSut();

            ObjectViewModel<IRedditAuthenticatedUser> result = await sut.BuildAsync(redditAuthenticatedUser);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ObjectIdentifier);
            Assert.AreEqual(identifier, result.ObjectIdentifier);
            Assert.IsNotNull(result.Object);
            Assert.AreEqual(redditAuthenticatedUser, result.Object);
            Assert.AreEqual(createdTime, result.Timestamp);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(userName, commentKarma, linkKarma, hasUnreadMail), result.Html);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditAuthenticatedUserWithCommentKarma_ReturnsInitializedObjectViewModelWithCorrectHtml()
        {
            string userName = Guid.NewGuid().ToString("D");
            int commentKarma = GetKarmaValue(hasKarmaValue: true);
            int linkKarma = GetKarmaValue();
            bool hasUnreadMail = _random.Next(1, 100) > 50;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(userName: userName, commentKarma: commentKarma, linkKarma: linkKarma, hasUnreadMail: hasUnreadMail);

            IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser> sut = CreateSut();

            ObjectViewModel<IRedditAuthenticatedUser> result = await sut.BuildAsync(redditAuthenticatedUser);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(userName, commentKarma, linkKarma, hasUnreadMail), result.Html);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditAuthenticatedUserWithoutCommentKarma_ReturnsInitializedObjectViewModelWithCorrectHtml()
        {
            string userName = Guid.NewGuid().ToString("D");
            int commentKarma = GetKarmaValue(hasKarmaValue: false);
            int linkKarma = GetKarmaValue();
            bool hasUnreadMail = _random.Next(1, 100) > 50;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(userName: userName, commentKarma: commentKarma, linkKarma: linkKarma, hasUnreadMail: hasUnreadMail);

            IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser> sut = CreateSut();

            ObjectViewModel<IRedditAuthenticatedUser> result = await sut.BuildAsync(redditAuthenticatedUser);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(userName, commentKarma, linkKarma, hasUnreadMail), result.Html);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditAuthenticatedUserWithLinkKarma_ReturnsInitializedObjectViewModelWithCorrectHtml()
        {
            string userName = Guid.NewGuid().ToString("D");
            int commentKarma = GetKarmaValue();
            int linkKarma = GetKarmaValue(hasKarmaValue: true);
            bool hasUnreadMail = _random.Next(1, 100) > 50;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(userName: userName, commentKarma: commentKarma, linkKarma: linkKarma, hasUnreadMail: hasUnreadMail);

            IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser> sut = CreateSut();

            ObjectViewModel<IRedditAuthenticatedUser> result = await sut.BuildAsync(redditAuthenticatedUser);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(userName, commentKarma, linkKarma, hasUnreadMail), result.Html);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditAuthenticatedUserWithoutLinkKarma_ReturnsInitializedObjectViewModelWithCorrectHtml()
        {
            string userName = Guid.NewGuid().ToString("D");
            int commentKarma = GetKarmaValue();
            int linkKarma = GetKarmaValue(hasKarmaValue: false);
            bool hasUnreadMail = _random.Next(1, 100) > 50;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(userName: userName, commentKarma: commentKarma, linkKarma: linkKarma, hasUnreadMail: hasUnreadMail);

            IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser> sut = CreateSut();

            ObjectViewModel<IRedditAuthenticatedUser> result = await sut.BuildAsync(redditAuthenticatedUser);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(userName, commentKarma, linkKarma, hasUnreadMail), result.Html);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditAuthenticatedUserWithUnreadMail_ReturnsInitializedObjectViewModelWithCorrectHtml()
        {
            string userName = Guid.NewGuid().ToString("D");
            int commentKarma = GetKarmaValue();
            int linkKarma = GetKarmaValue();
            const bool hasUnreadMail = true;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(userName: userName, commentKarma: commentKarma, linkKarma: linkKarma, hasUnreadMail: hasUnreadMail);

            IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser> sut = CreateSut();

            ObjectViewModel<IRedditAuthenticatedUser> result = await sut.BuildAsync(redditAuthenticatedUser);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(userName, commentKarma, linkKarma, hasUnreadMail), result.Html);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithRedditAuthenticatedUserWithoutUnreadMail_ReturnsInitializedObjectViewModelWithCorrectHtml()
        {
            string userName = Guid.NewGuid().ToString("D");
            int commentKarma = GetKarmaValue();
            int linkKarma = GetKarmaValue();
            const bool hasUnreadMail = false;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(userName: userName, commentKarma: commentKarma, linkKarma: linkKarma, hasUnreadMail: hasUnreadMail);

            IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser> sut = CreateSut();

            ObjectViewModel<IRedditAuthenticatedUser> result = await sut.BuildAsync(redditAuthenticatedUser);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(userName, commentKarma, linkKarma, hasUnreadMail), result.Html);
        }

        private IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser> CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Factories.RedditAuthenticatedUserToObjectViewModelBuilder();
        }

        private IRedditAuthenticatedUser CreateRedditAuthenticatedUser(string identifier = null, DateTime? createdTime = null, string userName = null, int? commentKarma = null, int? linkKarma = null, bool? hasUnreadMail = null)
        {
            return CreateRedditAuthenticatedUserMock(identifier, createdTime, userName, commentKarma, linkKarma, hasUnreadMail).Object;
        }

        private Mock<IRedditAuthenticatedUser> CreateRedditAuthenticatedUserMock(string identifier = null, DateTime? createdTime = null, string userName = null, int? commentKarma = null, int? linkKarma = null, bool? hasUnreadMail = null)
        {
            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = new Mock<IRedditAuthenticatedUser>();
            redditAuthenticatedUserMock.Setup(m => m.Identifier)
                .Returns(identifier);
            redditAuthenticatedUserMock.Setup(m => m.CreatedTime)
                .Returns(createdTime ?? DateTime.Now.AddDays(_random.Next(1, 365) * -1).AddMinutes(_random.Next(-120, 120)));
            redditAuthenticatedUserMock.Setup(m => m.UserName)
                .Returns(userName);
            redditAuthenticatedUserMock.Setup(m => m.CommentKarma)
                .Returns(commentKarma ?? GetKarmaValue());
            redditAuthenticatedUserMock.Setup(m => m.LinkKarma)
                .Returns(linkKarma ?? GetKarmaValue());
            redditAuthenticatedUserMock.Setup(m => m.HasUnreadMail)
                .Returns(hasUnreadMail ?? _random.Next(1, 100) > 50);
            return redditAuthenticatedUserMock;
        }

        private int GetKarmaValue(bool? hasKarmaValue = null)
        {
            return (hasKarmaValue ?? _random.Next(1, 100) > 50) ? _random.Next(1, 100) : 0;
        }

        private string GetExpectedHtml(string userName, int commentKarma, int linkKarma, bool hasUnreadMail)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            StringBuilder htmlBuilder = new StringBuilder("<div class=\"card h-100\">");
            htmlBuilder.Append("<img class=\"card-img-top img-fluid\" src=\"/images/reddit_logo_and_wordmark.png\" />");
            htmlBuilder.Append("<div class=\"card-body\">");
            htmlBuilder.Append($"<h5 class=\"card-title\">{userName}</h5>");
            if (commentKarma > 0)
            {
                htmlBuilder.Append($"<p class=\"card-text\">Comment karma: {commentKarma}</p>");
            }
            if (linkKarma > 0)
            {
                htmlBuilder.Append($"<p class=\"card-text\">Link karma: {linkKarma}</p>");
            }
            if (hasUnreadMail)
            {
                htmlBuilder.Append("<p class=\"card-text\">You have unread mail</p>");
            }
            htmlBuilder.Append("</div>");
            htmlBuilder.Append("</div>");

            return htmlBuilder.ToString();
        }
    }
}
