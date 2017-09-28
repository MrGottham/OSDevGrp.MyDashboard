using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Contracts.Logic
{
    public interface INewsLogic
    {
        Task<IEnumerable<INews>> GetNewsAsync(int numberOfNews);
    }
}