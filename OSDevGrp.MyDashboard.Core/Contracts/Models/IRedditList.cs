using System.Collections.Generic;

namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IRedditList<TRedditThing> : IRedditObject, IEnumerable<TRedditThing> where TRedditThing : IRedditThing
    {
        string Before { get; }

        string After { get; }

        IRedditList<TCastTo> As<TCastTo>() where TCastTo : class, IRedditThing;
    }
}