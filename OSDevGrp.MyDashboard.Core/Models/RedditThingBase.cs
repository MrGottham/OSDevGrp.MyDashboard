using OSDevGrp.MyDashboard.Core.Contracts.Models;
using System.Runtime.Serialization;

namespace OSDevGrp.MyDashboard.Core.Models
{
    [DataContract]
    public abstract class RedditThingBase : RedditObjectBase, IRedditThing
    {
        #region Properties

        [IgnoreDataMember]
        public abstract string Identifier { get; }

        [IgnoreDataMember]
        public abstract string FullName { get; }

        [IgnoreDataMember]
        public abstract string Kind { get; }

        [IgnoreDataMember]
        public virtual IRedditObject Data => this;

        #endregion
    }
}