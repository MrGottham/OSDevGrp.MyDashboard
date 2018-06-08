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
    public class GetLinksAsyncTests
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
        public void GetLinksAsync_WhenRedditAccessTokenIsNull_ThrowsArgumentNullException()
        {
            const IRedditAccessToken accessToken = null;
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            IRedditLogic sut = CreateSut();

            sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
        }

        [TestMethod]
        [ExpectedArgumentNullException("subreddit")]
        public void GetLinksAsync_WhenSubredditIsNull_ThrowsArgumentNullException()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            const IRedditSubreddit subreddit = null;
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            IRedditLogic sut = CreateSut();

            sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalled_AssertWillExceedRateLimitWasCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            IRedditLogic sut = CreateSut();

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _redditRateLimitLogicMock.Verify(m => m.WillExceedRateLimit(It.Is<int>(value => value == 1)), Times.Once);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertGetLinksAsyncWasNotCalledOnRedditRepository()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _redditRepositoryMock.Verify(m => m.GetLinksAsync(
                    It.Is<IRedditAccessToken>(value => value == accessToken),
                    It.Is<IRedditSubreddit>(value => value == subreddit)),
                Times.Never);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertEnforceRateLimitAsyncWasNotCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _redditRateLimitLogicMock.Verify(m => m.EnforceRateLimitAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime>()),
                Times.Never);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertRemoveUserBannedContentAsyncWasNotCalledOnRedditFilterLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _redditFilterLogicMock.Verify(m => m.RemoveUserBannedContentAsync(It.IsAny<IEnumerable<IRedditLink>>()), Times.Never);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertRemoveNsfwContentAsyncWasNotCalledOnRedditFilterLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _redditFilterLogicMock.Verify(m => m.RemoveNsfwContentAsync(It.IsAny<IEnumerable<IRedditLink>>()), Times.Never);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertRemoveNoneNsfwContentAsyncWasNotCalledOnRedditFilterLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _redditFilterLogicMock.Verify(m => m.RemoveNoneNsfwContentAsync(It.IsAny<IEnumerable<IRedditLink>>()), Times.Never);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasExceeded_ReturnsEmptyCollection()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            IEnumerable<IRedditLink> result = getLinksTask.Result;
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertGetLinksAsyncWasCalledOnRedditRepository()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _redditRepositoryMock.Verify(m => m.GetLinksAsync(
                    It.Is<IRedditAccessToken>(value => value == accessToken),
                    It.Is<IRedditSubreddit>(value => value == subreddit)),
                Times.Once);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertEnforceRateLimitAsyncWasCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            int rateLimitUsed = _random.Next(1, 60);
            int rateLimitRemaining = _random.Next(1, 60);
            DateTime rateLimitResetTime = DateTime.Now.AddSeconds(_random.Next(90, 300));
            DateTime receivedTime = DateTime.Now.AddSeconds(_random.Next(1, 10) * -1);
            IRedditResponse<IRedditList<IRedditLink>> redditResponse = CreateRedditResponse(rateLimitUsed, rateLimitRemaining, rateLimitResetTime, receivedTime);
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, redditResponse: redditResponse);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _redditRateLimitLogicMock.Verify(m => m.EnforceRateLimitAsync(
                    It.Is<int>(value => value == rateLimitUsed),
                    It.Is<int>(value => value == rateLimitRemaining),
                    It.Is<DateTime?>(value => value.HasValue && value.Value == rateLimitResetTime),
                    It.Is<DateTime>(value => value == receivedTime)), 
                Times.Once);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertRemoveUserBannedContentAsyncWasCalledOnRedditFilterLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            IRedditList<IRedditLink> redditList = CreateRedditList();
            IRedditResponse<IRedditList<IRedditLink>> redditResponse = CreateRedditResponse(redditList: redditList);
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, redditResponse: redditResponse);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _redditFilterLogicMock.Verify(m => m.RemoveUserBannedContentAsync(It.Is<IEnumerable<IRedditLink>>(value => value == redditList)), Times.Once);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceededAndIncludeNsfwContentIsFalse_AssertRemoveNsfwContentAsyncWasCalledOnRedditFilterLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            const bool includeNsfwContent = false;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            IEnumerable<IRedditLink> filteredLinkCollection = new List<IRedditLink>(0);
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, filteredLinkCollection: filteredLinkCollection);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _redditFilterLogicMock.Verify(m => m.RemoveNsfwContentAsync(It.Is<IEnumerable<IRedditLink>>(value => value == filteredLinkCollection)), Times.Once);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceededAndIncludeNsfwContentIsTrue_AssertRemoveNsfwContentAsyncWasNotCalledOnRedditFilterLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            const bool includeNsfwContent = true;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _redditFilterLogicMock.Verify(m => m.RemoveNsfwContentAsync(It.IsAny<IEnumerable<IRedditLink>>()), Times.Never);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceededAndOnlyNsfwContentIsTrue_AssertRemoveNoneNsfwContentAsyncWasCalledOnRedditFilterLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            const bool onlyNsfwContent = true;

            const bool willExceedRateLimit = false;
            IEnumerable<IRedditLink> filteredLinkCollection = new List<IRedditLink>(0);
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, filteredLinkCollection: filteredLinkCollection);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _redditFilterLogicMock.Verify(m => m.RemoveNoneNsfwContentAsync(It.Is<IEnumerable<IRedditLink>>(value => value == filteredLinkCollection)), Times.Once);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceededAndOnlyNsfwContentIsFalse_AssertRemoveNoneNsfwContentAsyncWasNotCalledOnRedditFilterLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            const bool onlyNsfwContent = false;

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _redditFilterLogicMock.Verify(m => m.RemoveNoneNsfwContentAsync(It.IsAny<IEnumerable<IRedditLink>>()), Times.Never);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertCreatedTimeWasCalledOnEachLinkInFilteredCollectionOfLinks()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            Mock<IRedditLink> link1Mock = CreateLinkMock();
            Mock<IRedditLink> link2Mock = CreateLinkMock();
            Mock<IRedditLink> link3Mock = CreateLinkMock();
            IEnumerable<IRedditLink> filteredLinkCollection = new List<IRedditLink>
            {
                link1Mock.Object,
                link2Mock.Object,
                link3Mock.Object
            };
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, filteredLinkCollection: filteredLinkCollection);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            link1Mock.Verify(m => m.CreatedTime, Times.Once);
            link2Mock.Verify(m => m.CreatedTime, Times.Once);
            link3Mock.Verify(m => m.CreatedTime, Times.Once);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceeded_ReturnsFilteredCollectionOfLinks()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            IRedditLink link1 = CreateLink();
            IRedditLink link2 = CreateLink();
            IRedditLink link3 = CreateLink();
            IEnumerable<IRedditLink> filteredLinkCollection = new List<IRedditLink>
            {
                link1,
                link2,
                link3
            };
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, filteredLinkCollection: filteredLinkCollection);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            IEnumerable<IRedditLink> result = getLinksTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(filteredLinkCollection.Count(), result.Count());
            Assert.IsTrue(result.Contains(link1));
            Assert.IsTrue(result.Contains(link2));
            Assert.IsTrue(result.Contains(link3));
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceededAndAggregateExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            AggregateException aggregateException = new AggregateException();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: aggregateException);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<AggregateException>(value => value == aggregateException)), Times.Once);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceededAndAggregateExceptionOccurs_ReturnsEmptyLinkCollection()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            AggregateException aggregateException = new AggregateException();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: aggregateException);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            IEnumerable<IRedditLink> result = getLinksTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceededAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            Exception exception = new Exception();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: exception);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(value => value == exception)), Times.Once);
        }

        [TestMethod]
        public void GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceededAndExceptionOccurs_ReturnsEmptyLinkCollection()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IRedditSubreddit subreddit = CreateSubreddit();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            Exception exception = new Exception();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: exception);

            Task<IEnumerable<IRedditLink>> getLinksTask = sut.GetLinksAsync(accessToken, subreddit, includeNsfwContent, onlyNsfwContent);
            getLinksTask.Wait();

            IEnumerable<IRedditLink> result = getLinksTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        private IRedditLogic CreateSut(bool willExceedRateLimit = false, IRedditResponse<IRedditList<IRedditLink>> redditResponse = null, IEnumerable<IRedditLink> filteredLinkCollection = null, Exception exception = null)
        {
            _redditRateLimitLogicMock.Setup(m => m.WillExceedRateLimit(It.IsAny<int>()))
                .Returns(willExceedRateLimit);
            _redditRateLimitLogicMock.Setup(m => m.EnforceRateLimitAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime>()))
                .Returns(Task.Run(() => { }));

            if (exception != null)
            {
                _redditRepositoryMock.Setup(m => m.GetLinksAsync(It.IsAny<IRedditAccessToken>(), It.IsAny<IRedditSubreddit>()))
                    .Throws(exception);
            }
            else
            {
                _redditRepositoryMock.Setup(m => m.GetLinksAsync(It.IsAny<IRedditAccessToken>(), It.IsAny<IRedditSubreddit>()))
                    .Returns(Task.Run(() => redditResponse ?? CreateRedditResponse()));
            }

            _redditFilterLogicMock.Setup(m => m.RemoveUserBannedContentAsync(It.IsAny<IEnumerable<IRedditLink>>()))
                .Returns(Task.Run<IEnumerable<IRedditLink>>(() => filteredLinkCollection ?? new List<IRedditLink>(0)));
            _redditFilterLogicMock.Setup(m => m.RemoveNsfwContentAsync(It.IsAny<IEnumerable<IRedditLink>>()))
                .Returns(Task.Run<IEnumerable<IRedditLink>>(() => filteredLinkCollection ?? new List<IRedditLink>(0)));
            _redditFilterLogicMock.Setup(m => m.RemoveNoneNsfwContentAsync(It.IsAny<IEnumerable<IRedditLink>>()))
                .Returns(Task.Run<IEnumerable<IRedditLink>>(() => filteredLinkCollection ?? new List<IRedditLink>(0)));

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

        private IRedditSubreddit CreateSubreddit()
        {
            return CreateSubredditMock().Object;
        }

        private Mock<IRedditSubreddit> CreateSubredditMock()
        {
            Mock<IRedditSubreddit> subredditMock = new Mock<IRedditSubreddit>();
            return subredditMock;
        }

        private IRedditResponse<IRedditList<IRedditLink>> CreateRedditResponse(int? rateLimitUsed = null, int? rateLimitRemaining = null, DateTime? rateLimitResetTime = null, DateTime? receivedTime = null, IRedditList<IRedditLink> redditList = null)
        {
            return CreateRedditResponseMock(rateLimitUsed, rateLimitRemaining, rateLimitResetTime, receivedTime, redditList).Object;
        }

        private Mock<IRedditResponse<IRedditList<IRedditLink>>> CreateRedditResponseMock(int? rateLimitUsed = null, int? rateLimitRemaining = null, DateTime? rateLimitResetTime = null, DateTime? receivedTime = null, IRedditList<IRedditLink> redditList = null)
        {
            Mock<IRedditResponse<IRedditList<IRedditLink>>> redditResponseMock = new Mock<IRedditResponse<IRedditList<IRedditLink>>>();
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

        private IRedditList<IRedditLink> CreateRedditList()
        {
            return CreateRedditListMock().Object;
        }

        private Mock<IRedditList<IRedditLink>> CreateRedditListMock()
        {
            Mock<IRedditList<IRedditLink>> redditListMock = new Mock<IRedditList<IRedditLink>>();
            return redditListMock;
        }

        private IRedditLink CreateLink()
        {
            return CreateLinkMock().Object;
        }

        private Mock<IRedditLink> CreateLinkMock()
        {
            Mock<IRedditLink> linkMock = new Mock<IRedditLink>();
            linkMock.Setup(m => m.CreatedTime)
                .Returns(DateTime.Now.AddMinutes(_random.Next(0, 300) * -1));
            return linkMock;
        }
    }
}