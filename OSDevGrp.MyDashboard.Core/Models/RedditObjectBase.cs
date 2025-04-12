using OSDevGrp.MyDashboard.Core.Contracts.Models;
using System.Runtime.Serialization;

namespace OSDevGrp.MyDashboard.Core.Models
{
    [DataContract]
    public abstract class RedditObjectBase : IRedditObject
    {
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