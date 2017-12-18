using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Tests.Models.RedditAccessToken
{
    [TestClass]
    public class ToBase64Tests
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
        public void ToBase64_WhenCalled_ReturnsBase64()
        {
            IRedditAccessToken sut = CreateSut();

            string result = sut.ToBase64();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Trim().Length % 4 == 0 && Regex.IsMatch(result.Trim(), @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None));
        }
 
        private IRedditAccessToken CreateSut()
        {
            int expiresIn = _random.Next(60, 300);
            
            return new MyRedditToken(
                Guid.NewGuid().ToString("D"),
                Guid.NewGuid().ToString("D"),
                 expiresIn,
                Guid.NewGuid().ToString("D"),
                Guid.NewGuid().ToString("D"),
                DateTime.UtcNow.AddSeconds(_random.Next(1, 30)*-1));
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
