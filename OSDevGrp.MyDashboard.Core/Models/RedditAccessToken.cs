using System;
using System.IO;
using System.Runtime.Serialization;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Utilities;

namespace OSDevGrp.MyDashboard.Core.Models
{
    [DataContract]
    public class RedditAccessToken : RedditObjectBase, IRedditAccessToken
    {
        #region Properties

        [DataMember(Name = "access_token", IsRequired = true)]
        public string AccessToken { get; protected set; }

        [DataMember(Name = "token_type", IsRequired = true)]
        public string TokenType { get; protected set; }

        [DataMember(Name = "expires_in", IsRequired = true)]
        protected int ExpiresIn { get; set; }
        
        [IgnoreDataMember]
        public DateTime Expires
        {
            get
            {
                if (Received.HasValue == false)
                {
                    throw new Exception("This Access Token has not been received from Reddit.");
                }
                return Received.Value.ToLocalTime().AddSeconds(ExpiresIn); 
            }
        }

        [DataMember(Name = "scope", IsRequired = true)]
        public string Scope { get; protected set; }

        [DataMember(Name = "refresh_token", IsRequired = true)]
        public string RefreshToken { get; protected set; }

        [DataMember(Name = "received_at", IsRequired = false)]
        protected DateTime? Received { get; set; }

        [DataMember(Name = "error", IsRequired = false)]
        internal string Error { get; set; }

        #endregion

        #region Methods

        public string ToBase64()
        {
            return JsonSerialization.ToBase64(this); 
        }

        public static IRedditAccessToken Create(string base64)
        {
            return JsonSerialization.FromBase64<RedditAccessToken>(base64);
        }

        public static IRedditAccessToken Create(Stream stream)
        {
            return JsonSerialization.FromStream<RedditAccessToken>(stream);
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext streamingContext)
        {
            if (Received.HasValue == false)
            {
                throw new Exception("This Access Token has not been received from Reddit.");
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            if (Received.HasValue == false)
            {
                Received = DateTime.UtcNow;
            }
        }

        #endregion
    }
}