using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Factories.DataProviderFactory
{
    [TestClass]
    public class RenewRedditAccessTokenAsyncTests
    {
        #region Private variables

        private Mock<IRandomizer> _randomizerMock;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _randomizerMock = new Mock<IRandomizer>();
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void RenewRedditAccessTokenAsync_WhenClientIdIsNull_ThrowsArgumentNullException()
        {
            const string clientId = null;
            string clientSecret = Guid.NewGuid().ToString("D");
            string refreshToken = Guid.NewGuid().ToString("D");

            IDataProviderFactory sut = CreateSut();

            sut.RenewRedditAccessTokenAsync(clientId, clientSecret, refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void RenewRedditAccessTokenAsync_WhenClientIdIsEmpty_ThrowsArgumentNullException()
        {
            string clientId = string.Empty;
            string clientSecret = Guid.NewGuid().ToString("D");
            string refreshToken = Guid.NewGuid().ToString("D");

            IDataProviderFactory sut = CreateSut();

            sut.RenewRedditAccessTokenAsync(clientId, clientSecret, refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void RenewRedditAccessTokenAsync_WhenClientIdIsWhitespace_ThrowsArgumentNullException()
        {
            const string clientId = " ";
            string clientSecret = Guid.NewGuid().ToString("D");
            string refreshToken = Guid.NewGuid().ToString("D");

            IDataProviderFactory sut = CreateSut();

            sut.RenewRedditAccessTokenAsync(clientId, clientSecret, refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void RenewRedditAccessTokenAsync_WhenClientIdIsWhitespaces_ThrowsArgumentNullException()
        {
            const string clientId = "  ";
            string clientSecret = Guid.NewGuid().ToString("D");
            string refreshToken = Guid.NewGuid().ToString("D");

            IDataProviderFactory sut = CreateSut();

            sut.RenewRedditAccessTokenAsync(clientId, clientSecret, refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientSecret")]
        public void RenewRedditAccessTokenAsync_WhenClientSecretIsNull_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string clientSecret = null;
            string refreshToken = Guid.NewGuid().ToString("D");

            IDataProviderFactory sut = CreateSut();

            sut.RenewRedditAccessTokenAsync(clientId, clientSecret, refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientSecret")]
        public void RenewRedditAccessTokenAsync_WhenClientSecretIsEmpty_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = string.Empty;
            string refreshToken = Guid.NewGuid().ToString("D");

            IDataProviderFactory sut = CreateSut();

            sut.RenewRedditAccessTokenAsync(clientId, clientSecret, refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientSecret")]
        public void RenewRedditAccessTokenAsync_WhenClientSecretIsWhitespace_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string clientSecret = " ";
            string refreshToken = Guid.NewGuid().ToString("D");

            IDataProviderFactory sut = CreateSut();

            sut.RenewRedditAccessTokenAsync(clientId, clientSecret, refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientSecret")]
        public void RenewRedditAccessTokenAsync_WhenClientSecretIsWhitespaces_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string clientSecret = "  ";
            string refreshToken = Guid.NewGuid().ToString("D");

            IDataProviderFactory sut = CreateSut();

            sut.RenewRedditAccessTokenAsync(clientId, clientSecret, refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("refreshToken")]
        public void RenewRedditAccessTokenAsync_WhenRefreshTokenIsNull_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            const string refreshToken = null;

            IDataProviderFactory sut = CreateSut();

            sut.RenewRedditAccessTokenAsync(clientId, clientSecret, refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("refreshToken")]
        public void RenewRedditAccessTokenAsync_WhenRefreshTokenIsEmpty_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            string refreshToken = string.Empty;

            IDataProviderFactory sut = CreateSut();

            sut.RenewRedditAccessTokenAsync(clientId, clientSecret, refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("refreshToken")]
        public void RenewRedditAccessTokenAsync_WhenRefreshTokenIsWhitespace_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            const string refreshToken = " ";

            IDataProviderFactory sut = CreateSut();

            sut.RenewRedditAccessTokenAsync(clientId, clientSecret, refreshToken);
        }

        [TestMethod]
        [ExpectedArgumentNullException("refreshToken")]
        public void RenewRedditAccessTokenAsync_WhenRefreshTokenIsWhitespaces_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            const string refreshToken = "  ";

            IDataProviderFactory sut = CreateSut();

            sut.RenewRedditAccessTokenAsync(clientId, clientSecret, refreshToken);
        }

        [TestMethod]
        [ExpectedAggregateException(typeof(UnauthorizedAccessException), "You are not authorized to perform this operation against Reddit.")]
        public void RenewRedditAccessTokenAsync_WhenCalled_ThrowsUnauthorizedAccessException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            string refreshToken = Guid.NewGuid().ToString("D");

            IDataProviderFactory sut = CreateSut();

            Task<IRedditAccessToken> renewRedditAccessTokenTask = sut.RenewRedditAccessTokenAsync(clientId, clientSecret, refreshToken);
            renewRedditAccessTokenTask.Wait();
        }

        private IDataProviderFactory CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Factories.DataProviderFactory(_randomizerMock.Object);
        }
    }
}