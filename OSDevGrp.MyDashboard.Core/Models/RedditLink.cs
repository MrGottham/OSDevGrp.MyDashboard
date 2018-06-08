using System;
using System.Collections.Generic;
using System.Linq;
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

        [DataMember(Name = "preview", IsRequired = false)]
        internal RedditLinkPreview Preview { get; set; }

        [IgnoreDataMember]
        public IEnumerable<IRedditImage> Images
        {
            get
            {
                return Preview != null ? Preview.AsRedditImageCollection() : null;
            }
        }

        [DataMember(Name = "banned_by", IsRequired = true)]
        public string BannedBy { get; protected set; }

        [IgnoreDataMember]
        public DateTime? BannedAtTime
        {
            get
            {
                if (BannedAtUtcTime.HasValue == false)
                {
                    return null;
                }
                return BannedAtUtcTime.Value.ToLocalTime();
            }
        }

        [IgnoreDataMember]
        public DateTime? BannedAtUtcTime
        {
            get
            {
                if (BannedAtUnixUtcTimestamp.HasValue == false)
                {
                    return null;
                }
                return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToInt64(BannedAtUnixUtcTimestamp.Value));
            }
        }

        [DataMember(Name = "banned_at_utc", IsRequired = true)]
        protected decimal? BannedAtUnixUtcTimestamp
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public bool UserBanned
        {
            get
            {
                return BannedAtUtcTime.HasValue;
            }
        }

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

    [DataContract]
    internal class RedditLinkPreview : RedditObjectBase
    {
        #region Properties

        [DataMember(Name = "images", IsRequired = false)]
        internal RedditLinkPreviewImageCollection Images { get; set; }

        #endregion

        #region Methods

        internal IEnumerable<IRedditImage> AsRedditImageCollection()
        {
            return Images != null ? Images.AsRedditImageCollection() : null;
        }

        #endregion
    }

    [CollectionDataContract]
    internal class RedditLinkPreviewImageCollection : List<RedditLinkPreviewImage>, IRedditObject
    {
        #region Methods

        internal IEnumerable<IRedditImage> AsRedditImageCollection()
        {
            return this
                .Where(redditLinkPreviewImage => redditLinkPreviewImage != null)
                .SelectMany(redditLinkPreviewImage => redditLinkPreviewImage.AsRedditImageCollection())
                .ToList();
        }

        #endregion
    }

    [DataContract]
    internal class RedditLinkPreviewImage : RedditObjectBase
    {
        #region Properties

        [DataMember(Name = "id", IsRequired = true)]
        internal string Id { get; set; }

        [DataMember(Name = "source", IsRequired = true)]
        internal RedditImage Source { get; set; }

        [DataMember(Name = "resolutions", IsRequired = false)]
        internal RedditLinkPreviewImageResolutionCollection Resolutions { get; set; }

        #endregion

        #region Methods

        internal IEnumerable<IRedditImage> AsRedditImageCollection()
        {
            List<IRedditImage> redditImageCollection = new List<IRedditImage> {Source};
            if (Resolutions != null && Resolutions.Any(redditImage => redditImage != null))
            {
                redditImageCollection.AddRange(Resolutions.AsRedditImageCollection().Where(redditImage => redditImage != null).ToList());
            }
            return redditImageCollection;
        }

        #endregion
    }

    [CollectionDataContract]
    internal class RedditLinkPreviewImageResolutionCollection : List<RedditImage>, IRedditObject
    {
        #region Methods

        internal IEnumerable<IRedditImage> AsRedditImageCollection()
        {
            return this;
        }

        #endregion
    }
}