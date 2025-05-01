using OSDevGrp.MyDashboard.Core.Contracts.Models;
using System.Runtime.Serialization;

namespace OSDevGrp.MyDashboard.Core.Models
{
    [DataContract]
    public class RedditUser : RedditCreatedThingBase, IRedditUser
    {
        #region Properties

        [IgnoreDataMember]
        public override string Identifier => Id;

        [IgnoreDataMember]
        public override string FullName => $"{Kind}_{Identifier}";

        [IgnoreDataMember]
        public override string Kind => "t2";

        [DataMember(Name = "comment_karma", IsRequired = true)]

        public int CommentKarma { get; protected set; }

        [DataMember(Name = "link_karma", IsRequired = true)]
        public int LinkKarma { get; protected set; }

        [DataMember(Name = "name", IsRequired = true)]
        public string UserName { get; protected set; }

        [DataMember(Name = "over_18", IsRequired = true)]
        public bool Over18 { get; protected set; }

        [DataMember(Name = "id", IsRequired = true)]
        protected string Id { get; set; }

        #endregion

        #region Methods

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            UserName = UnescapeRedditString(UserName);
        }

        #endregion
    }
}