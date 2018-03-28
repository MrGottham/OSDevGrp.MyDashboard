using System;
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
    public class GetAuthenticatedUserAsyncTests
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
        public void GetAuthenticatedUserAsync_WhenRedditAccessTokenIsNull_ThrowsArgumentNullException()
        {
            IRedditLogic sut = CreateSut();

            sut.GetAuthenticatedUserAsync(null);
        }

        [TestMethod]
        public void GetAuthenticatedUserAsync_WhenCalled_AssertWillExceedRateLimitWasCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            IRedditLogic sut = CreateSut();

            Task<IRedditAuthenticatedUser> getAuthenticatedUserTask = sut.GetAuthenticatedUserAsync(redditAccessToken);
            getAuthenticatedUserTask.Wait();

            _redditRateLimitLogicMock.Verify(m => m.WillExceedRateLimit(It.Is<int>(value => value == 1)), Times.Once);
        }

        [TestMethod]
        public void GetAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertGetAuthenticatedUserAsyncWasNotCalledOnRedditRepository()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IRedditAuthenticatedUser> getAuthenticatedUserTask = sut.GetAuthenticatedUserAsync(redditAccessToken);
            getAuthenticatedUserTask.Wait();

            _redditRepositoryMock.Verify(m => m.GetAuthenticatedUserAsync(It.IsAny<IRedditAccessToken>()), Times.Never);
        }

        [TestMethod]
        public void GetAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertEnforceRateLimitAsyncWasNotCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IRedditAuthenticatedUser> getAuthenticatedUserTask = sut.GetAuthenticatedUserAsync(redditAccessToken);
            getAuthenticatedUserTask.Wait();

            _redditRateLimitLogicMock.Verify(m => m.EnforceRateLimitAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime>()),
                Times.Never);
        }

        [TestMethod]
        public void GetAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasExceeded_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IRedditAuthenticatedUser> getAuthenticatedUserTask = sut.GetAuthenticatedUserAsync(redditAccessToken);
            getAuthenticatedUserTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void GetAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasExceeded_ReturnsNull()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            const bool willExceedRateLimit = true;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IRedditAuthenticatedUser> getAuthenticatedUserTask = sut.GetAuthenticatedUserAsync(redditAccessToken);
            getAuthenticatedUserTask.Wait();

            Assert.IsNull(getAuthenticatedUserTask.Result);
        }

        [TestMethod]
        public void GetAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertGetAuthenticatedUserAsyncWasCalledOnRedditRepository()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IRedditAuthenticatedUser> getAuthenticatedUserTask = sut.GetAuthenticatedUserAsync(redditAccessToken);
            getAuthenticatedUserTask.Wait();

            _redditRepositoryMock.Verify(m => m.GetAuthenticatedUserAsync(It.Is<IRedditAccessToken>(value => value == redditAccessToken)), Times.Once);
        }

        [TestMethod]
        public void GetAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertEnforceRateLimitAsyncWasCalledOnRedditRateLimitLogic()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            const bool willExceedRateLimit = false;
            int rateLimitUsed = _random.Next(1, 60);
            int rateLimitRemaining = _random.Next(1, 60);
            DateTime rateLimitResetTime = DateTime.Now.AddSeconds(_random.Next(90, 300));
            DateTime receivedTime = DateTime.Now.AddSeconds(_random.Next(1, 10) * -1);
            IRedditResponse<IRedditAuthenticatedUser> redditResponse = CreateRedditResponse(rateLimitUsed, rateLimitRemaining, rateLimitResetTime, receivedTime);
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, redditResponse: redditResponse);

            Task<IRedditAuthenticatedUser> getAuthenticatedUserTask = sut.GetAuthenticatedUserAsync(redditAccessToken);
            getAuthenticatedUserTask.Wait();

            _redditRateLimitLogicMock.Verify(m => m.EnforceRateLimitAsync(
                    It.Is<int>(value => value == rateLimitUsed),
                    It.Is<int>(value => value == rateLimitRemaining),
                    It.Is<DateTime?>(value => value.HasValue && value.Value == rateLimitResetTime),
                    It.Is<DateTime>(value => value == receivedTime)), 
                Times.Once);
        }

        [TestMethod]
        public void GetAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceeded_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            const bool willExceedRateLimit = false;
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit);

            Task<IRedditAuthenticatedUser> getAuthenticatedUserTask = sut.GetAuthenticatedUserAsync(redditAccessToken);
            getAuthenticatedUserTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void GetAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceeded_ReturnsRedditAuthenticatedUserFromRedditRepository()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            const bool willExceedRateLimit = false;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();
            IRedditResponse<IRedditAuthenticatedUser> redditResponse = CreateRedditResponse(redditAuthenticatedUser: redditAuthenticatedUser);
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, redditResponse: redditResponse);

            Task<IRedditAuthenticatedUser> getAuthenticatedUserTask = sut.GetAuthenticatedUserAsync(redditAccessToken);
            getAuthenticatedUserTask.Wait();

            Assert.AreEqual(redditAuthenticatedUser, getAuthenticatedUserTask.Result);
        }

        [TestMethod]
        public void GetAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceededAndAggregateExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            const bool willExceedRateLimit = false;
            AggregateException aggregateException = new AggregateException();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: aggregateException);

            Task<IRedditAuthenticatedUser> getAuthenticatedUserTask = sut.GetAuthenticatedUserAsync(redditAccessToken);
            getAuthenticatedUserTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<AggregateException>(value => value == aggregateException)), Times.Once);
        }

        [TestMethod]
        public void GetAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceededAndAggregateExceptionOccurs_ReturnsNull()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            const bool willExceedRateLimit = false;
            AggregateException aggregateException = new AggregateException();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: aggregateException);

            Task<IRedditAuthenticatedUser> getAuthenticatedUserTask = sut.GetAuthenticatedUserAsync(redditAccessToken);
            getAuthenticatedUserTask.Wait();

            Assert.IsNull(getAuthenticatedUserTask.Result);
        }

        [TestMethod]
        public void GetAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceededAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            const bool willExceedRateLimit = false;
            Exception exception = new Exception();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: exception);

            Task<IRedditAuthenticatedUser> getAuthenticatedUserTask = sut.GetAuthenticatedUserAsync(redditAccessToken);
            getAuthenticatedUserTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(value => value == exception)), Times.Once);
        }

        [TestMethod]
        public void GetAuthenticatedUserAsync_WhenCalledAndRedditRateLimitHasNotExceededAndExceptionOccurs_ReturnsNull()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            const bool willExceedRateLimit = false;
            Exception exception = new Exception();
            IRedditLogic sut = CreateSut(willExceedRateLimit: willExceedRateLimit, exception: exception);

            Task<IRedditAuthenticatedUser> getAuthenticatedUserTask = sut.GetAuthenticatedUserAsync(redditAccessToken);
            getAuthenticatedUserTask.Wait();

            Assert.IsNull(getAuthenticatedUserTask.Result);
        }

        private IRedditLogic CreateSut(bool willExceedRateLimit = false, IRedditResponse<IRedditAuthenticatedUser> redditResponse = null, Exception exception = null)
        {
            _redditRateLimitLogicMock.Setup(m => m.WillExceedRateLimit(It.IsAny<int>()))
                .Returns(willExceedRateLimit);
            _redditRateLimitLogicMock.Setup(m => m.EnforceRateLimitAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime>()))
                .Returns(Task.Run(() => { }));

            if (exception != null)
            {
                _redditRepositoryMock.Setup(m => m.GetAuthenticatedUserAsync(It.IsAny<IRedditAccessToken>()))
                    .Throws(exception);
            }
            else
            {
                _redditRepositoryMock.Setup(m => m.GetAuthenticatedUserAsync(It.IsAny<IRedditAccessToken>()))
                    .Returns(Task.Run<IRedditResponse<IRedditAuthenticatedUser>>(() => redditResponse ?? CreateRedditResponse()));
            }

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

        private IRedditResponse<IRedditAuthenticatedUser> CreateRedditResponse(int? rateLimitUsed = null, int? rateLimitRemaining = null, DateTime? rateLimitResetTime = null, DateTime? receivedTime = null, IRedditAuthenticatedUser redditAuthenticatedUser = null)
        {
            return CreateRedditResponseMock(rateLimitUsed, rateLimitRemaining, rateLimitResetTime, receivedTime, redditAuthenticatedUser).Object;
        }

        private Mock<IRedditResponse<IRedditAuthenticatedUser>> CreateRedditResponseMock(int? rateLimitUsed = null, int? rateLimitRemaining = null, DateTime? rateLimitResetTime = null, DateTime? receivedTime = null, IRedditAuthenticatedUser redditAuthenticatedUser = null)
        {
            Mock<IRedditResponse<IRedditAuthenticatedUser>> redditResponseMock = new Mock<IRedditResponse<IRedditAuthenticatedUser>>();
            redditResponseMock.Setup(m => m.RateLimitUsed)
                .Returns(rateLimitUsed ?? _random.Next(1, 60));
            redditResponseMock.Setup(m => m.RateLimitRemaining)
                .Returns(rateLimitRemaining ?? _random.Next(1, 60));
            redditResponseMock.Setup(m => m.RateLimitResetTime)
                .Returns(rateLimitResetTime ?? DateTime.Now.AddSeconds(_random.Next(30, 60)));
            redditResponseMock.Setup(m => m.ReceivedTime)
                .Returns(receivedTime ?? DateTime.Now.AddSeconds(_random.Next(1, 10) * -1));
            redditResponseMock.Setup(m => m.Data)
                .Returns(redditAuthenticatedUser ?? CreateRedditAuthenticatedUser());
            return redditResponseMock;
        }

        private IRedditAuthenticatedUser CreateRedditAuthenticatedUser()
        {
            return CreateRedditAuthenticatedUserMock().Object;
        }

        private Mock<IRedditAuthenticatedUser> CreateRedditAuthenticatedUserMock()
        {
            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = new Mock<IRedditAuthenticatedUser>();
            return redditAuthenticatedUserMock;
        }
    }
}