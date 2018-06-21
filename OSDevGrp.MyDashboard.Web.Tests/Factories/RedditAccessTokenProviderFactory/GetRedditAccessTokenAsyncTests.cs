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
    public class GetRedditAccessTokenAsyncTests
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
        [ExpectedArgumentNullException("code")]
        public void GetRedditAccessTokenAsync_WhenCodeIsNull_ThrowsArgumentNullException()
        {
            const string code = null;
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("code")]
        public void GetRedditAccessTokenAsync_WhenCodeIsEmpty_ThrowsArgumentNullException()
        {
            string code = string.Empty;
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("code")]
        public void GetRedditAccessTokenAsync_WhenCodeIsWhitespace_ThrowsArgumentNullException()
        {
            const string code = " ";
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("code")]
        public void GetRedditAccessTokenAsync_WhenCodeIsWhitespaces_ThrowsArgumentNullException()
        {
            const string code = "  ";
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("redirectUri")]
        public void GetRedditAccessTokenAsync_WhenRedirectUriIsNull_ThrowsArgumentNullException()
        {
            string code = Guid.NewGuid().ToString("D");
            const Uri redirectUri = null;

            IRedditAccessTokenProviderFactory sut = CreateSut();

            sut.GetRedditAccessTokenAsync(code, redirectUri);
        }

        [TestMethod]
        public void GetRedditAccessTokenAsync_WhenCalled_AssertAuthenticationRedditClientIdWasCalledOnConfiguration()
        {
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            Task<IRedditAccessToken> result = sut.GetRedditAccessTokenAsync(code, redirectUri);
            result.Wait();

            _configurationMock.Verify(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)], Times.Once);
        }

        [TestMethod]
        public void GetRedditAccessTokenAsync_WhenCalled_AssertAuthenticationRedditClientSecretWasCalledOnConfiguration()
        {
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            Task<IRedditAccessToken> result = sut.GetRedditAccessTokenAsync(code, redirectUri);
            result.Wait();

            _configurationMock.Verify(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientSecret", value, StringComparison.Ordinal) == 0)], Times.Once);
        }

        [TestMethod]
        public void GetRedditAccessTokenAsync_WhenCalled_AssertGetRedditAccessTokenAsyncWasCalledOnDataProviderFactory()
        {
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            IRedditAccessTokenProviderFactory sut = CreateSut(clientId: clientId, clientSecret: clientSecret);

            Task<IRedditAccessToken> result = sut.GetRedditAccessTokenAsync(code, redirectUri);
            result.Wait();

            _dataProviderFactoryMock.Verify(m => m.GetRedditAccessTokenAsync(
                    It.Is<string>(value => string.Compare(clientId, value, StringComparison.Ordinal) == 0),
                    It.Is<string>(value => string.Compare(clientSecret, value, StringComparison.Ordinal) == 0),
                    It.Is<string>(value => string.Compare(code, value, StringComparison.Ordinal) == 0),
                    It.Is<Uri>(value => value == redirectUri)),
                Times.Once);
        }

        [TestMethod]
        public void GetRedditAccessTokenAsync_WhenCalled_ReturnsTaskFromDataProviderFactory()
        {
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            Task<IRedditAccessToken> getRedditAccessTokenTask = Task.Run<IRedditAccessToken>(() => CreateRedditAccessToken());
            IRedditAccessTokenProviderFactory sut = CreateSut(getRedditAccessTokenTask: getRedditAccessTokenTask);

            Task<IRedditAccessToken> result = sut.GetRedditAccessTokenAsync(code, redirectUri);
            result.Wait();

            Assert.AreEqual(getRedditAccessTokenTask, result);           
        }

        private IRedditAccessTokenProviderFactory CreateSut(string clientId = null, string clientSecret = null, Task<IRedditAccessToken> getRedditAccessTokenTask = null)
        {
            _configurationMock.Setup(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientId", value, StringComparison.Ordinal) == 0)])
                .Returns(clientId ?? Guid.NewGuid().ToString("D"));
            _configurationMock.Setup(m => m[It.Is<string>(value => string.Compare("Authentication:Reddit:ClientSecret", value, StringComparison.Ordinal) == 0)])
                .Returns(clientSecret ?? Guid.NewGuid().ToString("D"));

            _dataProviderFactoryMock.Setup(m => m.GetRedditAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Uri>()))
                .Returns(getRedditAccessTokenTask ?? Task.Run<IRedditAccessToken>(() => CreateRedditAccessToken()));

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