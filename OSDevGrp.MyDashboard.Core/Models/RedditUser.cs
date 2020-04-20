using System;
using System.Runtime.Serialization;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    [Serializable]
    [DataContract]
    public class RedditUser : RedditCreatedThingBase, IRedditUser
    {
        #region Constructors

        public RedditUser()
        {
        }

        protected RedditUser(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

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
                return "t2";
            }
        }

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