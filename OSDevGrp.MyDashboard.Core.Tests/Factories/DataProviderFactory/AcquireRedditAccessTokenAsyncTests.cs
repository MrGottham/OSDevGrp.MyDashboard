using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Factories;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Factories.DataProviderFactory
{
    [TestClass]
    public class AcquireRedditAccessTokenAsyncTests
    {
        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void AcquireRedditAccessTokenAsync_WhenClientIdIsNull_ThrowsArgumentNullException()
        {
            const string clientId = null;
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{state}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAccessTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void AcquireRedditAccessTokenAsync_WhenClientIdIsEmpty_ThrowsArgumentNullException()
        {
            string clientId = string.Empty;
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{state}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAccessTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void AcquireRedditAccessTokenAsync_WhenClientIdIsWhitespace_ThrowsArgumentNullException()
        {
            const string clientId = " ";
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{state}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAccessTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void AcquireRedditAccessTokenAsync_WhenClientIdIsWhitespaces_ThrowsArgumentNullException()
        {
            const string clientId = "  ";
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{state}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAccessTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public void AcquireRedditAccessTokenAsync_WhenStateIsNull_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string state = null;
            Uri redirectUri = new Uri($"http://localhost/{clientId}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAccessTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public void AcquireRedditAccessTokenAsync_WhenStateIsEmpty_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string state = string.Empty;
            Uri redirectUri = new Uri($"http://localhost/{clientId}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAccessTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public void AcquireRedditAccessTokenAsync_WhenStateIsWhitespace_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string state = " ";
            Uri redirectUri = new Uri($"http://localhost/{clientId}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAccessTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("state")]
        public void AcquireRedditAccessTokenAsync_WhenStateIsWhitespaces_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string state = "  ";
            Uri redirectUri = new Uri($"http://localhost/{clientId}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAccessTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("redirectUri")]
        public void AcquireRedditAccessTokenAsync_WhenRedirectUrlIsNull_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");
            const Uri redirectUri = null;

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAccessTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        public void AcquireRedditAccessTokenAsync_WhenCalled_ReturnsUriForAcquiringRedditAccessToken()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            Task<Uri> acquireRedditAccessTokenTask = sut.AcquireRedditAccessTokenAsync(clientId, state, redirectUri);
            acquireRedditAccessTokenTask.Wait();

            Assert.IsNotNull(acquireRedditAccessTokenTask.Result);
            Assert.AreEqual($"https://www.reddit.com/api/v1/authorize?client_id={clientId}&response_type=code&state={state}&redirect_uri={redirectUri.AbsoluteUri}&duration=permanent&scope=identity", acquireRedditAccessTokenTask.Result.AbsoluteUri);
        }

        private IDataProviderFactory CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Factories.DataProviderFactory();
        }
    }
}