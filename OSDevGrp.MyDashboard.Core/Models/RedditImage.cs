using System;
using System.Runtime.Serialization;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    [DataContract]
    public class RedditImage : RedditObjectBase, IRedditImage
    {
        #region Properties

        [IgnoreDataMember]
        public Uri Url
        {
            get
            {
                if (Uri.IsWellFormedUriString(UrlAsString, UriKind.Absolute) == false)
                {
                    return null;
                }
                return new Uri(UrlAsString);
            }
        }

        [DataMember(Name = "url", IsRequired = true)]
        protected string UrlAsString { get; set; }

        [DataMember(Name = "width", IsRequired = true)]
        public int Width { get; protected set; }

        [DataMember(Name = "height", IsRequired = true)]
        public int Height { get; protected set; }

        #endregion
    }
}