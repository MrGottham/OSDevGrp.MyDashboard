using System;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Models.DashboardSettingsViewModel
{
    [TestClass]
    public class ToDashboardSettingsTests
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
        public void ToDashboardSettings_WhenCalled_ReturnsInitializedDashboardSettings()
        {
            int numberOfNews = _random.Next(25, 50);
            bool useReddit = _random.Next(100) > 50;
            string redditAccessToken = CreateReddditAccessToken().ToBase64();
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut(numberOfNews, useReddit, redditAccessToken);

            IDashboardSettings result = sut.ToDashboardSettings();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DashboardSettings));
            Assert.AreEqual(numberOfNews, result.NumberOfNews);
            Assert.AreEqual(useReddit, result.UseReddit);
            Assert.IsNotNull(result.RedditAccessToken);
            Assert.IsFalse(result.IncludeNsfwContent);
            Assert.IsFalse(result.OnlyNsfwContent);
        }

        [TestMethod]
        public void ToDashboardSettings_WhenCalledWhereRedditAccessTokenIsNull_ReturnsInitializedDashboardSettings()
        {
            const string redditAccessToken = null;
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut(redditAccessToken: redditAccessToken);

            IDashboardSettings result = sut.ToDashboardSettings();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DashboardSettings));
            Assert.IsNull(result.RedditAccessToken);
        }

        [TestMethod]
        public void ToDashboardSettings_WhenCalledWhereRedditAccessTokenIsEmpty_ReturnsInitializedDashboardSettings()
        {
            string redditAccessToken = string.Empty;
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut(redditAccessToken: redditAccessToken);

            IDashboardSettings result = sut.ToDashboardSettings();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DashboardSettings));
            Assert.IsNull(result.RedditAccessToken);
        }

        [TestMethod]
        public void ToDashboardSettings_WhenCalledWhereRedditAccessTokenIsWhitespace_ReturnsInitializedDashboardSettings()
        {
            const string redditAccessToken = " ";
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut(redditAccessToken: redditAccessToken);

            IDashboardSettings result = sut.ToDashboardSettings();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DashboardSettings));
            Assert.IsNull(result.RedditAccessToken);
        }

        [TestMethod]
        public void ToDashboardSettings_WhenCalledWhereRedditAccessTokenIsWhitespaces_ReturnsInitializedDashboardSettings()
        {
            const string redditAccessToken = "  ";
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut(redditAccessToken: redditAccessToken);

            IDashboardSettings result = sut.ToDashboardSettings();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DashboardSettings));
            Assert.IsNull(result.RedditAccessToken);
        }

        [TestMethod]
        public void ToDashboardSettings_WhenCalledWhereRedditAccessTokenIsBase64_ReturnsInitializedDashboardSettings()
        {
            string accessToken = Guid.NewGuid().ToString("D");
            string tokenType = Guid.NewGuid().ToString("D");
            int expiresIn =  _random.Next(60, 300);
            string scope = Guid.NewGuid().ToString("D");
            string refreshToken = Guid.NewGuid().ToString("D");
            DateTime received = DateTime.UtcNow;
            string redditAccessToken = CreateReddditAccessToken(accessToken, tokenType, expiresIn, scope, refreshToken, received).ToBase64();
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut(redditAccessToken: redditAccessToken);

            IDashboardSettings result = sut.ToDashboardSettings();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DashboardSettings));
            Assert.IsNotNull(result.RedditAccessToken);
            Assert.IsInstanceOfType(result.RedditAccessToken, typeof(RedditAccessToken));
            Assert.AreEqual(accessToken, result.RedditAccessToken.AccessToken);
            Assert.AreEqual(tokenType, result.RedditAccessToken.TokenType);
            Assert.AreEqual(received.ToLocalTime().AddSeconds(expiresIn).ToString(), result.RedditAccessToken.Expires.ToString());
            Assert.AreEqual(scope, result.RedditAccessToken.Scope);
            Assert.AreEqual(refreshToken, result.RedditAccessToken.RefreshToken);
        }

        private OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel CreateSut(int? numberOfNews = null, bool? useReddit = null, string redditAccessToken = null)
        {
            return new OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel
            {
                NumberOfNews = numberOfNews ?? _random.Next(25, 50),
                UseReddit = useReddit ?? _random.Next(100) > 50,
                RedditAccessToken = redditAccessToken
            };
        }

        private IRedditAccessToken CreateReddditAccessToken(string accessToken = null, string tokenType = null, int? expiresIn = null, string scope = null, string refreshToken = null, DateTime? received = null)
        {
            return new MyRedditAccessToken(
                accessToken ?? Guid.NewGuid().ToString("D"),
                tokenType ?? Guid.NewGuid().ToString("D"),
                expiresIn ?? _random.Next(60, 300),
                scope ?? Guid.NewGuid().ToString("D"),
                refreshToken ?? Guid.NewGuid().ToString("D"),
                received ?? DateTime.UtcNow);
        }

        [DataContract]
        private class MyRedditAccessToken : RedditAccessToken
        {
            #region Constructor

            public MyRedditAccessToken(string accessToken, string tokenType, int expiresIn, string scope, string refreshToken, DateTime received)
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