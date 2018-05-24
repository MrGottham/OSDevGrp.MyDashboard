using System;
using System.Runtime.Serialization;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    [DataContract]
    public class RedditLink : RedditCreatedThingBase, IRedditLink
    {
        #region Properties
        
        [IgnoreDataMember]
        public override string Identifier 
        {
            get
            {
                return Id;
            }
        }

        [IgnoreDataMember]
        public override string FullName
        {
            get
            {
                return $"{Kind}_{Identifier}";
            }
        }

        [IgnoreDataMember]
        public override string Kind
        {
            get
            {
                return "t3";
            }
        }

        [DataMember(Name = "id", IsRequired = true)]
        protected string Id { get; set; }

        [IgnoreDataMember]
        public IRedditSubreddit Subreddit { get; internal set; }

        [DataMember(Name = "title", IsRequired = true)]
        public string Title { get; protected set; }

        [DataMember(Name = "selftext", IsRequired = false)]
        public string SelftextAsText { get; protected set; }

        [DataMember(Name = "selftext_html", IsRequired = false)]
        public string SelftextAsHtml { get; protected set; }

        [DataMember(Name = "author", IsRequired = true)]
        public string Author { get; protected set; }

        [IgnoreDataMember]        
        public Uri ThumbnailUrl 
        { 
            get
            {
                return new Uri(ThumbnailAsString);
            }
        }

        [DataMember(Name = "thumbnail", IsRequired = false)]
        protected string ThumbnailAsString { get; set; }

        [DataMember(Name = "over_18", IsRequired = true)]
        public bool Over18 { get; protected set; }

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

        #endregion
    }
}