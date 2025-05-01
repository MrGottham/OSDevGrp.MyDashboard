using OSDevGrp.MyDashboard.Core.Contracts.Models;
using System;
using System.Runtime.Serialization;

namespace OSDevGrp.MyDashboard.Core.Models
{
    [DataContract]
    public abstract class RedditCreatedThingBase : RedditThingBase, IRedditCreatedThing
    {
        #region Properties

        [IgnoreDataMember]
        public virtual DateTime CreatedTime => CreatedUtcTime.ToLocalTime();

        [IgnoreDataMember]
        public virtual DateTime CreatedUtcTime => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToInt64(CreatedUnixUtcTimestamp));

        [IgnoreDataMember]
        public virtual DateTime Timestamp => CreatedTime;

        [DataMember(Name = "created", IsRequired = true)]
        protected virtual decimal CreatedUnixTimestamp
        {
            get;
            set;
        }

        [DataMember(Name = "created_utc", IsRequired = true)]
        protected virtual decimal CreatedUnixUtcTimestamp
        {
            get;
            set;
        }

        #endregion
    }
}