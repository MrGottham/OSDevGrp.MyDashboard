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
        public void BuildAsync_WhenRedditSubredditIsNull_ThrowsArgumentNullException()
        {
            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            sut.BuildAsync(null);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertIdentifierWasCalledOnRedditSubreddit()
        {
            Mock<IRedditSubreddit> redditSubredditMock = CreateRedditSubredditMock();

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            Task<ObjectViewModel<IRedditSubreddit>> buildTask = sut.BuildAsync(redditSubredditMock.Object);
            buildTask.Wait();

            redditSubredditMock.Verify(m => m.Identifier, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertCreatedTimeWasCalledOnRedditSubreddit()
        {
            Mock<IRedditSubreddit> redditSubredditMock = CreateRedditSubredditMock();

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            Task<ObjectViewModel<IRedditSubreddit>> buildTask = sut.BuildAsync(redditSubredditMock.Object);
            buildTask.Wait();

            redditSubredditMock.Verify(m => m.CreatedTime, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertDisplayNamePrefixedWasCalledOnRedditSubreddit()
        {
            Mock<IRedditSubreddit> redditSubredditMock = CreateRedditSubredditMock();

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            Task<ObjectViewModel<IRedditSubreddit>> buildTask = sut.BuildAsync(redditSubredditMock.Object);
            buildTask.Wait();

            redditSubredditMock.Verify(m => m.DisplayNamePrefixed, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_ReturnsInitializedObjectViewModel()
        {
            string identifier = Guid.NewGuid().ToString("D");
            DateTime createdTime = DateTime.Now.AddDays(_random.Next(1, 365) * -1).AddMinutes(_random.Next(-120, 120));
            string displayNamePrefixed = Guid.NewGuid().ToString("D");
            IRedditSubreddit redditSubreddit = CreateRedditSubreddit(identifier, createdTime, displayNamePrefixed);

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            Task<ObjectViewModel<IRedditSubreddit>> buildTask = sut.BuildAsync(redditSubreddit);
            buildTask.Wait();

            ObjectViewModel<IRedditSubreddit> result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ObjectIdentifier);
            Assert.AreEqual(identifier, result.ObjectIdentifier);
            Assert.IsNotNull(result.Object);
            Assert.AreEqual(redditSubreddit, result.Object);
            Assert.AreEqual(createdTime, result.Timestamp);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(displayNamePrefixed), result.Html);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_ReturnsInitializedObjectViewModelWithCorrectHtml()
        {
            string displayNamePrefixed = Guid.NewGuid().ToString("D");
            IRedditSubreddit redditSubreddit = CreateRedditSubreddit(displayNamePrefixed: displayNamePrefixed);

            IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> sut = CreateSut();

            Task<ObjectViewModel<IRedditSubreddit>> buildTask = sut.BuildAsync(redditSubreddit);
            buildTask.Wait();

            ObjectViewModel<IRedditSubreddit> result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Html);
            Assert.AreEqual(GetExpectedHtml(displayNamePrefixed), result.Html);
        }

        private IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Factories.RedditSubredditToObjectViewModelBuilder();
        }

        private IRedditSubreddit CreateRedditSubreddit(string identifier = null, DateTime? createdTime = null, string displayNamePrefixed = null)
        {
            return CreateRedditSubredditMock(identifier, createdTime, displayNamePrefixed).Object;
        }

        private Mock<IRedditSubreddit> CreateRedditSubredditMock(string identifier = null, DateTime? createdTime = null, string displayNamePrefixed = null)
        {
            Mock<IRedditSubreddit> redditSubredditMock = new Mock<IRedditSubreddit>();
            redditSubredditMock.Setup(m => m.Identifier)
                .Returns(identifier);
            redditSubredditMock.Setup(m => m.CreatedTime)
                .Returns(createdTime ?? DateTime.Now.AddDays(_random.Next(1, 365) * -1).AddMinutes(_random.Next(-120, 120)));
            redditSubredditMock.Setup(m => m.DisplayNamePrefixed)
                .Returns(displayNamePrefixed ?? Guid.NewGuid().ToString("D"));
            return redditSubredditMock;
        }
 
        private string GetExpectedHtml(string displayNamePrefixed)
        {
            if (string.IsNullOrWhiteSpace(displayNamePrefixed))
            {
                throw new ArgumentNullException(nameof(displayNamePrefixed));
            }

            StringBuilder htmlBuilder = new StringBuilder("<div>");
            htmlBuilder.Append($"<h5>{displayNamePrefixed}</h5>");
            htmlBuilder.Append("</div>");

            return htmlBuilder.ToString();
        }
    }
}