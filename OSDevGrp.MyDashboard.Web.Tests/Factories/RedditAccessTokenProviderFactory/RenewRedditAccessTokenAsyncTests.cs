using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.RedditAccessTokenProviderFactory
{
    [TestClass]
    public class RenewRedditAccessTokenAsyncTests
    {
        #region Private variables

        private Mock<IConfiguration> _configurationMock;
        private Mock<IDataProviderFactory> _dataProviderFactoryMock;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _configurationMock = new Mock<IConfiguration>();
            _dataProviderFactoryMock = new Mock<IDataProviderFactory>();
        }

        [TestMethod]
        [ExpectedArgumentNullException("refreshToken")]
        public void RenewRedditAccessTokenAsync_WhenRefreshTokenIsNull_ThrowsArgumentNullException()
        {
            const string refreshToken = null;

            IRedditAccessTokenProviderFactory sut = CreateSut();

            sut.RenewRedditAccessTokenAsync(refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("refreshToken")]
        public void RenewRedditAccessTokenAsync_WhenRefreshTokenIsEmpty_ThrowsArgumentNullException()
        {
            string refreshToken = string.Empty;

            IRedditAccessTokenProviderFactory sut = CreateSut();

            sut.RenewRedditAccessTokenAsync(refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("refreshToken")]
        public void RenewRedditAccessTokenAsync_WhenRefreshTokenIsWhitespace_ThrowsArgumentNullException()
        {
            const string refreshToken = " ";

            IRedditAccessTokenProviderFactory sut = CreateSut();

            sut.RenewRedditAccessTokenAsync(refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("refreshToken")]
        public void RenewRedditAccessTokenAsync_WhenRefreshTokenIsWhitespaces_ThrowsArgumentNullException()
        {
            const string refreshToken = "  ";

            IRedditAccessTokenProviderFactory sut = CreateSut();

            sut.RenewRedditAccessTokenAsync(refreshToken);
        }

        [TestMethod]
        public void RenewRedditAccessTokenAsync_WhenCalled_AssertAuthenticationRedditClientIdWasCalledOnConfiguration()
        {
            string refreshToken = Guid.NewGuid().ToString("D");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            Task<IRedditAccessToken> result = sut.RenewRedditAccessTokenAsync(refreshToken);
            result.Wait();

            _configurationMock.Verify(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)], Times.Once);
        }

        [TestMethod]
        public void RenewRedditAccessTokenAsync_WhenCalled_AssertAuthenticationRedditClientSecretWasCalledOnConfiguration()
        {
            string refreshToken = Guid.NewGuid().ToString("D");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            Task<IRedditAccessToken> result = sut.RenewRedditAccessTokenAsync(refreshToken);
            result.Wait();

            _configurationMock.Verify(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientSecret", value, StringComparison.Ordinal) == 0)], Times.Once);
        }

        [TestMethod]
        public void RenewRedditAccessTokenAsync_WhenCalled_AssertRenewRedditAccessTokenAsyncWasCalledOnDataProviderFactory()
        {
            string refreshToken = Guid.NewGuid().ToString("D");

            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            IRedditAccessTokenProviderFactory sut = CreateSut(clientId: clientId, clientSecret: clientSecret);

            Task<IRedditAccessToken> result = sut.RenewRedditAccessTokenAsync(refreshToken);
            result.Wait();

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
            result.Wait();

            Assert.AreEqual(renewRedditAccessTokenTask, result);           
        }

        private IRedditAccessTokenProviderFactory CreateSut(string clientId = null, string clientSecret = null, Task<IRedditAccessToken> renewRedditAccessTokenTask = null)
        {
            _configurationMock.Setup(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)])
                .Returns(clientId ?? Guid.NewGuid().ToString("D"));
            _configurationMock.Setup(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientSecret", value, StringComparison.Ordinal) == 0)])
                .Returns(clientSecret ?? Guid.NewGuid().ToString("D"));

            _dataProviderFactoryMock.Setup(m => m.RenewRedditAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(renewRedditAccessTokenTask ?? Task.Run<IRedditAccessToken>(() => CreateRedditAccessToken()));

            return new OSDevGrp.MyDashboard.Web.Factories.RedditAccessTokenProviderFactory(
                _configurationMock.Object,
                _dataProviderFactoryMock.Object);
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