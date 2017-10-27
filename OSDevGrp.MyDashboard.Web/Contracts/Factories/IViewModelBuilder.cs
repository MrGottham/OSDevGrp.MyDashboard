using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Contracts.Factories
{
    public interface IViewModelBuilder<TOutput, TInput> where TOutput : class, IViewModel where TInput : class 
    {
        Task<TOutput> BuildAsync(TInput input);
    }
}