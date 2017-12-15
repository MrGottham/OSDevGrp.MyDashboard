using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Factories;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Factories.DataProviderFactory
{
    [TestClass]
    public class GetRedditAccessTokenAsyncTests
    {
        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void GetRedditAccessTokenAsync_WhenClientIdIsNull_ThrowsArgumentNullException()
        {
            const string clientId = null;
            string clientSecret = Guid.NewGuid().ToString("D");
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void GetRedditAccessTokenAsync_WhenClientIdIsEmpty_ThrowsArgumentNullException()
        {
            string clientId = string.Empty;
            string clientSecret = Guid.NewGuid().ToString("D");
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void GetRedditAccessTokenAsync_WhenClientIdIsWhitespace_ThrowsArgumentNullException()
        {
            const string clientId = " ";
            string clientSecret = Guid.NewGuid().ToString("D");
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void GetRedditAccessTokenAsync_WhenClientIdIsWhitespaces_ThrowsArgumentNullException()
        {
            const string clientId = "  ";
            string clientSecret = Guid.NewGuid().ToString("D");
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientSecret")]
        public void GetRedditAccessTokenAsync_WhenClientSecretIsNull_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string clientSecret = null;
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientSecret")]
        public void GetRedditAccessTokenAsync_WhenClientSecretIsEmpty_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = string.Empty;
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientSecret")]
        public void GetRedditAccessTokenAsync_WhenClientSecretIsWhitespace_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string clientSecret = " ";
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientSecret")]
        public void GetRedditAccessTokenAsync_WhenClientSecretIsWhitespaces_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string clientSecret = "  ";
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("code")]
        public void GetRedditAccessTokenAsync_WhenCodeIsNull_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            const string code = null;
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("code")]
        public void GetRedditAccessTokenAsync_WhenCodeIsEmpty_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            string code = string.Empty;
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("code")]
        public void GetRedditAccessTokenAsync_WhenCodeIsWhitespace_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            const string code = " ";
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("code")]
        public void GetRedditAccessTokenAsync_WhenCodeIsWhitespaces_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            const string code = "  ";
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("redirectUri")]
        public void GetRedditAccessTokenAsync_WhenRedirectUriIsNull_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            string code = Guid.NewGuid().ToString("D");
            const Uri redirectUri = null;

            IDataProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri);
        }

        [TestMethod]
        [ExpectedAggregateException(typeof(UnauthorizedAccessException), "Unable to get the access token from Reddit.")]
        public void GetRedditAccessTokenAsync_WhenCalled_ThrowsUnauthorizedAccessException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            string code = Guid.NewGuid().ToString("D");
             Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            Task<IRedditAccessToken> getRedditAccessTokenTask = sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri);
            getRedditAccessTokenTask.Wait();
        }

        private IDataProviderFactory CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Factories.DataProviderFactory();
        }
    }
}