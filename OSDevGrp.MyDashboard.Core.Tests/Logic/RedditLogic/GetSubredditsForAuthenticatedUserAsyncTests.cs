using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Logic.RedditLogic
{
    [TestClass]
    public class GetSubredditsForAuthenticatedUserAsyncTests
    {
        #region Private variables

        private Mock<IRedditRepository> _redditRepositoryMock;
        private Mock<IRedditRateLimitLogic> _redditRateLimitLogicMock;
        private Mock<IRedditFilterLogic> _redditFilterLogicMock;
        private Mock<IExceptionHandler> _exceptionHandlerMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _redditRepositoryMock = new Mock<IRedditRepository>();
            _redditRateLimitLogicMock = new Mock<IRedditRateLimitLogic>();
            _redditFilterLogicMock = new Mock<IRedditFilterLogic>();
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullException("accessToken")]
        public void GetSubredditsForAuthenticatedUserAsync_WhenRedditAccessTokenIsNull_ThrowsArgumentNullException()
        {
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            IRedditLogic sut = CreateSut();

            sut.GetSubredditsForAuthenticatedUserAsync(null, includeNsfwContent, onlyNsfwContent);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalled_AssertWillExceedRateLimitWasCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            IRedditLogic sut = CreateSut();

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _redditRateLimitLogicMock.Verify(m => m.WillExceedRateLimit(It.Is<int>(value => value == 1)), Times.Once);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertGetSubredditsForAuthenticatedUserAsyncWasNotCalledOnRedditRepository()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _redditRepositoryMock.Verify(m => m.GetSubredditsForAuthenticatedUserAsync(It.IsAny<IRedditAccessToken>()), Times.Never);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertEnforceRateLimitAsyncWasNotCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _redditRateLimitLogicMock.Verify(m => m.EnforceRateLimitAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime>()),
                Times.Never);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertRemoveUserBannedContentAsyncWasNotCalledOnRedditFilterLogic()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _redditFilterLogicMock.Verify(m => m.RemoveUserBannedContentAsync(It.IsAny<IEnumerable<IRedditSubreddit>>()), Times.Never);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertRemoveNsfwContentAsyncWasNotCalledOnRedditFilterLogic()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _redditFilterLogicMock.Verify(m => m.RemoveNsfwContentAsync(It.IsAny<IEnumerable<IRedditSubreddit>>()), Times.Never);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertRemoveNoneNsfwContentAsyncWasNotCalledOnRedditFilterLogic()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _redditFilterLogicMock.Verify(m => m.RemoveNoneNsfwContentAsync(It.IsAny<IEnumerable<IRedditSubreddit>>()), Times.Never);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasExceeded_ReturnsEmptySubredditCollection()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            IEnumerable<IRedditSubreddit> result = getSubredditsForAuthenticatedUserTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertGetSubredditsForAuthenticatedUserAsyncWasCalledOnRedditRepository()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _redditRepositoryMock.Verify(m => m.GetSubredditsForAuthenticatedUserAsync(It.Is<IRedditAccessToken>(value => value == redditAccessToken)), Times.Once);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertEnforceRateLimitAsyncWasCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = false;
            int rateLimitUsed = _random.Next(1, 60);
            int rateLimitRemaining = _random.Next(1, 60);
            DateTime rateLimitResetTime = DateTime.Now.AddSeconds(_random.Next(90, 300));
            DateTime receivedTime = DateTime.Now.AddSeconds(_random.Next(1, 10) * -1);
            IRedditResponse<IRedditList<IRedditSubreddit>> redditResponse = CreateRedditResponse(rateLimitUsed, rateLimitRemaining, rateLimitResetTime, receivedTime);
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, redditResponse: redditResponse);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _redditRateLimitLogicMock.Verify(m => m.EnforceRateLimitAsync(
                    It.Is<int>(value => value == rateLimitUsed),
                    It.Is<int>(value => value == rateLimitRemaining),
                    It.Is<DateTime?>(value => value.HasValue && value.Value == rateLimitResetTime),
                    It.Is<DateTime>(value => value == receivedTime)), 
                Times.Once);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertRemoveUserBannedContentAsyncWasCalledOnRedditFilterLogic()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = false;
            IRedditList<IRedditSubreddit> redditList = CreateRedditList();
            IRedditResponse<IRedditList<IRedditSubreddit>> redditResponse = CreateRedditResponse(redditList: redditList);
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, redditResponse: redditResponse);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _redditFilterLogicMock.Verify(m => m.RemoveUserBannedContentAsync(It.Is<IEnumerable<IRedditSubreddit>>(value => value == redditList)), Times.Once);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceededAndIncludeNsfwContentIsFalse_AssertRemoveNsfwContentAsyncWasCalledOnRedditFilterLogic()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            const bool includeNsfwContent = false;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = false;
            IEnumerable<IRedditSubreddit> filteredSubredditCollection = new List<IRedditSubreddit>(0);
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, filteredSubredditCollection: filteredSubredditCollection);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _redditFilterLogicMock.Verify(m => m.RemoveNsfwContentAsync(It.Is<IEnumerable<IRedditSubreddit>>(value => value == filteredSubredditCollection)), Times.Once);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceededAndIncludeNsfwContentIsTrue_AssertRemoveNsfwContentAsyncWasNotCalledOnRedditFilterLogic()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            const bool includeNsfwContent = true;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _redditFilterLogicMock.Verify(m => m.RemoveNsfwContentAsync(It.IsAny<IEnumerable<IRedditSubreddit>>()), Times.Never);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceededAndOnlyNsfwContentIsTrue_AssertRemoveNoneNsfwContentAsyncWasCalledOnRedditFilterLogic()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            const bool onlyNsfwContent = true;

            const bool willExceedRateLimit = false;
            IEnumerable<IRedditSubreddit> filteredSubredditCollection = new List<IRedditSubreddit>(0);
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, filteredSubredditCollection: filteredSubredditCollection);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _redditFilterLogicMock.Verify(m => m.RemoveNoneNsfwContentAsync(It.Is<IEnumerable<IRedditSubreddit>>(value => value == filteredSubredditCollection)), Times.Once);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceededAndOnlyNsfwContentIsFalse_AssertRemoveNoneNsfwContentAsyncWasNotCalledOnRedditFilterLogic()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            const bool onlyNsfwContent = false;

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _redditFilterLogicMock.Verify(m => m.RemoveNoneNsfwContentAsync(It.IsAny<IEnumerable<IRedditSubreddit>>()), Times.Never);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceeded_ReturnsFilteredCollectionOfSubreddits()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = false;
            IRedditSubreddit subreddit1 = CreateSubreddit();
            IRedditSubreddit subreddit2 = CreateSubreddit();
            IRedditSubreddit subreddit3 = CreateSubreddit();
            IEnumerable<IRedditSubreddit> filteredSubredditCollection = new List<IRedditSubreddit>
            {
                subreddit1,
                subreddit2,
                subreddit3
            };
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, filteredSubredditCollection: filteredSubredditCollection);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            IEnumerable<IRedditSubreddit> result = getSubredditsForAuthenticatedUserTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(filteredSubredditCollection.Count(), result.Count());
            Assert.IsTrue(result.Contains(subreddit1));
            Assert.IsTrue(result.Contains(subreddit2));
            Assert.IsTrue(result.Contains(subreddit3));
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceededAndAggregateExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = false;
            AggregateException aggregateException = new AggregateException();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: aggregateException);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<AggregateException>(value => value == aggregateException)), Times.Once);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceededAndAggregateExceptionOccurs_ReturnsEmptySubredditCollection()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = false;
            AggregateException aggregateException = new AggregateException();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: aggregateException);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            IEnumerable<IRedditSubreddit> result = getSubredditsForAuthenticatedUserTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceededAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = false;
            Exception exception = new Exception();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: exception);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(value => value == exception)), Times.Once);
        }

        [TestMethod]
        public void GetSubredditsForAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceededAndExceptionOccurs_ReturnsEmptySubredditCollection()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;

            const bool willExceedRateLimit = false;
            Exception exception = new Exception();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: exception);

            Task<IEnumerable<IRedditSubreddit>> getSubredditsForAuthenticatedUserTask = sut.GetSubredditsForAuthenticatedUserAsync(redditAccessToken, includeNsfwContent, onlyNsfwContent);
            getSubredditsForAuthenticatedUserTask.Wait();

            IEnumerable<IRedditSubreddit> result = getSubredditsForAuthenticatedUserTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        private IRedditLogic CreateSut(bool willExceedRateLimit = false, IRedditResponse<IRedditList<IRedditSubreddit>> redditResponse = null, IEnumerable<IRedditSubreddit> filteredSubredditCollection = null, Exception exception = null)
        {
            _redditRateLimitLogicMock.Setup(m => m.WillExceedRateLimit(It.IsAny<int>()))
                .Returns(willExceedRateLimit);
            _redditRateLimitLogicMock.Setup(m => m.EnforceRateLimitAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime>()))
                .Returns(Task.Run(() => { }));

            if (exception != null)
            {
                _redditRepositoryMock.Setup(m => m.GetSubredditsForAuthenticatedUserAsync(It.IsAny<IRedditAccessToken>()))
                    .Throws(exception);
            }
            else
            {
                _redditRepositoryMock.Setup(m => m.GetSubredditsForAuthenticatedUserAsync(It.IsAny<IRedditAccessToken>()))
                    .Returns(Task.Run<IRedditResponse<IRedditList<IRedditSubreddit>>>(() => redditResponse ?? CreateRedditResponse()));
            }

            _redditFilterLogicMock.Setup(m => m.RemoveUserBannedContentAsync(It.IsAny<IEnumerable<IRedditSubreddit>>()))
                .Returns(Task.Run<IEnumerable<IRedditSubreddit>>(() => filteredSubredditCollection ?? new List<IRedditSubreddit>(0)));
            _redditFilterLogicMock.Setup(m => m.RemoveNsfwContentAsync(It.IsAny<IEnumerable<IRedditSubreddit>>()))
                .Returns(Task.Run<IEnumerable<IRedditSubreddit>>(() => filteredSubredditCollection ?? new List<IRedditSubreddit>(0)));
            _redditFilterLogicMock.Setup(m => m.RemoveNoneNsfwContentAsync(It.IsAny<IEnumerable<IRedditSubreddit>>()))
                .Returns(Task.Run<IEnumerable<IRedditSubreddit>>(() => filteredSubredditCollection ?? new List<IRedditSubreddit>(0)));

            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<AggregateException>()))
                .Returns(Task.Run(() => { }));
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.Run(() => { }));

            return new OSDevGrp.MyDashboard.Core.Logic.RedditLogic(
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

        private IRedditResponse<IRedditList<IRedditSubreddit>> CreateRedditResponse(int? rateLimitUsed = null, int? rateLimitRemaining = null, DateTime? rateLimitResetTime = null, DateTime? receivedTime = null, IRedditList<IRedditSubreddit> redditList = null)
        {
            return CreateRedditResponseMock(rateLimitUsed, rateLimitRemaining, rateLimitResetTime, receivedTime, redditList).Object;
        }

        private Mock<IRedditResponse<IRedditList<IRedditSubreddit>>> CreateRedditResponseMock(int? rateLimitUsed = null, int? rateLimitRemaining = null, DateTime? rateLimitResetTime = null, DateTime? receivedTime = null, IRedditList<IRedditSubreddit> redditList = null)
        {
            Mock<IRedditResponse<IRedditList<IRedditSubreddit>>> redditResponseMock = new Mock<IRedditResponse<IRedditList<IRedditSubreddit>>>();
            redditResponseMock.Setup(m => m.RateLimitUsed)
                .Returns(rateLimitUsed ?? _random.Next(1, 60));
            redditResponseMock.Setup(m => m.RateLimitRemaining)
                .Returns(rateLimitRemaining ?? _random.Next(1, 60));
            redditResponseMock.Setup(m => m.RateLimitResetTime)
                .Returns(rateLimitResetTime ?? DateTime.Now.AddSeconds(_random.Next(30, 60)));
            redditResponseMock.Setup(m => m.ReceivedTime)
                .Returns(receivedTime ?? DateTime.Now.AddSeconds(_random.Next(1, 10) * -1));
            redditResponseMock.Setup(m => m.Data)
                .Returns(redditList ?? CreateRedditList());
            return redditResponseMock;
        }

        private IRedditList<IRedditSubreddit> CreateRedditList()
        {
            return CreateRedditListMock().Object;
        }

        private Mock<IRedditList<IRedditSubreddit>> CreateRedditListMock()
        {
            Mock<IRedditList<IRedditSubreddit>> redditListMock = new Mock<IRedditList<IRedditSubreddit>>();
            return redditListMock;
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