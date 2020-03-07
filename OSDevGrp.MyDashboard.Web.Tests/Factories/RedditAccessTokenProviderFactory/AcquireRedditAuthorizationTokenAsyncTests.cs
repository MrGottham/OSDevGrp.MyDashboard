using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.RedditAccessTokenProviderFactory
{
    [TestClass]
    public class AcquireRedditAuthorizationTokenAsyncTests
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
        [ExpectedArgumentNullException("state")]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenStateIsNull_ThrowsArgumentNullException()
        {
            const string state = null;
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.AcquireRedditAuthorizationTokenAsync(state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenStateIsEmpty_ThrowsArgumentNullException()
        {
            string state = string.Empty;
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.AcquireRedditAuthorizationTokenAsync(state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenStateIsWhitespace_ThrowsArgumentNullException()
        {
            const string state = " ";
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.AcquireRedditAuthorizationTokenAsync(state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenStateIsWhitespaces_ThrowsArgumentNullException()
        {
            const string state = "  ";
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.AcquireRedditAuthorizationTokenAsync(state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("redirectUri")]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenRedirectUriIsNull_ThrowsArgumentNullException()
        {
            string state = Guid.NewGuid().ToString("D");
            const Uri redirectUri = null;

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.AcquireRedditAuthorizationTokenAsync(state, redirectUri);
        }

        [TestMethod]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenCalled_AssertAuthenticationRedditClientIdWasCalledOnConfiguration()
        {
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.AcquireRedditAuthorizationTokenAsync(state, redirectUri);

            _configurationMock.Verify(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)], Times.Once);
        }

        [TestMethod]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenCalled_AssertAcquireRedditAuthorizationTokenAsyncWasCalledOnDataProviderFactory()
        {
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            string clientId = Guid.NewGuid().ToString("D");
            IRedditAccessTokenProviderFactory sut = CreateSut(clientId: clientId);

            await sut.AcquireRedditAuthorizationTokenAsync(state, redirectUri);

            _dataProviderFactoryMock.Verify(m => m.AcquireRedditAuthorizationTokenAsync(
                    It.Is<string>(value => string.Compare(clientId, value, StringComparison.Ordinal) == 0),
                    It.Is<string>(value => string.Compare(state, value, StringComparison.Ordinal) == 0),
                    It.Is<Uri>(value => value == redirectUri)),
                Times.Once);
        }

        [TestMethod]
        public void AcquireRedditAuthorizationTokenAsync_WhenCalled_ReturnsTaskFromDataProviderFactory()
        {
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            Task<Uri> acquireRedditAuthorizationTokenTask = Task.Run<Uri>(() => new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}"));
            IRedditAccessTokenProviderFactory sut = CreateSut(acquireRedditAuthorizationTokenTask: acquireRedditAuthorizationTokenTask);

            Task<Uri> result = sut.AcquireRedditAuthorizationTokenAsync(state, redirectUri);

            Assert.AreEqual(acquireRedditAuthorizationTokenTask, result);           
        }

        private IRedditAccessTokenProviderFactory CreateSut(string clientId = null, Task<Uri> acquireRedditAuthorizationTokenTask = null)
        {
            _configurationMock.Setup(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)])
                .Returns(clientId ?? Guid.NewGuid().ToString("D"));

            _dataProviderFactoryMock.Setup(m => m.AcquireRedditAuthorizationTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Uri>()))
                .Returns(acquireRedditAuthorizationTokenTask ?? Task.Run<Uri>(() => new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}")));

            return new OSDevGrp.MyDashboard.Web.Factories.RedditAccessTokenProviderFactory(
                _configurationMock.Object,
                _dataProviderFactoryMock.Object);
        }
    }
}