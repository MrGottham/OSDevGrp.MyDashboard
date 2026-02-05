using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;

namespace OSDevGrp.MyDashboard.Core.Tests.Repositories.RedditRepository
{
    [TestClass]
    public class GetLinksAsyncTests
    {
        #region Private variables

        private Mock<IExceptionHandler> _exceptionHandlerMock;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenAccessTokenIsNull_ThrowsArgumentNullException()
        {
            const IRedditAccessToken accessToken = null;
            IRedditSubreddit subreddit = CreateRedditSubreddit();

            IRedditRepository sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetLinksAsync(accessToken, subreddit));

            Assert.AreEqual("accessToken", result.ParamName);
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenSubredditIsNull_ThrowsArgumentNullException()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            const IRedditSubreddit subreddit = null;

            IRedditRepository sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetLinksAsync(accessToken, subreddit));

            Assert.AreEqual("subreddit", result.ParamName);
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalled_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateRedditSubreddit();

            IRedditRepository sut = CreateSut();

            await sut.GetLinksAsync(redditAccessToken, subreddit);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(exception => 
                    exception != null &&
                    exception.InnerException == null &&
                    exception.GetType() == typeof(UnauthorizedAccessException))),
                Times.Once());
        }

        private IRedditRepository CreateSut()
        {
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<AggregateException>()))
                .Returns(Task.Run(() => { }));
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.Run(() => { }));

            return new OSDevGrp.MyDashboard.Core.Repositories.RedditRepository(_exceptionHandlerMock.Object);
        }
 
        private IRedditAccessToken CreateRedditAccessToken()
        {
            return CreateRedditAccessTokenMock().Object;
        }

        private Mock<IRedditAccessToken> CreateRedditAccessTokenMock()
        {
            Mock<IRedditAccessToken> redditAccessTokenMock = new Mock<IRedditAccessToken>();
            redditAccessTokenMock.Setup(m => m.TokenType)
                .Returns("bearer");
            redditAccessTokenMock.Setup(m => m.AccessToken)
                .Returns(Guid.NewGuid().ToString("D"));
            return redditAccessTokenMock;
        }

        private IRedditSubreddit CreateRedditSubreddit()
        {
            return CreateRedditSubredditMock().Object;
        }

        private Mock<IRedditSubreddit> CreateRedditSubredditMock()
        {
            Mock<IRedditSubreddit> redditSubredditMock = new Mock<IRedditSubreddit>();
            redditSubredditMock.Setup(m => m.Url)
                .Returns(new Uri($"https://localhost/{Guid.NewGuid().ToString("D")}"));
            return redditSubredditMock;
        }
    }
}