using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;

namespace OSDevGrp.MyDashboard.Core.Tests.Factories.DataProviderFactory
{
    [TestClass]
    public class GetRedditAccessTokenAsyncTests
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
        public async Task GetRedditAccessTokenAsync_WhenClientIdIsNull_ThrowsArgumentNullException()
        {
            const string clientId = null;
            string clientSecret = Guid.NewGuid().ToString("D");
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri));

            Assert.AreEqual("clientId", result.ParamName);
        }

        [TestMethod]
        public async Task GetRedditAccessTokenAsync_WhenClientIdIsEmpty_ThrowsArgumentNullException()
        {
            string clientId = string.Empty;
            string clientSecret = Guid.NewGuid().ToString("D");
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri));

            Assert.AreEqual("clientId", result.ParamName);
        }

        [TestMethod]
        public async Task GetRedditAccessTokenAsync_WhenClientIdIsWhitespace_ThrowsArgumentNullException()
        {
            const string clientId = " ";
            string clientSecret = Guid.NewGuid().ToString("D");
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri));

            Assert.AreEqual("clientId", result.ParamName);
        }

        [TestMethod]
        public async Task GetRedditAccessTokenAsync_WhenClientIdIsWhitespaces_ThrowsArgumentNullException()
        {
            const string clientId = "  ";
            string clientSecret = Guid.NewGuid().ToString("D");
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri));

            Assert.AreEqual("clientId", result.ParamName);
        }

        [TestMethod]
        public async Task GetRedditAccessTokenAsync_WhenClientSecretIsNull_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string clientSecret = null;
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri));

            Assert.AreEqual("clientSecret", result.ParamName);
        }

        [TestMethod]
        public async Task GetRedditAccessTokenAsync_WhenClientSecretIsEmpty_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = string.Empty;
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri));

            Assert.AreEqual("clientSecret", result.ParamName);
        }

        [TestMethod]
        public async Task GetRedditAccessTokenAsync_WhenClientSecretIsWhitespace_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string clientSecret = " ";
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri));

            Assert.AreEqual("clientSecret", result.ParamName);
        }

        [TestMethod]
        public async Task GetRedditAccessTokenAsync_WhenClientSecretIsWhitespaces_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            const string clientSecret = "  ";
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri));

            Assert.AreEqual("clientSecret", result.ParamName);
        }

        [TestMethod]
        public async Task GetRedditAccessTokenAsync_WhenCodeIsNull_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            const string code = null;
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri));

            Assert.AreEqual("code", result.ParamName);
        }

        [TestMethod]
        public async Task GetRedditAccessTokenAsync_WhenCodeIsEmpty_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            string code = string.Empty;
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri));

            Assert.AreEqual("code", result.ParamName);
        }

        [TestMethod]
        public async Task GetRedditAccessTokenAsync_WhenCodeIsWhitespace_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            const string code = " ";
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri));

            Assert.AreEqual("code", result.ParamName);
        }

        [TestMethod]
        public async Task GetRedditAccessTokenAsync_WhenCodeIsWhitespaces_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            const string code = "  ";
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri));

            Assert.AreEqual("code", result.ParamName);
        }

        [TestMethod]
        public async Task GetRedditAccessTokenAsync_WhenRedirectUriIsNull_ThrowsArgumentNullException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            string code = Guid.NewGuid().ToString("D");
            const Uri redirectUri = null;

            IDataProviderFactory sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri));

            Assert.AreEqual("redirectUri", result.ParamName);
        }

        [TestMethod]
        public async Task GetRedditAccessTokenAsync_WhenCalled_ThrowsUnauthorizedAccessException()
        {
            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            string code = Guid.NewGuid().ToString("D");
             Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IDataProviderFactory sut = CreateSut();

            UnauthorizedAccessException result = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await sut.GetRedditAccessTokenAsync(clientId, clientSecret, code, redirectUri));

            Assert.AreEqual("You are not authorized to perform this operation against Reddit.", result.Message);
        }

        private IDataProviderFactory CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Factories.DataProviderFactory(_randomizerMock.Object);
        }
    }
}