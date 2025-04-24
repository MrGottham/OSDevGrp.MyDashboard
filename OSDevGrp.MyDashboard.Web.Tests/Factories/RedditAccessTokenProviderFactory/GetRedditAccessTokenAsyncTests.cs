using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Options;
using System;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.RedditAccessTokenProviderFactory
{
    [TestClass]
    public class GetRedditAccessTokenAsyncTests
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
        [ExpectedArgumentNullException("code")]
        public async Task GetRedditAccessTokenAsync_WhenCodeIsNull_ThrowsArgumentNullException()
        {
            const string code = null;
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.GetRedditAccessTokenAsync(code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("code")]
        public async Task GetRedditAccessTokenAsync_WhenCodeIsEmpty_ThrowsArgumentNullException()
        {
            string code = string.Empty;
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.GetRedditAccessTokenAsync(code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("code")]
        public async Task GetRedditAccessTokenAsync_WhenCodeIsWhitespace_ThrowsArgumentNullException()
        {
            const string code = " ";
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.GetRedditAccessTokenAsync(code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("code")]
        public async Task GetRedditAccessTokenAsync_WhenCodeIsWhitespaces_ThrowsArgumentNullException()
        {
            const string code = "  ";
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.GetRedditAccessTokenAsync(code, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("redirectUri")]
        public async Task GetRedditAccessTokenAsync_WhenRedirectUriIsNull_ThrowsArgumentNullException()
        {
            string code = Guid.NewGuid().ToString("D");
            const Uri redirectUri = null;

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.GetRedditAccessTokenAsync(code, redirectUri);
        }

        [TestMethod]
        public async Task GetRedditAccessTokenAsync_WhenCalled_AssertValueWasCalledTwiceOnRedditOptions()
        {
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IRedditAccessTokenProviderFactory sut = CreateSut();

            await sut.GetRedditAccessTokenAsync(code, redirectUri);

            _redditOptionsMock.Verify(m => m.Value, Times.Exactly(2));
        }

        [TestMethod]
        public async Task GetRedditAccessTokenAsync_WhenCalled_AssertGetRedditAccessTokenAsyncWasCalledOnDataProviderFactory()
        {
            string code = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            string clientId = Guid.NewGuid().ToString("D");
            string clientSecret = Guid.NewGuid().ToString("D");
            IRedditAccessTokenProviderFactory sut = CreateSut(clientId: clientId, clientSecret: clientSecret);

            await sut.GetRedditAccessTokenAsync(code, redirectUri);

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

            Assert.AreEqual(getRedditAccessTokenTask, result);           
        }

        private IRedditAccessTokenProviderFactory CreateSut(string clientId = null, string clientSecret = null, Task<IRedditAccessToken> getRedditAccessTokenTask = null)
        {
            _redditOptionsMock.Setup(m => m.Value)
                .Returns(CreateRedditOptions(clientId, clientSecret));

            _dataProviderFactoryMock.Setup(m => m.GetRedditAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Uri>()))
                .Returns(getRedditAccessTokenTask ?? Task.Run<IRedditAccessToken>(() => CreateRedditAccessToken()));

            return new Web.Factories.RedditAccessTokenProviderFactory(
                _redditOptionsMock.Object,
                _dataProviderFactoryMock.Object);
        }

        private RedditOptions CreateRedditOptions(string clientId = null, string clientSecret = null)
        {
            return new RedditOptions
            {
                ClientId = clientId ?? Guid.NewGuid().ToString("D"),
                ClientSecret = clientSecret ?? Guid.NewGuid().ToString("D")
            };
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