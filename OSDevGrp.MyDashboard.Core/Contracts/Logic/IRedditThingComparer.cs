using System.Collections.Generic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Contracts.Logic
{
    public interface IRedditThingComparer<T> : IEqualityComparer<T> where T : IRedditThing
    {
    }
}