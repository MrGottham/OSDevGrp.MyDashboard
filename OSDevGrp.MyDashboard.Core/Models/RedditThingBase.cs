using System;
using System.Runtime.Serialization;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    [Serializable]
    [DataContract]
    public abstract class RedditThingBase : RedditObjectBase, IRedditThing
    {
        #region Constructors

        protected RedditThingBase()
        {
        }

        protected RedditThingBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region Properties

        [IgnoreDataMember]
        public abstract string Identifier { get; }

        [IgnoreDataMember]
        public abstract string FullName { get; }

        [IgnoreDataMember]
        public abstract string Kind { get; }

        [IgnoreDataMember]
        public virtual IRedditObject Data 
        { 
            get
            {
                return this;
            }
        }

        #endregion
    }
}