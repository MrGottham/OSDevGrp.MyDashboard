using System.Runtime.Serialization;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    [DataContract]
    public class RedditAuthenticatedUser : RedditUser, IRedditAuthenticatedUser
    {
        #region Properties
        
        [DataMember(Name = "has_mail", IsRequired = true)]
        public bool HasUnreadMail { get; protected set; }
        
        [DataMember(Name = "has_mod_mail", IsRequired = true)]
        public bool HasUnreadModMail { get; protected set; }
        
        [DataMember(Name = "inbox_count", IsRequired = true)]
        public int InboxCount { get; protected set; }
        
        #endregion
    }
}