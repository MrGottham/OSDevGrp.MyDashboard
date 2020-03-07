using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GetNsfwSubredditsAsyncTests
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
        public async Task GetNsfwSubredditsAsync_WhenRedditAccessTokenIsNull_ThrowsArgumentNullException()
        {
            const IRedditAccessToken accessToken = null;
            int numberOfSubreddits = _random.Next(1, 10);

            IRedditLogic sut = CreateSut();

            await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalled_AssertGetKnownNsfwSubredditsAsyncWasCalledOnDataProviderFactory()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            IRedditLogic sut = CreateSut();

            await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            _dataProviderFactoryMock.Verify(m => m.GetKnownNsfwSubredditsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalled_AssertRankWasCalledOnEachKnownNsfwSubreddit()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 5);

            Mock<IRedditKnownSubreddit> knownNsfwSubreddit1 = CreateRedditKnownSubredditMock();
            Mock<IRedditKnownSubreddit> knownNsfwSubreddit2 = CreateRedditKnownSubredditMock();
            Mock<IRedditKnownSubreddit> knownNsfwSubreddit3 = CreateRedditKnownSubredditMock();
            Mock<IRedditKnownSubreddit> knownNsfwSubreddit4 = CreateRedditKnownSubredditMock();
            Mock<IRedditKnownSubreddit> knownNsfwSubreddit5 = CreateRedditKnownSubredditMock();
            IEnumerable<IRedditKnownSubreddit> knownNsfwSubredditCollection = new List<IRedditKnownSubreddit>
            {
                knownNsfwSubreddit1.Object,
                knownNsfwSubreddit2.Object,
                knownNsfwSubreddit3.Object,
                knownNsfwSubreddit4.Object,
                knownNsfwSubreddit5.Object
            };
            IRedditLogic sut = CreateSut(knownNsfwSubredditCollection: knownNsfwSubredditCollection);

            await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            knownNsfwSubreddit1.Verify(m => m.Rank, Times.Once);
            knownNsfwSubreddit2.Verify(m => m.Rank, Times.Once);
            knownNsfwSubreddit3.Verify(m => m.Rank, Times.Once);
            knownNsfwSubreddit4.Verify(m => m.Rank, Times.Once);
            knownNsfwSubreddit5.Verify(m => m.Rank, Times.Once);
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalled_AssertNameWasCalledOnEachKnownNsfwSubreddit()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 5);

            Mock<IRedditKnownSubreddit> knownNsfwSubreddit1 = CreateRedditKnownSubredditMock();
            Mock<IRedditKnownSubreddit> knownNsfwSubreddit2 = CreateRedditKnownSubredditMock();
            Mock<IRedditKnownSubreddit> knownNsfwSubreddit3 = CreateRedditKnownSubredditMock();
            Mock<IRedditKnownSubreddit> knownNsfwSubreddit4 = CreateRedditKnownSubredditMock();
            Mock<IRedditKnownSubreddit> knownNsfwSubreddit5 = CreateRedditKnownSubredditMock();
            IEnumerable<IRedditKnownSubreddit> knownNsfwSubredditCollection = new List<IRedditKnownSubreddit>
            {
                knownNsfwSubreddit1.Object,
                knownNsfwSubreddit2.Object,
                knownNsfwSubreddit3.Object,
                knownNsfwSubreddit4.Object,
                knownNsfwSubreddit5.Object
            };
            IRedditLogic sut = CreateSut(knownNsfwSubredditCollection: knownNsfwSubredditCollection);

            await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            knownNsfwSubreddit1.Verify(m => m.Name, Times.Once);
            knownNsfwSubreddit2.Verify(m => m.Name, Times.Once);
            knownNsfwSubreddit3.Verify(m => m.Name, Times.Once);
            knownNsfwSubreddit4.Verify(m => m.Name, Times.Once);
            knownNsfwSubreddit5.Verify(m => m.Name, Times.Once);
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalled_AssertWillExceedRateLimitWasCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            int numberOfKnownNsfwSubreddits = _random.Next(5, 10);
            int numberOfSubredditsToGet = Math.Min(numberOfSubreddits, numberOfKnownNsfwSubreddits);
            IEnumerable<IRedditKnownSubreddit> knownNsfwSubredditCollection = CreateRedditKnownSubredditCollection(numberOfSubreddits: numberOfKnownNsfwSubreddits);
            IRedditLogic sut = CreateSut(knownNsfwSubredditCollection: knownNsfwSubredditCollection);

            await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            _redditRateLimitLogicMock.Verify(m => m.WillExceedRateLimit(It.Is<int>(value => value == numberOfSubredditsToGet)), Times.Exactly(numberOfSubredditsToGet > 1 ? 1 : 2));
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertWillExceedRateLimitWasCalledOnlyOnceOnRedditRateLimitLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            _redditRateLimitLogicMock.Verify(m => m.WillExceedRateLimit(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertGetSpecificSubredditAsyncWasNotCalledOnRedditRepository()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            _redditRepositoryMock.Verify(m => m.GetSpecificSubredditAsync(
                    It.IsAny<IRedditAccessToken>(),
                    It.IsAny<IRedditKnownSubreddit>()),
                Times.Never);
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertEnforceRateLimitAsyncWasNotCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            _redditRateLimitLogicMock.Verify(m => m.EnforceRateLimitAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime>()),
                Times.Never);
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasExceeded_ReturnsEmptySubredditCollection()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            IEnumerable<IRedditSubreddit> result = await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertWillExceedRateLimitWasCalledOnRedditRateLimitLogicForEachKnownNsfwSubredditToGet()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            int numberOfKnownNsfwSubreddits = _random.Next(5, 10);
            int numberOfSubredditsToGet = Math.Min(numberOfSubreddits, numberOfKnownNsfwSubreddits);
            IEnumerable<IRedditKnownSubreddit> knownNsfwSubredditCollection = CreateRedditKnownSubredditCollection(numberOfSubreddits: numberOfKnownNsfwSubreddits);
            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(knownNsfwSubredditCollection: knownNsfwSubredditCollection, willExceedRateLimit: willExceedRateLimit);

            await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            _redditRateLimitLogicMock.Verify(m => m.WillExceedRateLimit(It.Is<int>(value => value == 1)), Times.Exactly(numberOfSubredditsToGet + Convert.ToInt32(numberOfSubredditsToGet == 1)));
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertGetSpecificSubredditAsyncWasCalledOnRedditRepositoryForEachKnownNsfwSubredditToGet()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            int numberOfKnownNsfwSubreddits = _random.Next(5, 10);
            int numberOfSubredditsToGet = Math.Min(numberOfSubreddits, numberOfKnownNsfwSubreddits);
            IEnumerable<IRedditKnownSubreddit> knownNsfwSubredditCollection = CreateRedditKnownSubredditCollection(numberOfSubreddits: numberOfKnownNsfwSubreddits);
            IEnumerable<IRedditKnownSubreddit> knownNsfwSubredditToGetCollection = knownNsfwSubredditCollection
                .OrderBy(m => m.Rank)
                .ThenBy(m => m.Name)
                .Take(numberOfSubredditsToGet)
                .ToList();
            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(knownNsfwSubredditCollection: knownNsfwSubredditCollection, willExceedRateLimit: willExceedRateLimit);

            await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            _redditRepositoryMock.Verify(m => m.GetSpecificSubredditAsync(
                    It.Is<IRedditAccessToken>(value => value == accessToken),
                    It.Is<IRedditKnownSubreddit>(value => knownNsfwSubredditToGetCollection.Contains(value))),
                Times.Exactly(numberOfSubredditsToGet));
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertEnforceRateLimitAsyncWasCalledOnRedditRateLimitLogicForEachKnownNsfwSubredditToGet()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            int numberOfKnownNsfwSubreddits = _random.Next(5, 10);
            int numberOfSubredditsToGet = Math.Min(numberOfSubreddits, numberOfKnownNsfwSubreddits);
            IEnumerable<IRedditKnownSubreddit> knownNsfwSubredditCollection = CreateRedditKnownSubredditCollection(numberOfSubreddits: numberOfKnownNsfwSubreddits);
            const bool willExceedRateLimit = false;
            int rateLimitUsed = _random.Next(1, 60);
            int rateLimitRemaining = _random.Next(1, 60);
            DateTime rateLimitResetTime = DateTime.Now.AddSeconds(_random.Next(90, 300));
            DateTime receivedTime = DateTime.Now.AddSeconds(_random.Next(1, 10) * -1);
            IRedditResponse<IRedditSubreddit> redditResponse = CreateRedditResponse(rateLimitUsed, rateLimitRemaining, rateLimitResetTime, receivedTime);
            IRedditLogic sut = CreateSut(knownNsfwSubredditCollection: knownNsfwSubredditCollection, willExceedRateLimit: willExceedRateLimit, redditResponse: redditResponse);

            await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            _redditRateLimitLogicMock.Verify(m => m.EnforceRateLimitAsync(
                    It.Is<int>(value => value == rateLimitUsed),
                    It.Is<int>(value => value == rateLimitRemaining),
                    It.Is<DateTime?>(value => value.HasValue && value.Value == rateLimitResetTime),
                    It.Is<DateTime>(value => value == receivedTime)), 
                Times.Exactly(numberOfSubredditsToGet));
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasNotExceeded_ReturnsSubredditCollection()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            int numberOfKnownNsfwSubreddits = _random.Next(5, 10);
            int numberOfSubredditsToGet = Math.Min(numberOfSubreddits, numberOfKnownNsfwSubreddits);
            IEnumerable<IRedditKnownSubreddit> knownNsfwSubredditCollection = CreateRedditKnownSubredditCollection(numberOfSubreddits: numberOfKnownNsfwSubreddits);
            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(knownNsfwSubredditCollection: knownNsfwSubredditCollection, willExceedRateLimit: willExceedRateLimit);

            IEnumerable<IRedditSubreddit> result = await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            Assert.IsNotNull(result);
            Assert.AreEqual(numberOfSubredditsToGet, result.Count());
            Assert.IsTrue(result.All(subreddit => subreddit != null));
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasNotExceededAndAggregateExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            const bool willExceedRateLimit = false;
            AggregateException aggregateException = new AggregateException();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: aggregateException);

            await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<AggregateException>(value => value == aggregateException)), Times.Once);
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasNotExceededAndAggregateExceptionOccurs_ReturnsEmptySubredditCollection()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            const bool willExceedRateLimit = false;
            AggregateException aggregateException = new AggregateException();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: aggregateException);

            IEnumerable<IRedditSubreddit> result = await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasNotExceededAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            const bool willExceedRateLimit = false;
            Exception exception = new Exception();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: exception);

            await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(value => value == exception)), Times.Once);
        }

        [TestMethod]
        public async Task GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasNotExceededAndExceptionOccurs_ReturnsEmptySubredditCollection()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            const bool willExceedRateLimit = false;
            Exception exception = new Exception();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: exception);

            IEnumerable<IRedditSubreddit> result = await sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        private IRedditLogic CreateSut(IEnumerable<IRedditKnownSubreddit> knownNsfwSubredditCollection = null, bool willExceedRateLimit = false, IRedditResponse<IRedditSubreddit> redditResponse = null, Exception exception = null)
        {
            if (exception != null)
            {
                _dataProviderFactoryMock.Setup(m => m.GetKnownNsfwSubredditsAsync())
                    .Throws(exception);
            }
            else
            {
                _dataProviderFactoryMock.Setup(m => m.GetKnownNsfwSubredditsAsync())
                    .Returns(Task.Run(() => knownNsfwSubredditCollection ?? CreateRedditKnownSubredditCollection()));
            }

            _redditRateLimitLogicMock.Setup(m => m.WillExceedRateLimit(It.IsAny<int>()))
                .Returns(willExceedRateLimit);
            _redditRateLimitLogicMock.Setup(m => m.EnforceRateLimitAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime>()))
                .Returns(Task.Run(() => { }));

            _redditRepositoryMock.Setup(m => m.GetSpecificSubredditAsync(It.IsAny<IRedditAccessToken>(), It.IsAny<IRedditKnownSubreddit>()))
                .Returns(Task.Run(() => redditResponse ?? CreateRedditResponse()));

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

        private IEnumerable<IRedditKnownSubreddit> CreateRedditKnownSubredditCollection(int? numberOfSubreddits = null)
        {
            int capacity = numberOfSubreddits ?? _random.Next(5, 10);
            IList<IRedditKnownSubreddit> redditKnownSubredditCollection = new List<IRedditKnownSubreddit>(capacity);
            while (redditKnownSubredditCollection.Count < capacity)
            {
                redditKnownSubredditCollection.Add(CreateRedditKnownSubreddit());
            }
            return redditKnownSubredditCollection;
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
            redditKnownSubredditMock.Setup(m => m.Rank)
                .Returns(_random.Next(1, 1000));
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

        private IRedditSubreddit CreateSubreddit(long? subscribers = null)
        {
            return CreateSubredditMock(subscribers).Object;
        }

        private Mock<IRedditSubreddit> CreateSubredditMock(long? subscribers = null)
        {
            Mock<IRedditSubreddit> subredditMock = new Mock<IRedditSubreddit>();
            subredditMock.Setup(m => m.Subscribers)
                .Returns(subscribers ?? _random.Next(2500, 10000));
            return subredditMock;
        }
    }
}