using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    [DataContract]
    public class RedditList<TRedditThing> : RedditObjectBase, IRedditList<TRedditThing> where TRedditThing : IRedditThing
    {
        #region Constructors

        public RedditList() : base()
        {
        }

        internal RedditList(string kind, RedditListData<TRedditThing> data) : base()
        {
            if (string.IsNullOrWhiteSpace(kind))
            {
                throw new ArgumentNullException(nameof(kind));
            }
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Kind = kind;
            Data = data;
        }

        #endregion

        #region Properties

        [IgnoreDataMember]
        public string Before 
        { 
            get
            {
                return Data != null ? Data.Before : null;
            }
        }

        [IgnoreDataMember]
        public string After 
        { 
            get
            {
                return Data != null ? Data.After : null;
            }
        }
        
        [DataMember(Name = "kind", IsRequired = true)]
        internal string Kind { get; set; }

        [DataMember(Name = "data", IsRequired = true)]
        internal RedditListData<TRedditThing> Data { get; set; }

        #endregion

        #region Methods

        IEnumerator<TRedditThing> IEnumerable<TRedditThing>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IRedditList<TCastTo> As<TCastTo>() where TCastTo : class, IRedditThing
        {
            return new RedditList<TCastTo>(
                Kind,
                Data != null ? Data.As<TCastTo>() : new RedditListData<TCastTo>(null, null, null, 0, new RedditListChildren<TCastTo>(new List<RedditListChild<TCastTo>>(0))));
        }

        private IEnumerator<TRedditThing> GetEnumerator()
        {
            if (Data == null || Data.Children == null)
            {
                IList<TRedditThing> emptyList = new List<TRedditThing>(0);
                return emptyList.GetEnumerator();
            }
            return Data.Children.Select(m => m.Data).GetEnumerator();
        }

        #endregion
    }

    [DataContract]
    internal class RedditListData<TRedditThing> : RedditObjectBase where TRedditThing : IRedditThing
    {
        #region Constructors

        internal RedditListData() : base()
        {
        }

        internal RedditListData(string before, string after, string whitelistStatus, int dist, RedditListChildren<TRedditThing> children) : base()
        {
            if (children == null)
            {
                throw new ArgumentNullException(nameof(children));
            }

            Before = before;
            After = after;
            WhitelistStatus = whitelistStatus;
            Dist = dist;
            Children = children;
        }

        #endregion

        #region Properties

        [DataMember(Name = "before", IsRequired = true)]
        internal string Before { get; set; }

        [DataMember(Name = "after", IsRequired = true)]
        internal string After { get; set; }

        [DataMember(Name = "whitelist_status", IsRequired = false)]
        internal string WhitelistStatus { get; set; }

        [DataMember(Name = "dist", IsRequired = true)]
        internal int Dist { get; set; }

        [DataMember(Name = "children", IsRequired = true)]
        internal RedditListChildren<TRedditThing> Children { get; set; }

        #endregion

        #region Methods

        internal RedditListData<TCastTo> As<TCastTo>() where TCastTo : class, IRedditThing
        {
            
            return new RedditListData<TCastTo>(
                Before,
                After,
                WhitelistStatus,
                Dist,
                Children != null ? Children.As<TCastTo>() : new RedditListChildren<TCastTo>(new List<RedditListChild<TCastTo>>(0)));
        }

        #endregion
    }

    [CollectionDataContract]
    internal class RedditListChildren<TRedditThing> : List<RedditListChild<TRedditThing>> where TRedditThing : IRedditThing
    {
        #region Constructors

        internal RedditListChildren() : base()
        {
        }

        internal RedditListChildren(IEnumerable<RedditListChild<TRedditThing>> source) : base(source)
        {
        }

        #endregion

        #region Methods

        internal RedditListChildren<TCastTo> As<TCastTo>() where TCastTo : class, IRedditThing
        {
            return new RedditListChildren<TCastTo>(this.Select(item => item.As<TCastTo>()));
        }

        #endregion
    }

    [DataContract]
    internal class RedditListChild<TRedditThing> : RedditObjectBase where TRedditThing : IRedditThing
    {
        #region Constructors

        internal RedditListChild() : base()
        {
        }

        internal RedditListChild(string kind, TRedditThing data) : base()
        {
            if (string.IsNullOrWhiteSpace(kind))
            {
                throw new ArgumentNullException(nameof(kind));
            }
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Kind = kind;
            Data = data;
        }
        
        #endregion

        #region Properties

        [DataMember(Name = "kind", IsRequired = true)]
        internal string Kind { get; set; }

        [DataMember(Name = "data", IsRequired = true)]
        internal TRedditThing Data { get; set; }

        #endregion

        #region Methods

        internal RedditListChild<TCastTo> As<TCastTo>() where TCastTo : class, IRedditThing
        {
            return new RedditListChild<TCastTo>(
                Kind, 
                Data as TCastTo);
        }

        #endregion
    }
}