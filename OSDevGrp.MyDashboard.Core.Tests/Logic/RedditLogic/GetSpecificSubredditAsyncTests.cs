using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Logic.RedditLogic
{
    [TestClass]
    public class GetSpecificSubredditAsyncTests
    {
        #region Private variables

        private Mock<IDataProviderFactory> _dataProviderFactoryMock;
        private Mock<IRedditAccessTokenProviderFactory> _redditAccessTokenProviderFactoryMock;
        private Mock<IRedditRepository> _redditRepositoryMock;
        private Mock<IRedditRateLimitLogic> _redditRateLimitLogicMock;
        private Mock<IRedditFilterLogic> _redditFilterLogicMock;
        private Mock<IExceptionHandler> _exceptionHandlerMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _dataProviderFactoryMock = new Mock<IDataProviderFactory>();
            _redditAccessTokenProviderFactoryMock = new Mock<IRedditAccessTokenProviderFactory>();
            _redditRepositoryMock = new Mock<IRedditRepository>();
            _redditRateLimitLogicMock = new Mock<IRedditRateLimitLogic>();
            _redditFilterLogicMock = new Mock<IRedditFilterLogic>();
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullException("accessToken")]
        public async Task GetSpecificSubredditAsync_WhenRedditAccessTokenIsNull_ThrowsArgumentNullException()
        {
            const IRedditAccessToken accessToken = null;
            IRedditKnownSubreddit knownSubreddit = CreateRedditKnownSubreddit();

            IRedditLogic sut = CreateSut();

            await sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);
        }

        [TestMethod]
        [ExpectedArgumentNullException("knownSubreddit")]
        public async Task GetSpecificSubredditAsync_WhenRedditKnownSubredditIsNull_ThrowsArgumentNullException()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            const IRedditKnownSubreddit knownSubreddit = null;

            IRedditLogic sut = CreateSut();

            await sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);
        }

        [TestMethod]
        public async Task GetSpecificSubredditAsync_WhenCalled_AssertWillExceedRateLimitWasCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditKnownSubreddit knownSubreddit = CreateRedditKnownSubreddit();

            IRedditLogic sut = CreateSut();

            await sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);

            _redditRateLimitLogicMock.Verify(m => m.WillExceedRateLimit(It.Is<int>(value => value == 1)), Times.Once);
        }

        [TestMethod]
        public async Task GetSpecificSubredditAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertGetSpecificSubredditAsyncWasNotCalledOnRedditRepository()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditKnownSubreddit knownSubreddit = CreateRedditKnownSubreddit();

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);

            _redditRepositoryMock.Verify(m => m.GetSpecificSubredditAsync(
                    It.IsAny<IRedditAccessToken>(),
                    It.IsAny<IRedditKnownSubreddit>()),
                Times.Never);
        }

        [TestMethod]
        public async Task GetSpecificSubredditAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertEnforceRateLimitAsyncWasNotCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditKnownSubreddit knownSubreddit = CreateRedditKnownSubreddit();

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);

            _redditRateLimitLogicMock.Verify(m => m.EnforceRateLimitAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime>()),
                Times.Never);
        }

        [TestMethod]
        public async Task GetSpecificSubredditAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditKnownSubreddit knownSubreddit = CreateRedditKnownSubreddit();

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public async Task GetSpecificSubredditAsync_WhenCalledAndRedditRateLimitHasExceeded_ReturnsNull()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditKnownSubreddit knownSubreddit = CreateRedditKnownSubreddit();

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            IRedditSubreddit result = await sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetSpecificSubredditAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertGetSpecificSubredditAsyncWasCalledOnRedditRepository()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditKnownSubreddit knownSubreddit = CreateRedditKnownSubreddit();

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);

            _redditRepositoryMock.Verify(m => m.GetSpecificSubredditAsync(
                    It.Is<IRedditAccessToken>(value => value == accessToken),
                    It.Is<IRedditKnownSubreddit>(value => value == knownSubreddit)),
                Times.Once);
        }

        [TestMethod]
        public async Task GetSpecificSubredditAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertEnforceRateLimitAsyncWasCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditKnownSubreddit knownSubreddit = CreateRedditKnownSubreddit();

            const bool willExceedRateLimit = false;
            int rateLimitUsed = _random.Next(1, 60);
            int rateLimitRemaining = _random.Next(1, 60);
            DateTime rateLimitResetTime = DateTime.Now.AddSeconds(_random.Next(90, 300));
            DateTime receivedTime = DateTime.Now.AddSeconds(_random.Next(1, 10) * -1);
            IRedditResponse<IRedditSubreddit> redditResponse = CreateRedditResponse(rateLimitUsed, rateLimitRemaining, rateLimitResetTime, receivedTime);
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, redditResponse: redditResponse);

            await sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);

            _redditRateLimitLogicMock.Verify(m => m.EnforceRateLimitAsync(
                    It.Is<int>(value => value == rateLimitUsed),
                    It.Is<int>(value => value == rateLimitRemaining),
                    It.Is<DateTime?>(value => value.HasValue && value.Value == rateLimitResetTime),
                    It.Is<DateTime>(value => value == receivedTime)), 
                Times.Once);
        }

        [TestMethod]
        public async Task GetSpecificSubredditAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditKnownSubreddit knownSubreddit = CreateRedditKnownSubreddit();

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public async Task GetSpecificSubredditAsync_WhenCalledAndRedditRateLimitHasNotExceeded_ReturnsSubreddit()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditKnownSubreddit knownSubreddit = CreateRedditKnownSubreddit();

            const bool willExceedRateLimit = false;
            IRedditSubreddit subreddit = CreateSubreddit();
            IRedditResponse<IRedditSubreddit> redditResponse = CreateRedditResponse(subreddit: subreddit);
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, redditResponse: redditResponse);

            IRedditSubreddit result = await sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);

            Assert.IsNotNull(result);
            Assert.AreEqual(subreddit, result);
        }

        [TestMethod]
        public async Task GetSpecificSubredditAsync_WhenCalledAndRedditRateLimitHasNotExceededAndAggregateExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditKnownSubreddit knownSubreddit = CreateRedditKnownSubreddit();

            const bool willExceedRateLimit = false;
            AggregateException aggregateException = new AggregateException();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: aggregateException);

            await sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<AggregateException>(value => value == aggregateException)), Times.Once);
        }

        [TestMethod]
        public async Task GetSpecificSubredditAsync_WhenCalledAndRedditRateLimitHasNotExceededAndAggregateExceptionOccurs_ReturnsNull()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditKnownSubreddit knownSubreddit = CreateRedditKnownSubreddit();

            const bool willExceedRateLimit = false;
            AggregateException aggregateException = new AggregateException();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: aggregateException);

            IRedditSubreddit result = await sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetSpecificSubredditAsync_WhenCalledAndRedditRateLimitHasNotExceededAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditKnownSubreddit knownSubreddit = CreateRedditKnownSubreddit();

            const bool willExceedRateLimit = false;
            Exception exception = new Exception();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: exception);

            await sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(value => value == exception)), Times.Once);
        }

        [TestMethod]
        public async Task GetSpecificSubredditAsync_WhenCalledAndRedditRateLimitHasNotExceededAndExceptionOccurs_ReturnsNull()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditKnownSubreddit knownSubreddit = CreateRedditKnownSubreddit();

            const bool willExceedRateLimit = false;
            Exception exception = new Exception();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: exception);

            IRedditSubreddit result = await sut.GetSpecificSubredditAsync(accessToken, knownSubreddit);

            Assert.IsNull(result);
        }

        private IRedditLogic CreateSut(bool willExceedRateLimit = false, IRedditResponse<IRedditSubreddit> redditResponse = null, Exception exception = null)
        {
            _redditRateLimitLogicMock.Setup(m => m.WillExceedRateLimit(It.IsAny<int>()))
                .Returns(willExceedRateLimit);
            _redditRateLimitLogicMock.Setup(m => m.EnforceRateLimitAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime>()))
                .Returns(Task.Run(() => { }));

            if (exception != null)
            {
                _redditRepositoryMock.Setup(m => m.GetSpecificSubredditAsync(It.IsAny<IRedditAccessToken>(), It.IsAny<IRedditKnownSubreddit>()))
                    .Throws(exception);
            }
            else
            {
                _redditRepositoryMock.Setup(m => m.GetSpecificSubredditAsync(It.IsAny<IRedditAccessToken>(), It.IsAny<IRedditKnownSubreddit>()))
                    .Returns(Task.Run(() => redditResponse ?? CreateRedditResponse()));
            }

            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<AggregateException>()))
                .Returns(Task.Run(() => { }));
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.Run(() => { }));

            return new OSDevGrp.MyDashboard.Core.Logic.RedditLogic(
                _dataProviderFactoryMock.Object,
                _redditAccessTokenProviderFactoryMock.Object,
                _redditRepositoryMock.Object,
                _redditRateLimitLogicMock.Object,
                _redditFilterLogicMock.Object,
                _exceptionHandlerMock.Object);
        }

        private IRedditAccessToken CreateRedditAccessToken()
        {
            return CreateRedditAccessTokenMock().Object;
        }

        private Mock<IRedditAccessToken> CreateRedditAccessTokenMock()
        {
            Mock<IRedditAccessToken> redditAccessTokenMock = new Mock<IRedditAccessToken>();
            return redditAccessTokenMock; 
        }

        private IRedditKnownSubreddit CreateRedditKnownSubreddit()
        {
            return CreateRedditKnownSubredditMock().Object; 
        }

        private Mock<IRedditKnownSubreddit> CreateRedditKnownSubredditMock()
        {
            Mock<IRedditKnownSubreddit> redditKnownSubredditMock = new Mock<IRedditKnownSubreddit>();
            return redditKnownSubredditMock; 
        }
 
        private IRedditResponse<IRedditSubreddit> CreateRedditResponse(int? rateLimitUsed = null, int? rateLimitRemaining = null, DateTime? rateLimitResetTime = null, DateTime? receivedTime = null, IRedditSubreddit subreddit = null)
        {
            return CreateRedditResponseMock(rateLimitUsed, rateLimitRemaining, rateLimitResetTime, receivedTime, subreddit).Object;
        }

        private Mock<IRedditResponse<IRedditSubreddit>> CreateRedditResponseMock(int? rateLimitUsed = null, int? rateLimitRemaining = null, DateTime? rateLimitResetTime = null, DateTime? receivedTime = null, IRedditSubreddit subreddit = null)
        {
            Mock<IRedditResponse<IRedditSubreddit>> redditResponseMock = new Mock<IRedditResponse<IRedditSubreddit>>();
            redditResponseMock.Setup(m => m.RateLimitUsed)
                .Returns(rateLimitUsed ?? _random.Next(1, 60));
            redditResponseMock.Setup(m => m.RateLimitRemaining)
                .Returns(rateLimitRemaining ?? _random.Next(1, 60));
            redditResponseMock.Setup(m => m.RateLimitResetTime)
                .Returns(rateLimitResetTime ?? DateTime.Now.AddSeconds(_random.Next(30, 60)));
            redditResponseMock.Setup(m => m.ReceivedTime)
                .Returns(receivedTime ?? DateTime.Now.AddSeconds(_random.Next(1, 10) * -1));
            redditResponseMock.Setup(m => m.Data)
                .Returns(subreddit ?? CreateSubreddit());
            return redditResponseMock;
        }

        private IRedditSubreddit CreateSubreddit()
        {
            return CreateSubredditMock().Object;
        }

        private Mock<IRedditSubreddit> CreateSubredditMock()
        {
            Mock<IRedditSubreddit> subredditMock = new Mock<IRedditSubreddit>();
            return subredditMock;
        }
    }
}