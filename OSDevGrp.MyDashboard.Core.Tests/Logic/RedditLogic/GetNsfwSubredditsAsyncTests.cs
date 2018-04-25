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
            _redditRepositoryMock = new Mock<IRedditRepository>();
            _redditRateLimitLogicMock = new Mock<IRedditRateLimitLogic>();
            _redditFilterLogicMock = new Mock<IRedditFilterLogic>();
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullException("accessToken")]
        public void GetNsfwSubredditsAsync_WhenRedditAccessTokenIsNull_ThrowsArgumentNullException()
        {
            const IRedditAccessToken accessToken = null;
            int numberOfSubreddits = _random.Next(1, 10);

            IRedditLogic sut = CreateSut();

            sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);
        }

        [TestMethod]
        public void GetNsfwSubredditsAsync_WhenCalled_AssertGetKnownNsfwSubredditsAsyncWasCalledOnDataProviderFactory()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            IRedditLogic sut = CreateSut();

            Task<IEnumerable<IRedditSubreddit>> getNsfwSubredditsTask = sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);
            getNsfwSubredditsTask.Wait();

            _dataProviderFactoryMock.Verify(m => m.GetKnownNsfwSubredditsAsync(), Times.Once);
        }

        [TestMethod]
        public void GetNsfwSubredditsAsync_WhenCalled_AssertRankWasCalledOnEachKnownNsfwSubreddit()
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

            Task<IEnumerable<IRedditSubreddit>> getNsfwSubredditsTask = sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);
            getNsfwSubredditsTask.Wait();

            knownNsfwSubreddit1.Verify(m => m.Rank, Times.Once);
            knownNsfwSubreddit2.Verify(m => m.Rank, Times.Once);
            knownNsfwSubreddit3.Verify(m => m.Rank, Times.Once);
            knownNsfwSubreddit4.Verify(m => m.Rank, Times.Once);
            knownNsfwSubreddit5.Verify(m => m.Rank, Times.Once);
        }

        [TestMethod]
        public void GetNsfwSubredditsAsync_WhenCalled_AssertNameWasCalledOnEachKnownNsfwSubreddit()
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

            Task<IEnumerable<IRedditSubreddit>> getNsfwSubredditsTask = sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);
            getNsfwSubredditsTask.Wait();

            knownNsfwSubreddit1.Verify(m => m.Name, Times.Once);
            knownNsfwSubreddit2.Verify(m => m.Name, Times.Once);
            knownNsfwSubreddit3.Verify(m => m.Name, Times.Once);
            knownNsfwSubreddit4.Verify(m => m.Name, Times.Once);
            knownNsfwSubreddit5.Verify(m => m.Name, Times.Once);
        }

        [TestMethod]
        public void GetNsfwSubredditsAsync_WhenCalled_AssertWillExceedRateLimitWasCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            int numberOfKnownNsfwSubreddits = _random.Next(5, 10);
            IEnumerable<IRedditKnownSubreddit> knownNsfwSubredditCollection = CreateRedditKnownSubredditCollection(numberOfSubreddits: numberOfKnownNsfwSubreddits);
            IRedditLogic sut = CreateSut(knownNsfwSubredditCollection: knownNsfwSubredditCollection);

            Task<IEnumerable<IRedditSubreddit>> getNsfwSubredditsTask = sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);
            getNsfwSubredditsTask.Wait();

            _redditRateLimitLogicMock.Verify(m => m.WillExceedRateLimit(It.Is<int>(value => value == Math.Min(numberOfSubreddits, numberOfKnownNsfwSubreddits))), Times.Once);
        }

        [TestMethod]
        public void GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertWillExceedRateLimitWasCalledOnlyOnceOnRedditRateLimitLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getNsfwSubredditsTask = sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);
            getNsfwSubredditsTask.Wait();

            _redditRateLimitLogicMock.Verify(m => m.WillExceedRateLimit(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertGetSpecificSubredditAsyncWasNotCalledOnRedditRepository()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getNsfwSubredditsTask = sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);
            getNsfwSubredditsTask.Wait();

            _redditRepositoryMock.Verify(m => m.GetSpecificSubredditAsync(
                    It.IsAny<IRedditAccessToken>(),
                    It.IsAny<IRedditKnownSubreddit>()),
                Times.Never);
        }

        [TestMethod]
        public void GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertEnforceRateLimitAsyncWasNotCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getNsfwSubredditsTask = sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);
            getNsfwSubredditsTask.Wait();

            _redditRateLimitLogicMock.Verify(m => m.EnforceRateLimitAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime>()),
                Times.Never);
        }

        [TestMethod]
        public void GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getNsfwSubredditsTask = sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);
            getNsfwSubredditsTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasExceeded_ReturnsEmptySubredditCollection()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getNsfwSubredditsTask = sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);
            getNsfwSubredditsTask.Wait();

            IEnumerable<IRedditSubreddit> result = getNsfwSubredditsTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetNsfwSubredditsAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertWillExceedRateLimitWasCalledOnRedditRateLimitLogicForEachKnownNsfwSubredditToGet()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();;
            int numberOfSubreddits = _random.Next(1, 10);

            int numberOfKnownNsfwSubreddits = _random.Next(5, 10);
            IEnumerable<IRedditKnownSubreddit> knownNsfwSubredditCollection = CreateRedditKnownSubredditCollection(numberOfSubreddits: numberOfKnownNsfwSubreddits);
            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(knownNsfwSubredditCollection: knownNsfwSubredditCollection, willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getNsfwSubredditsTask = sut.GetNsfwSubredditsAsync(accessToken, numberOfSubreddits);
            getNsfwSubredditsTask.Wait();

            _redditRateLimitLogicMock.Verify(m => m.WillExceedRateLimit(It.Is<int>(value => value == 1)), Times.Exactly(Math.Min(numberOfSubreddits, numberOfKnownNsfwSubreddits) + Convert.ToInt32(numberOfSubreddits == 1)));
        }

        private IRedditLogic CreateSut(IEnumerable<IRedditKnownSubreddit> knownNsfwSubredditCollection = null, bool willExceedRateLimit = false, Exception exception = null)
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

            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<AggregateException>()))
                .Returns(Task.Run(() => { }));
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.Run(() => { }));

            return new OSDevGrp.MyDashboard.Core.Logic.RedditLogic(
                _dataProviderFactoryMock.Object,
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
    }
}