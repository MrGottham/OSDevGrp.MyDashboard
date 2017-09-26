using System;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Core.Contracts.Infrastructure
{
    public interface IExceptionHandler
    {
        Task HandleAsync(Exception exception);

        Task HandleAsync(AggregateException exception); 
    }
}
