using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Options;
using System;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.RedditAccessTokenProviderFactory
{
    [TestClass]
    public class AcquireRedditAuthorizationTokenAsyncTests
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
        public async Task AcquireRedditAuthorizationTokenAsync_WhenCalled_AssertValueWasCalledOnRedditOptions()
        {
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.AcquireRedditAuthorizationTokenAsync(state, redirectUri);

            _redditOptionsMock.Verify(m => m.Value, Times.Once);
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
            _redditOptionsMock.Setup(m => m.Value)
                .Returns(CreateRedditOptions(clientId));

            _dataProviderFactoryMock.Setup(m => m.AcquireRedditAuthorizationTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Uri>()))
                .Returns(acquireRedditAuthorizationTokenTask ?? Task.Run<Uri>(() => new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}")));

            return new Web.Factories.RedditAccessTokenProviderFactory(
                _redditOptionsMock.Object,
                _dataProviderFactoryMock.Object);
        }

        private RedditOptions CreateRedditOptions(string clientId = null)
        {
            return new RedditOptions
            {
                ClientId = clientId ?? Guid.NewGuid().ToString("D"),
                ClientSecret = Guid.NewGuid().ToString("D")
            };
        }
    }
}