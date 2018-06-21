using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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

        private Mock<IExceptionHandler> _exceptionHandlerMock;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
        }

        [TestMethod]
        [ExpectedArgumentNullException("accessToken")]
        public void GetSpecificSubredditAsync_WhenAccessTokenIsNull_ThrowsArgumentNullException()
        {
            const IRedditAccessToken accessToken = null;
            IRedditKnownSubreddit knownSubreddit = CreateRedditKnownSubreddit();

            IRedditRepository sut = CreateSut();

            sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);
        }

        [TestMethod]
        [ExpectedArgumentNullException("knownSubreddit")]
        public void GetSpecificSubredditAsync_WhenKnownSubredditIsNull_ThrowsArgumentNullException()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            const IRedditKnownSubreddit knownSubreddit = null;

            IRedditRepository sut = CreateSut();

            sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);
        }

        [TestMethod]
        public void GetSpecificSubredditAsync_WhenCalled_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IRedditKnownSubreddit knownSubreddit = CreateRedditKnownSubreddit();

            IRedditRepository sut = CreateSut();

            Task<IRedditResponse<IRedditSubreddit>> getSpecificSubredditTask = sut.GetSpecificSubredditAsync(redditAccessToken, knownSubreddit);
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

        private IRedditKnownSubreddit CreateRedditKnownSubreddit()
        {
            return CreateRedditKnownSubredditMock().Object;
        }

        private Mock<IRedditKnownSubreddit> CreateRedditKnownSubredditMock()
        {
            Mock<IRedditKnownSubreddit> redditKnownSubredditMock = new Mock<IRedditKnownSubreddit>();
            redditKnownSubredditMock.Setup(m => m.Name)
                .Returns(Guid.NewGuid().ToString("D"));
            return redditKnownSubredditMock;
        }
    }
}