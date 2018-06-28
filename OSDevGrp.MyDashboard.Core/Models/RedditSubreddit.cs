using System;
using System.Runtime.Serialization;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    [DataContract]
    public class RedditSubreddit : RedditCreatedThingBase, IRedditSubreddit
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
                return "t5";
            }
        }

        [DataMember(Name = "id", IsRequired = true)]
        protected string Id { get; set; }

        [DataMember(Name = "display_name", IsRequired = true)]
        public string DisplayName { get; protected set; }

        [DataMember(Name = "display_name_prefixed", IsRequired = true)]
        public string DisplayNamePrefixed { get; protected set; }

        [DataMember(Name = "title", IsRequired = true)]
        public string Title { get; protected set; }

        [DataMember(Name = "header_title", IsRequired = true)]
        public string HeaderTitle { get; protected set; }

        [DataMember(Name = "description", IsRequired = false)]
        public string DescriptionAsText { get; protected set; }

        [DataMember(Name = "description_html", IsRequired = false)]
        public string DescriptionAsHtml { get; protected set; }

        [DataMember(Name = "public_description", IsRequired = false)]
        public string PublicDescriptionAsText { get; protected set; }

        [DataMember(Name = "public_description_html", IsRequired = false)]
        public string PublicDescriptionAsHtml { get; protected set; }

        [IgnoreDataMember]
        public Uri BannerImageUrl 
        { 
            get
            {
                if (string.IsNullOrWhiteSpace(BannerImageUrlAsString) || Uri.IsWellFormedUriString(BannerImageUrlAsString, UriKind.Absolute) == false)
                {
                    return null;
                }
                return new Uri(BannerImageUrlAsString);
            }
        }

        [DataMember(Name = "banner_img", IsRequired = false)]
        protected string BannerImageUrlAsString { get; set; }

        [IgnoreDataMember]
        public Uri HeaderImageUrl 
        { 
            get
            {
                if (string.IsNullOrWhiteSpace(HeaderImageUrlAsString) || Uri.IsWellFormedUriString(HeaderImageUrlAsString, UriKind.Absolute) == false)
                {
                    return null;
                }
                return new Uri(HeaderImageUrlAsString);
            }
        }

        [DataMember(Name = "header_img", IsRequired = false)]
        protected string HeaderImageUrlAsString { get; set; }

        [IgnoreDataMember]
        public Uri IconImageUrl 
        { 
            get
            {
                if (string.IsNullOrWhiteSpace(IconImageUrlAsString) || Uri.IsWellFormedUriString(IconImageUrlAsString, UriKind.Absolute) == false)
                {
                    return null;
                }
                return new Uri(IconImageUrlAsString);
            }
        }

        [DataMember(Name = "icon_img", IsRequired = false)]
        protected string IconImageUrlAsString { get; set; }

        [DataMember(Name = "user_is_banned", IsRequired = true)]
        public bool UserIsBanned { get; protected set; }

        [IgnoreDataMember]
        public bool UserBanned
        {
            get
            {
                return UserIsBanned;
            }
        }

        [DataMember(Name = "over18", IsRequired = true)]
        public bool Over18 { get; protected set; }

        [DataMember(Name = "subscribers", IsRequired = true)]
        public long Subscribers { get; protected set; }
        
        [IgnoreDataMember]
        public Uri Url
        { 
            get
            {
                if (Uri.IsWellFormedUriString(UrlAsString, UriKind.Absolute))
                {
                    return new Uri(UrlAsString);
                }
                return new Uri($"http://www.reddit.com{UrlAsString}");
            }
        }

        [DataMember(Name = "url", IsRequired = true)]
        protected string UrlAsString { get; set; }

        #endregion

        #region Methods

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            DisplayName = UnescapeRedditString(DisplayName);
            DisplayNamePrefixed = UnescapeRedditString(DisplayNamePrefixed);
            Title = UnescapeRedditString(Title);
            HeaderTitle = UnescapeRedditString(HeaderTitle);
            DescriptionAsText = UnescapeRedditString(DescriptionAsText);
            DescriptionAsHtml = UnescapeRedditString(DescriptionAsHtml);
            PublicDescriptionAsText = UnescapeRedditString(PublicDescriptionAsText);
            PublicDescriptionAsHtml = UnescapeRedditString(PublicDescriptionAsHtml);
            BannerImageUrlAsString = UnescapeRedditString(BannerImageUrlAsString);
            HeaderImageUrlAsString = UnescapeRedditString(HeaderImageUrlAsString);
            IconImageUrlAsString = UnescapeRedditString(IconImageUrlAsString);
            UrlAsString = UnescapeRedditString(UrlAsString);
        }

        #endregion
    }
}