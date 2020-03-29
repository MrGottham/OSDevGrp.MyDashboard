using System;
using System.Runtime.Serialization;
using OSDevGrp.MyDashboard.Core.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Helpers.CookieHelper
{
    [DataContract]
    internal class MyRedditAccessToken : RedditAccessToken
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