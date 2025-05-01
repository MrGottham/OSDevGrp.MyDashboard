using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Options;
using System;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.RedditAccessTokenProviderFactory
{
    [TestClass]
    public class RenewRedditAccessTokenAsyncTests
    {
        #region Private variables

        private Mock<IOptions<RedditOptions>> _redditOptionsMock;
        private Mock<IDataProviderFactory> _dataProviderFactoryMock;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _redditOptionsMock = new Mock<IOptions<RedditOptions>>();
            _dataProviderFactoryMock = new Mock<IDataProviderFactory>();
        }

        [TestMethod]
        [ExpectedArgumentNullException("refreshToken")]
        public async Task RenewRedditAccessTokenAsync_WhenRefreshTokenIsNull_ThrowsArgumentNullException()
        {
            const string refreshToken = null;

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.RenewRedditAccessTokenAsync(refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("refreshToken")]
        public async Task RenewRedditAccessTokenAsync_WhenRefreshTokenIsEmpty_ThrowsArgumentNullException()
        {
            string refreshToken = string.Empty;

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.RenewRedditAccessTokenAsync(refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("refreshToken")]
        public async Task RenewRedditAccessTokenAsync_WhenRefreshTokenIsWhitespace_ThrowsArgumentNullException()
        {
            const string refreshToken = " ";

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.RenewRedditAccessTokenAsync(refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("refreshToken")]
        public async Task RenewRedditAccessTokenAsync_WhenRefreshTokenIsWhitespaces_ThrowsArgumentNullException()
        {
            const string refreshToken = "  ";

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.RenewRedditAccessTokenAsync(refreshToken);
        }

        [TestMethod]
        public async Task RenewRedditAccessTokenAsync_WhenCalled_AssertValueWasCalledTwiceOnRedditOptions()
        {
            string refreshToken = Guid.NewGuid().ToString("D");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.RenewRedditAccessTokenAsync(refreshToken);

            _redditOptionsMock.Verify(m => m.Value, Times.Exactly(2));
        }

        [TestMethod]
        public async Task RenewRedditAccessTokenAsync_WhenCalled_AssertRenewRedditAccessTokenAsyncWasCalledOnDataProviderFactory()
        {
            string refreshToken = Guid.NewGuid().ToString("D");

            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            IRedditAccessTokenProviderFactory sut = CreateSut(clientId: clientId, clientSecret: clientSecret);

            await sut.RenewRedditAccessTokenAsync(refreshToken);

            _dataProviderFactoryMock.Verify(m => m.RenewRedditAccessTokenAsync(
                    It.Is<string>(value => string.Compare(clientId, value, StringComparison.Ordinal) == 0),
                    It.Is<string>(value => string.Compare(clientSecret, value, StringComparison.Ordinal) == 0),
                    It.Is<string>(value => string.Compare(refreshToken, value, StringComparison.Ordinal) == 0)),
                Times.Once);
        }

        [TestMethod]
        public void RenewRedditAccessTokenAsync_WhenCalled_ReturnsTaskFromDataProviderFactory()
        {
            string refreshToken = Guid.NewGuid().ToString("D");

            Task<IRedditAccessToken> renewRedditAccessTokenTask = Task.Run<IRedditAccessToken>(() => CreateRedditAccessToken());
            IRedditAccessTokenProviderFactory sut = CreateSut(renewRedditAccessTokenTask: renewRedditAccessTokenTask);

            Task<IRedditAccessToken> result = sut.RenewRedditAccessTokenAsync(refreshToken);

            Assert.AreEqual(renewRedditAccessTokenTask, result);           
        }

        private IRedditAccessTokenProviderFactory CreateSut(string clientId = null, string clientSecret = null, Task<IRedditAccessToken> renewRedditAccessTokenTask = null)
        {
            _redditOptionsMock.Setup(m => m.Value)
                .Returns(CreateRedditOptions(clientId, clientSecret));

            _dataProviderFactoryMock.Setup(m => m.RenewRedditAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(renewRedditAccessTokenTask ?? Task.Run<IRedditAccessToken>(() => CreateRedditAccessToken()));

            return new Web.Factories.RedditAccessTokenProviderFactory(
                _redditOptionsMock.Object,
                _dataProviderFactoryMock.Object);
        }

        private RedditOptions CreateRedditOptions(string clientId = null, string clientSecret = null)
        {
            return new RedditOptions
            {
                ClientId = clientId ?? Guid.NewGuid().ToString("D"),
                ClientSecret = clientSecret ?? Guid.NewGuid().ToString("D")
            };
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
    }
}