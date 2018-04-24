using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Factories;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Factories.DataProviderFactory
{
    [TestClass]
    public class AcquireRedditAuthorizationTokenAsyncTests
    {
        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void AcquireRedditAuthorizationTokenAsync_WhenClientIdIsNull_ThrowsArgumentNullException()
        {
            const string clientId = null;
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{state}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void AcquireRedditAuthorizationTokenAsync_WhenClientIdIsEmpty_ThrowsArgumentNullException()
        {
            string clientId = string.Empty;
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{state}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void AcquireRedditAuthorizationTokenAsync_WhenClientIdIsWhitespace_ThrowsArgumentNullException()
        {
            const string clientId = " ";
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{state}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void AcquireRedditAuthorizationTokenAsync_WhenClientIdIsWhitespaces_ThrowsArgumentNullException()
        {
            const string clientId = "  ";
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{state}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public void AcquireRedditAuthorizationTokenAsync_WhenStateIsNull_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string state = null;
            Uri redirectUri = new Uri($"http://localhost/{clientId}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public void AcquireRedditAuthorizationTokenAsync_WhenStateIsEmpty_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string state = string.Empty;
            Uri redirectUri = new Uri($"http://localhost/{clientId}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public void AcquireRedditAuthorizationTokenAsync_WhenStateIsWhitespace_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string state = " ";
            Uri redirectUri = new Uri($"http://localhost/{clientId}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public void AcquireRedditAuthorizationTokenAsync_WhenStateIsWhitespaces_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string state = "  ";
            Uri redirectUri = new Uri($"http://localhost/{clientId}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("redirectUri")]
        public void AcquireRedditAuthorizationTokenAsync_WhenRedirectUrlIsNull_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");
            const Uri redirectUri = null;

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        public void AcquireRedditAuthorizationTokenAsync_WhenCalled_ReturnsUriForAcquiringRedditAuthorization()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");
            
            const string scope = "identity privatemessages mysubreddits read";
            Uri expectedUri = new Uri($"https://www.reddit.com/api/v1/authorize?client_id={clientId}&response_type=code&state={state}&redirect_uri={redirectUri.AbsoluteUri}&duration=permanent&scope={scope}");

            IDataProviderFactory sut = CreateSut();

            Task<Uri> acquireRedditAuthorizationTokenTask = sut.AcquireRedditAuthorizationTokenAsync(clientId, state, redirectUri);
            acquireRedditAuthorizationTokenTask.Wait();

            Assert.IsNotNull(acquireRedditAuthorizationTokenTask.Result);
            Assert.AreEqual(expectedUri.AbsoluteUri, acquireRedditAuthorizationTokenTask.Result.AbsoluteUri);
        }

        private IDataProviderFactory CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Factories.DataProviderFactory();
        }
    }
}