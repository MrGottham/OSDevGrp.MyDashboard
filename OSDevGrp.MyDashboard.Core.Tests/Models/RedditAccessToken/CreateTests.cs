using System;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Tests.Models.RedditAccessToken
{
    [TestClass]
    public class CreateTests
    {
        #region Private variables

        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        public void Create_WhenBase64IsNull_ThrowsArgumentNullException()
        {
            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => OSDevGrp.MyDashboard.Core.Models.RedditAccessToken.Create((string) null));

            Assert.AreEqual("base64", result.ParamName);
        }
        
        [TestMethod]
        public void Create_WhenBase64IsEmpty_ThrowsArgumentNullException()
        {
            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => OSDevGrp.MyDashboard.Core.Models.RedditAccessToken.Create(string.Empty));

            Assert.AreEqual("base64", result.ParamName);
        }
        
        [TestMethod]
        public void Create_WhenBase64IsWhitespace_ThrowsArgumentNullException()
        {
            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => OSDevGrp.MyDashboard.Core.Models.RedditAccessToken.Create(" "));

            Assert.AreEqual("base64", result.ParamName);
        }
        
        [TestMethod]
        public void Create_WhenBase64IsWhitespaces_ThrowsArgumentNullException()
        {
            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => OSDevGrp.MyDashboard.Core.Models.RedditAccessToken.Create("  "));

            Assert.AreEqual("base64", result.ParamName);
        }
        
        [TestMethod]
        public void Create_WhenCalledWithBase64_ReturnsRedditAccessToken()
        {
            string accessToken = Guid.NewGuid().ToString("D");
            string tokenType = Guid.NewGuid().ToString("D");
            int expiresIn = _random.Next(300, 900);
            string scope = Guid.NewGuid().ToString("D");
            string refreshToken = Guid.NewGuid().ToString("D");
            DateTime received = DateTime.UtcNow.AddSeconds(_random.Next(1, 30)*-1);
            IRedditAccessToken redditAccessToken = CreateSut(accessToken, tokenType, expiresIn, scope, refreshToken, received);

            IRedditAccessToken result = OSDevGrp.MyDashboard.Core.Models.RedditAccessToken.Create(redditAccessToken.ToBase64());
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.AccessToken);
            Assert.AreEqual(accessToken, result.AccessToken);
            Assert.IsNotNull(result.TokenType);
            Assert.AreEqual(tokenType, result.TokenType);
            Assert.AreEqual(received.ToLocalTime().AddSeconds(expiresIn).ToString(), result.Expires.ToString());
            Assert.IsNotNull(result.Scope);
            Assert.AreEqual(scope, result.Scope);
            Assert.IsNotNull(result.RefreshToken);
            Assert.AreEqual(refreshToken, result.RefreshToken);
        }

        private IRedditAccessToken CreateSut(string accessToken = null, string tokenType = null, int? expiresIn = null, string scope = null, string refreshToken = null, DateTime? received = null)
        {
            return new MyRedditToken(
                accessToken ?? Guid.NewGuid().ToString("D"),
                tokenType ?? Guid.NewGuid().ToString("D"),
                expiresIn ?? _random.Next(60, 300),
                scope ?? Guid.NewGuid().ToString("D"),
                refreshToken ?? Guid.NewGuid().ToString("D"),
                received ?? DateTime.UtcNow.AddSeconds(_random.Next(1, 30)*-1));
        }

        [DataContract]
        private class MyRedditToken : OSDevGrp.MyDashboard.Core.Models.RedditAccessToken
        {
            #region Constructors

            public MyRedditToken()
            {
            }

            public MyRedditToken(string accessToken, string tokenType, int expiresIn, string scope, string refreshToken, DateTime received)
            {
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    throw new ArgumentNullException(nameof(accessToken));
                }
                if (string.IsNullOrWhiteSpace(tokenType))
                {
                    throw new ArgumentNullException(nameof(tokenType));
                }
                if (string.IsNullOrWhiteSpace(scope))
                {
                    throw new ArgumentNullException(nameof(scope));
                }
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    throw new ArgumentNullException(nameof(refreshToken));
                }

                AccessToken = accessToken;
                TokenType = tokenType;
                ExpiresIn = expiresIn;
                Scope = scope;
                RefreshToken = refreshToken;
                Received = received;
            }

            #endregion
        }
   }
}
