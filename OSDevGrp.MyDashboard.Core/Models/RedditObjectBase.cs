using System;
using System.Runtime.Serialization;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    [Serializable]
    [DataContract]
    public abstract class RedditObjectBase : IRedditObject
    {
        #region Constructors

        protected RedditObjectBase()
        {
        }

        protected RedditObjectBase(SerializationInfo info, StreamingContext context)
        {
        }

        #endregion

        #region Methods

        protected string UnescapeRedditString(string redditString)
        {
            if (string.IsNullOrWhiteSpace(redditString))
            {
                return redditString;
            }
            return redditString.Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("&amp;", "&");
        }

        #endregion
    }
}