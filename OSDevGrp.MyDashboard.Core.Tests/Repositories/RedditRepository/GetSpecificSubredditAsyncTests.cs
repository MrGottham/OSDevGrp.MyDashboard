using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Repositories.RedditRepository
{
    [TestClass]
    public class GetSpecificSubredditAsyncTests
    {
        #region Private variables

        private Mock<IDataProviderFactory> _dataProviderFactoryMock;
        private Mock<IExceptionHandler> _exceptionHandlerMock;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _dataProviderFactoryMock = new Mock<IDataProviderFactory>();
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
        }

        [TestMethod]
        [ExpectedArgumentNullException("accessToken")]
        public void GetSpecificSubredditAsync_WhenAccessTokenIsNull_ThrowsArgumentNullException()
        {
            const IRedditAccessToken accessToken = null;
            string subreddit = Guid.NewGuid().ToString("D");

            IRedditRepository sut = CreateSut();

            sut.GetSpecificSubredditAsync(accessToken, subreddit);
        }

        [TestMethod]
        [ExpectedArgumentNullException("subreddit")]
        public void GetSpecificSubredditAsync_WhenSubredditIsNull_ThrowsArgumentNullException()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            const string subreddit = null;

            IRedditRepository sut = CreateSut();

            sut.GetSpecificSubredditAsync(accessToken, subreddit);
        }

        [TestMethod]
        [ExpectedArgumentNullException("subreddit")]
        public void GetSpecificSubredditAsync_WhenSubredditIsWhiteSpace_ThrowsArgumentNullException()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            const string subreddit = " ";

            IRedditRepository sut = CreateSut();

            sut.GetSpecificSubredditAsync(accessToken, subreddit);
        }

        [TestMethod]
        [ExpectedArgumentNullException("subreddit")]
        public void GetSpecificSubredditAsync_WhenSubredditIsWhiteSpaces_ThrowsArgumentNullException()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            const string subreddit = "  ";

            IRedditRepository sut = CreateSut();

            sut.GetSpecificSubredditAsync(accessToken, subreddit);
        }

        public void GetSpecificSubredditAsync_WhenSubredditIsEmpty_ThrowsArgumentNullException()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            string subreddit = string.Empty;

            IRedditRepository sut = CreateSut();

            sut.GetSpecificSubredditAsync(accessToken, subreddit);
        }

        [TestMethod]
        public void GetSpecificSubredditAsync_WhenCalled_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            IRedditRepository sut = CreateSut();
            string subreddit = Guid.NewGuid().ToString("D");

            Task<IRedditResponse<IRedditSubreddit>> getSpecificSubredditTask = sut.GetSpecificSubredditAsync(redditAccessToken, subreddit);
            getSpecificSubredditTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<AggregateException>(aggregateException => 
                    aggregateException != null &&
                    aggregateException.InnerException != null &&
                    aggregateException.InnerException.GetType() == typeof(UnauthorizedAccessException))),
                Times.Once());
        }

        private IRedditRepository CreateSut()
        {
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<AggregateException>()))
                .Returns(Task.Run(() => { }));
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.Run(() => { }));

            return new OSDevGrp.MyDashboard.Core.Repositories.RedditRepository(
                _dataProviderFactoryMock.Object,
                _exceptionHandlerMock.Object);
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
    }
}