using System;
using System.Runtime.Serialization;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    [DataContract]
    public class RedditAccessToken : IRedditAccessToken
    {
        #region Properties

        [DataMember(Name = "access_token")]

        public string AccessToken { get; set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Name = "expires_in")]
        protected int ExpiresIn { get; set; }

        [IgnoreDataMember]
        public DateTime Expires { get; private set; }

        #endregion

        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        #region Methods

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            Expires = DateTime.Now.AddSeconds(ExpiresIn);
        }

        #endregion
    }
}