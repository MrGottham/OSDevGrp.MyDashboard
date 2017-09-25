using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Models;

namespace OSDevGrp.MyDashboard.Core.Factories
{
    public class DataProviderFactory : IDataProviderFactory
    {
        public Task<IEnumerable<IDataProvider>> GetNewsProvidersAsync()
        {
            throw new NotImplementedException();
        } 
    }
}