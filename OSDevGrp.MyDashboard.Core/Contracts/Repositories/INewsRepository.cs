using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Contracts.Repositories
{
    public interface INewsRepository
    {
        Task<IEnumerable<INews>> GetNewsAsync();
    }
}