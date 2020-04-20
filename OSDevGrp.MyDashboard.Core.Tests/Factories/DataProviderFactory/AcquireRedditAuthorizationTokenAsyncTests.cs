using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Factories.DataProviderFactory
{
    [TestClass]
    public class AcquireRedditAuthorizationTokenAsyncTests
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
        public async Task AcquireRedditAuthorizationTokenAsync_WhenClientIdIsNull_ThrowsArgumentNullException()
        {
            const string clientId = null;
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            await sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenClientIdIsEmpty_ThrowsArgumentNullException()
        {
            string clientId = string.Empty;
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            await sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenClientIdIsWhitespace_ThrowsArgumentNullException()
        {
            const string clientId = " ";
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            await sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenClientIdIsWhitespaces_ThrowsArgumentNullException()
        {
            const string clientId = "  ";
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            await sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenStateIsNull_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string state = null;
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            await sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenStateIsEmpty_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string state = string.Empty;
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            await sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenStateIsWhitespace_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string state = " ";
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            await sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenStateIsWhitespaces_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string state = "  ";
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            await sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("redirectUri")]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenRedirectUrlIsNull_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");
            const Uri redirectUri = null;

            IDataProviderFactory sut = CreateSut();

            await sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenCalled_ReturnsUriForAcquiringRedditAuthorization()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            const string scope = "identity privatemessages mysubreddits read";
            Uri expectedUri = new Uri($"https://www.reddit.com/api/v1/authorize?client_id={Uri.EscapeDataString(clientId)}&response_type=code&state={Uri.EscapeDataString(state)}&redirect_uri={redirectUri.AbsoluteUri}&duration=permanent&scope={Uri.EscapeDataString(scope)}");

            IDataProviderFactory sut = CreateSut();

            Uri result = await sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedUri.AbsoluteUri, result.AbsoluteUri);
        }

        [TestMethod]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenClientIdShouldBeEscaped_ReturnsUriForAcquiringRedditAuthorization()
        {
            string clientId = $"{Guid.NewGuid().ToString("D")}&{Guid.NewGuid().ToString("D")}";
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            const string scope = "identity privatemessages mysubreddits read";
            Uri expectedUri = new Uri($"https://www.reddit.com/api/v1/authorize?client_id={Uri.EscapeDataString(clientId)}&response_type=code&state={Uri.EscapeDataString(state)}&redirect_uri={redirectUri.AbsoluteUri}&duration=permanent&scope={Uri.EscapeDataString(scope)}");

            IDataProviderFactory sut = CreateSut();

            Uri result = await sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedUri.AbsoluteUri, result.AbsoluteUri);
        }

        [TestMethod]
        public async Task AcquireRedditAuthorizationTokenAsync_WhenStateShouldBeEscaped_ReturnsUriForAcquiringRedditAuthorization()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string state = $"{Guid.NewGuid().ToString("D")}&{Guid.NewGuid().ToString("D")}";
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            const string scope = "identity privatemessages mysubreddits read";
            Uri expectedUri = new Uri($"https://www.reddit.com/api/v1/authorize?client_id={Uri.EscapeDataString(clientId)}&response_type=code&state={Uri.EscapeDataString(state)}&redirect_uri={redirectUri.AbsoluteUri}&duration=permanent&scope={Uri.EscapeDataString(scope)}");

            IDataProviderFactory sut = CreateSut();

            Uri result = await sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedUri.AbsoluteUri, result.AbsoluteUri);
        }

        private IDataProviderFactory CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Factories.DataProviderFactory(_randomizerMock.Object);
        }
    }
}