using System;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Core.Contracts.Repositories
{
    public interface IExceptionRepository
    {
        Task AddAsync(Exception exception);
    }
}