using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;

namespace OSDevGrp.MyDashboard.Core.Tests.Logic.RedditLogic
{
    [TestClass]
    public class RenewAccessTokenAsyncTests
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
        public async Task RenewAccessTokenAsync_WhenRedditAccessTokenIsNull_ThrowsArgumentNullException()
        {
            const IRedditAccessToken accessToken = null;

            IRedditLogic sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.RenewAccessTokenAsync(accessToken));

            Assert.AreEqual("accessToken", result.ParamName);
        }

        [TestMethod]
        public async Task RenewAccessTokenAsync_WhenCalled_AssertExpiresWasCalledOnRedditAccessToken()
        {
            Mock<IRedditAccessToken> accessTokenMock = CreateRedditAccessTokenMock();

            IRedditLogic sut = CreateSut();

            await sut.RenewAccessTokenAsync(accessTokenMock.Object);

            accessTokenMock.Verify(m => m.Expires, Times.Once);
        }

        [TestMethod]
        public async Task RenewAccessTokenAsync_WhenCalledWithUnexpiredRedditAccessToken_AssertRefreshTokenWasNotCalledOnRedditAccessToken()
        {
            const bool hasExpired = false;
            Mock<IRedditAccessToken> accessTokenMock = CreateRedditAccessTokenMock(hasExpired: hasExpired);

            IRedditLogic sut = CreateSut();

            await sut.RenewAccessTokenAsync(accessTokenMock.Object);

            accessTokenMock.Verify(m => m.RefreshToken, Times.Never);
        }

        [TestMethod]
        public async Task RenewAccessTokenAsync_WhenCalledWithUnexpiredRedditAccessToken_AssertRenewRedditAccessTokenAsyncWasNotCalledOnRedditAccessTokenProviderFactory()
        {
            const bool hasExpired = false;
            IRedditAccessToken accessToken = CreateRedditAccessToken(hasExpired: hasExpired);

            IRedditLogic sut = CreateSut();

            await sut.RenewAccessTokenAsync(accessToken);

            _redditAccessTokenProviderFactoryMock.Verify(m => m.RenewRedditAccessTokenAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task RenewAccessTokenAsync_WhenCalledWithUnexpiredRedditAccessToken_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            const bool hasExpired = false;
            IRedditAccessToken accessToken = CreateRedditAccessToken(hasExpired: hasExpired);

            IRedditLogic sut = CreateSut();

            await sut.RenewAccessTokenAsync(accessToken);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public async Task RenewAccessTokenAsync_WhenCalledWithUnexpiredRedditAccessToken_ReturnsUnexpiredRedditAccessToken()
        {
            const bool hasExpired = false;
            IRedditAccessToken accessToken = CreateRedditAccessToken(hasExpired: hasExpired);

            IRedditLogic sut = CreateSut();

            IRedditAccessToken result = await sut.RenewAccessTokenAsync(accessToken);

            Assert.AreEqual(accessToken, result);
        }

        [TestMethod]
        public async Task RenewAccessTokenAsync_WhenCalledWithExpiredRedditAccessToken_AssertRefreshTokenWasCalledOnRedditAccessToken()
        {
            const bool hasExpired = true;
            Mock<IRedditAccessToken> accessTokenMock = CreateRedditAccessTokenMock(hasExpired: hasExpired);

            IRedditLogic sut = CreateSut();

            await sut.RenewAccessTokenAsync(accessTokenMock.Object);

            accessTokenMock.Verify(m => m.RefreshToken, Times.Once);
        }

        [TestMethod]
        public async Task RenewAccessTokenAsync_WhenCalledWithExpiredRedditAccessToken_AssertRenewRedditAccessTokenAsyncWasCalledOnRedditAccessTokenProviderFactory()
        {
            const bool hasExpired = true;
            string refreshToken = Guid.NewGuid().ToString("D");
            IRedditAccessToken accessToken = CreateRedditAccessToken(hasExpired: hasExpired, refreshToken: refreshToken);

            IRedditLogic sut = CreateSut();

            await sut.RenewAccessTokenAsync(accessToken);

            _redditAccessTokenProviderFactoryMock.Verify(m => m.RenewRedditAccessTokenAsync(It.Is<string>(value => string.Compare(refreshToken, value, StringComparison.Ordinal) == 0)), Times.Once);
        }

        [TestMethod]
        public async Task RenewAccessTokenAsync_WhenCalledWithExpiredRedditAccessToken_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            const bool hasExpired = true;
            IRedditAccessToken accessToken = CreateRedditAccessToken(hasExpired: hasExpired);

            IRedditLogic sut = CreateSut();

            await sut.RenewAccessTokenAsync(accessToken);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public async Task RenewAccessTokenAsync_WhenCalledWithExpiredRedditAccessToken_ReturnsRenewedRedditAccessToken()
        {
            const bool hasExpired = true;
            IRedditAccessToken accessToken = CreateRedditAccessToken(hasExpired: hasExpired);

            IRedditAccessToken renewedAccessToken = CreateRedditAccessToken();
            IRedditLogic sut = CreateSut(renewedAccessToken: renewedAccessToken);

            IRedditAccessToken result = await sut.RenewAccessTokenAsync(accessToken);

            Assert.AreEqual(renewedAccessToken, result);
        }

        [TestMethod]
        public async Task RenewAccessTokenAsync_WhenCalledWithExpiredRedditAccessTokenAndAggregateExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            const bool hasExpired = true;
            IRedditAccessToken accessToken = CreateRedditAccessToken(hasExpired: hasExpired);

            AggregateException aggregateException = new AggregateException();
            IRedditLogic sut = CreateSut(exception: aggregateException);

            await sut.RenewAccessTokenAsync(accessToken);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<AggregateException>(value => value == aggregateException)), Times.Once);
        }

        [TestMethod]
        public async Task RenewAccessTokenAsync_WhenCalledWithExpiredRedditAccessTokenAndAggregateExceptionOccurs_ReturnsNull()
        {
            const bool hasExpired = true;
            IRedditAccessToken accessToken = CreateRedditAccessToken(hasExpired: hasExpired);

            AggregateException aggregateException = new AggregateException();
            IRedditLogic sut = CreateSut(exception: aggregateException);

            IRedditAccessToken result = await sut.RenewAccessTokenAsync(accessToken);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task RenewAccessTokenAsync_WhenCalledWithExpiredRedditAccessTokenAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            const bool hasExpired = true;
            IRedditAccessToken accessToken = CreateRedditAccessToken(hasExpired: hasExpired);

            Exception exception = new Exception();
            IRedditLogic sut = CreateSut(exception: exception);

            await sut.RenewAccessTokenAsync(accessToken);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(value => value == exception)), Times.Once);
        }

        [TestMethod]
        public async Task RenewAccessTokenAsync_WhenCalledWithExpiredRedditAccessTokenAndExceptionOccurs_ReturnsNull()
        {
            const bool hasExpired = true;
            IRedditAccessToken accessToken = CreateRedditAccessToken(hasExpired: hasExpired);

            Exception exception = new Exception();
            IRedditLogic sut = CreateSut(exception: exception);

            IRedditAccessToken result = await sut.RenewAccessTokenAsync(accessToken);

            Assert.IsNull(result);
        }

        private IRedditLogic CreateSut(IRedditAccessToken renewedAccessToken = null, Exception exception = null)
        {
            if (exception != null)
            {
                _redditAccessTokenProviderFactoryMock.Setup(m => m.RenewRedditAccessTokenAsync(It.IsAny<string>()))
                    .Throws(exception);
            }
            else
            {
                _redditAccessTokenProviderFactoryMock.Setup(m => m.RenewRedditAccessTokenAsync(It.IsAny<string>()))
                    .Returns(Task.Run<IRedditAccessToken>(() => renewedAccessToken ?? CreateRedditAccessToken()));
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

        private IRedditAccessToken CreateRedditAccessToken(bool hasExpired = false, string refreshToken = null)
        {
            return CreateRedditAccessTokenMock(hasExpired, refreshToken).Object;
        }

        private Mock<IRedditAccessToken> CreateRedditAccessTokenMock(bool hasExpired = false, string refreshToken = null)
        {
            Mock<IRedditAccessToken> redditAccessTokenMock = new Mock<IRedditAccessToken>();
            redditAccessTokenMock.Setup(m => m.Expires)
                .Returns(DateTime.Now.AddMinutes(_random.Next(1, 5) * (hasExpired ? -1 : 1)));
            redditAccessTokenMock.Setup(m => m.RefreshToken)
                .Returns(refreshToken ?? Guid.NewGuid().ToString("D"));
            return redditAccessTokenMock; 
        }
    }
}