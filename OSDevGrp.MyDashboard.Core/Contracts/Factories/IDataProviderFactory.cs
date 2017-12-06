using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Contracts.Factories
{
    public interface IDataProviderFactory
    {
        Task<IEnumerable<INewsProvider>> BuildNewsProvidersAsync();

        Task<Uri> AcquireRedditAccessTokenAsync(string clientId, string state, Uri redirectUri);
    }
}
