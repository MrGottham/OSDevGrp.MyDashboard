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
                return string.IsNullOrWhiteSpace(BannerImageUrlAsString) == false ? new Uri(BannerImageUrlAsString) : null;
            }
        }

        [DataMember(Name = "banner_img", IsRequired = false)]
        protected string BannerImageUrlAsString { get; set; }

        [IgnoreDataMember]
        public Uri HeaderImageUrl 
        { 
            get
            {
                return string.IsNullOrWhiteSpace(HeaderImageUrlAsString) == false ? new Uri(HeaderImageUrlAsString) : null;
            }
        }

        [DataMember(Name = "header_img", IsRequired = false)]
        protected string HeaderImageUrlAsString { get; set; }

        [IgnoreDataMember]
        public Uri IconImageUrl 
        { 
            get
            {
                return string.IsNullOrWhiteSpace(IconImageUrlAsString) == false ? new Uri(IconImageUrlAsString) : null;
            }
        }

        [DataMember(Name = "icon_img", IsRequired = false)]
        protected string IconImageUrlAsString { get; set; }

        [DataMember(Name = "user_is_banned", IsRequired = true)]
        public bool UserIsBanned { get; protected set; }

        [DataMember(Name = "over18", IsRequired = true)]
        public bool Over18 { get; protected set; }

        [DataMember(Name = "subscribers", IsRequired = true)]
        public long Subscribers { get; protected set; }
        
        public Uri Url
        { 
            get
            {
                return new Uri($"http://www.reddit.com{UrlAsString}");
            }
        }

        [DataMember(Name = "url", IsRequired = true)]
        protected string UrlAsString { get; set; }

        #endregion
    }
}