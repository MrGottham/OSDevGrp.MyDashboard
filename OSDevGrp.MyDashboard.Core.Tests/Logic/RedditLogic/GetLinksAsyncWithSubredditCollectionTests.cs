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
    public class GetLinksAsyncWithSubredditCollectionTests
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
        public async Task GetLinksAsync_WhenRedditAccessTokenIsNull_ThrowsArgumentNullException()
        {
            const IRedditAccessToken accessToken = null;
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            IRedditLogic sut = CreateSut();

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);
        }

        [TestMethod]
        [ExpectedArgumentNullException("subredditCollection")]
        public async Task GetLinksAsync_WhenSubredditCollectionIsNull_ThrowsArgumentNullException()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            const IEnumerable<IRedditSubreddit> subredditCollection = null;
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            IRedditLogic sut = CreateSut();

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalled_AssertWillExceedRateLimitWasCalledOnRedditRateLimitLogic()
        {
            int numberOfSubreddits = _random.Next(1, 10);
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection(numberOfSubreddits: numberOfSubreddits);
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            IRedditLogic sut = CreateSut();

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _redditRateLimitLogicMock.Verify(m => m.WillExceedRateLimit(It.Is<int>(value => value == numberOfSubreddits)), Times.Exactly(numberOfSubreddits > 1 ? 1 : 2));
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertGetLinksAsyncWasNotCalledOnRedditRepository()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _redditRepositoryMock.Verify(m => m.GetLinksAsync(
                    It.IsAny<IRedditAccessToken>(),
                    It.IsAny<IRedditSubreddit>()),
                Times.Never);
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertEnforceRateLimitAsyncWasNotCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _redditRateLimitLogicMock.Verify(m => m.EnforceRateLimitAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime>()),
                Times.Never);
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertRemoveUserBannedContentAsyncWasNotCalledOnRedditFilterLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _redditFilterLogicMock.Verify(m => m.RemoveUserBannedContentAsync(It.IsAny<IEnumerable<IRedditLink>>()), Times.Never);
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertRemoveNsfwContentAsyncWasNotCalledOnRedditFilterLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _redditFilterLogicMock.Verify(m => m.RemoveNsfwContentAsync(It.IsAny<IEnumerable<IRedditLink>>()), Times.Never);
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertRemoveNoneNsfwContentAsyncWasNotCalledOnRedditFilterLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _redditFilterLogicMock.Verify(m => m.RemoveNoneNsfwContentAsync(It.IsAny<IEnumerable<IRedditLink>>()), Times.Never);
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertCreateComparerAsyncWasNotCalledOnRedditFilterLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _redditFilterLogicMock.Verify(m => m.CreateComparerAsync<IRedditLink>(), Times.Never);
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasExceeded_ReturnsEmptyCollection()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            IEnumerable<IRedditLink> result = await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertGetLinksAsyncWasCalledOnRedditRepositoryForEachSubredditInCollection()
        {
            int numberOfSubreddits = _random.Next(1, 10);
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection(numberOfSubreddits: numberOfSubreddits);
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            foreach (IRedditSubreddit subreddit in subredditCollection)
            {
                _redditRepositoryMock.Verify(m => m.GetLinksAsync(
                        It.Is<IRedditAccessToken>(value => value == accessToken),
                        It.Is<IRedditSubreddit>(value => value == subreddit)),
                    Times.Once);
            }
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertEnforceRateLimitAsyncWasCalledOnRedditRateLimitLogicForEachSubredditInCollection()
        {
            int numberOfSubreddits = _random.Next(1, 10);
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection(numberOfSubreddits: numberOfSubreddits);
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            int rateLimitUsed = _random.Next(1, 60);
            int rateLimitRemaining = _random.Next(1, 60);
            DateTime rateLimitResetTime = DateTime.Now.AddSeconds(_random.Next(90, 300));
            DateTime receivedTime = DateTime.Now.AddSeconds(_random.Next(1, 10) * -1);
            IRedditResponse<IRedditList<IRedditLink>> redditResponse = CreateRedditResponse(rateLimitUsed, rateLimitRemaining, rateLimitResetTime, receivedTime);
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, redditResponse: redditResponse);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _redditRateLimitLogicMock.Verify(m => m.EnforceRateLimitAsync(
                    It.Is<int>(value => value == rateLimitUsed),
                    It.Is<int>(value => value == rateLimitRemaining),
                    It.Is<DateTime?>(value => value.HasValue && value.Value == rateLimitResetTime),
                    It.Is<DateTime>(value => value == receivedTime)), 
                Times.Exactly(numberOfSubreddits));
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertRemoveUserBannedContentAsyncWasCalledOnRedditFilterLogicForEachSubredditInCollection()
        {
            int numberOfSubreddits = _random.Next(1, 10);
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection(numberOfSubreddits: numberOfSubreddits);
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            IRedditList<IRedditLink> redditList = CreateRedditList();
            IRedditResponse<IRedditList<IRedditLink>> redditResponse = CreateRedditResponse(redditList: redditList);
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, redditResponse: redditResponse);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _redditFilterLogicMock.Verify(m => m.RemoveUserBannedContentAsync(It.Is<IEnumerable<IRedditLink>>(value => value == redditList)), Times.Exactly(numberOfSubreddits));
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceededAndIncludeNsfwContentIsFalse_AssertRemoveNsfwContentAsyncWasCalledOnRedditFilterLogicForEachSubredditInCollection()
        {
            int numberOfSubreddits = _random.Next(1, 10);
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection(numberOfSubreddits: numberOfSubreddits);
            const bool includeNsfwContent = false;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            IEnumerable<IRedditLink> filteredLinkCollection = new List<IRedditLink>(0);
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, filteredLinkCollection: filteredLinkCollection);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _redditFilterLogicMock.Verify(m => m.RemoveNsfwContentAsync(It.Is<IEnumerable<IRedditLink>>(value => value == filteredLinkCollection)), Times.Exactly(numberOfSubreddits));
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceededAndIncludeNsfwContentIsTrue_AssertRemoveNsfwContentAsyncWasNotCalledOnRedditFilterLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            const bool includeNsfwContent = true;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _redditFilterLogicMock.Verify(m => m.RemoveNsfwContentAsync(It.IsAny<IEnumerable<IRedditLink>>()), Times.Never);
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceededAndOnlyNsfwContentIsTrue_AssertRemoveNoneNsfwContentAsyncWasCalledOnRedditFilterLogicForEachSubredditInCollection()
        {
            int numberOfSubreddits = _random.Next(1, 10);
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection(numberOfSubreddits: numberOfSubreddits);
            bool includeNsfwContent = _random.Next(100) > 50;
            const bool onlyNsfwContent = true;

            const bool willExceedRateLimit = false;
            IEnumerable<IRedditLink> filteredLinkCollection = new List<IRedditLink>(0);
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, filteredLinkCollection: filteredLinkCollection);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _redditFilterLogicMock.Verify(m => m.RemoveNoneNsfwContentAsync(It.Is<IEnumerable<IRedditLink>>(value => value == filteredLinkCollection)), Times.Exactly(numberOfSubreddits));
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceededAndOnlyNsfwContentIsFalse_AssertRemoveNoneNsfwContentAsyncWasNotCalledOnRedditFilterLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            bool includeNsfwContent = _random.Next(100) > 50;
            const bool onlyNsfwContent = false;

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _redditFilterLogicMock.Verify(m => m.RemoveNoneNsfwContentAsync(It.IsAny<IEnumerable<IRedditLink>>()), Times.Never);
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertCreateComparerAsyncWasCalledOnRedditFilterLogic()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _redditFilterLogicMock.Verify(m => m.CreateComparerAsync<IRedditLink>(), Times.Once);
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertCreatedTimeWasCalledOnEachLinkInFilteredCollectionOfLinks()
        {
            int numberOfSubreddits = _random.Next(1, 10);
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection(numberOfSubreddits: numberOfSubreddits);
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

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            link1Mock.Verify(m => m.CreatedTime, Times.Exactly(numberOfSubreddits * 2));
            link2Mock.Verify(m => m.CreatedTime, Times.Exactly(numberOfSubreddits * 2));
            link3Mock.Verify(m => m.CreatedTime, Times.Exactly(numberOfSubreddits * 2));
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceeded_ReturnsFilteredCollectionOfLinks()
        {
            int numberOfSubreddits = _random.Next(1, 10);
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection(numberOfSubreddits: numberOfSubreddits);
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

            IEnumerable<IRedditLink> result = await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            Assert.IsNotNull(result);
            Assert.AreEqual(filteredLinkCollection.Count() * numberOfSubreddits, result.Count());
            Assert.AreEqual(numberOfSubreddits, result.Count(subreddit => subreddit == link1));
            Assert.AreEqual(numberOfSubreddits, result.Count(subreddit => subreddit == link2));
            Assert.AreEqual(numberOfSubreddits, result.Count(subreddit => subreddit == link3));
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceededAndAggregateExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            AggregateException aggregateException = new AggregateException();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: aggregateException);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<AggregateException>(value => value == aggregateException)), Times.Once);
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceededAndAggregateExceptionOccurs_ReturnsEmptyLinkCollection()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            AggregateException aggregateException = new AggregateException();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: aggregateException);

            IEnumerable<IRedditLink> result = await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceededAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            Exception exception = new Exception();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: exception);

            await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(value => value == exception)), Times.Once);
        }

        [TestMethod]
        public async Task GetLinksAsync_WhenCalledAndRedditRateLimitHasNotExceededAndExceptionOccurs_ReturnsEmptyLinkCollection()
        {
            IRedditAccessToken accessToken = CreateRedditAccessToken();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection();
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;

            const bool willExceedRateLimit = false;
            Exception exception = new Exception();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: exception);

            IEnumerable<IRedditLink> result = await sut.GetLinksAsync(accessToken, subredditCollection, includeNsfwContent, onlyNsfwContent);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        private IRedditLogic CreateSut(bool willExceedRateLimit = false, IRedditResponse<IRedditList<IRedditLink>> redditResponse = null, IEnumerable<IRedditLink> filteredLinkCollection = null, Exception exception = null)
        {
            if (exception != null)
            {
                _redditRateLimitLogicMock.Setup(m => m.WillExceedRateLimit(It.IsAny<int>()))
                    .Throws(exception);
            }
            else
            {
                _redditRateLimitLogicMock.Setup(m => m.WillExceedRateLimit(It.IsAny<int>()))
                    .Returns(willExceedRateLimit);
            }
            _redditRateLimitLogicMock.Setup(m => m.EnforceRateLimitAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime>()))
                .Returns(Task.Run(() => { }));

            _redditRepositoryMock.Setup(m => m.GetLinksAsync(It.IsAny<IRedditAccessToken>(), It.IsAny<IRedditSubreddit>()))
                .Returns(Task.Run(() => redditResponse ?? CreateRedditResponse()));

            Mock<IRedditThingComparer<IRedditLink>> redditLinkComparerMock = new Mock<IRedditThingComparer<IRedditLink>>();
            redditLinkComparerMock.Setup(m => m.Equals(It.IsAny<IRedditLink>(), It.IsAny<IRedditLink>()))
                .Returns(false);
            redditLinkComparerMock.Setup(m => m.GetHashCode())
                .Returns(_random.Next());

            _redditFilterLogicMock.Setup(m => m.RemoveUserBannedContentAsync(It.IsAny<IEnumerable<IRedditLink>>()))
                .Returns(Task.Run<IEnumerable<IRedditLink>>(() => filteredLinkCollection ?? new List<IRedditLink>(0)));
            _redditFilterLogicMock.Setup(m => m.RemoveNsfwContentAsync(It.IsAny<IEnumerable<IRedditLink>>()))
                .Returns(Task.Run<IEnumerable<IRedditLink>>(() => filteredLinkCollection ?? new List<IRedditLink>(0)));
            _redditFilterLogicMock.Setup(m => m.RemoveNoneNsfwContentAsync(It.IsAny<IEnumerable<IRedditLink>>()))
                .Returns(Task.Run<IEnumerable<IRedditLink>>(() => filteredLinkCollection ?? new List<IRedditLink>(0)));
            _redditFilterLogicMock.Setup(m => m.CreateComparerAsync<IRedditLink>())
                .Returns(Task.Run<IRedditThingComparer<IRedditLink>>(() => redditLinkComparerMock.Object));

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

        private IEnumerable<IRedditSubreddit> CreateSubredditCollection(int numberOfSubreddits = 4)
        {
            IList<IRedditSubreddit> subredditCollection = new List<IRedditSubreddit>(numberOfSubreddits);
            while (subredditCollection.Count < numberOfSubreddits)
            {
                subredditCollection.Add(CreateSubreddit());
            }
            return subredditCollection;
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