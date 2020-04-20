using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Contracts.Factories
{
    public interface IModelExporter
    {
    }

    public interface IModelExporter<TOutput, TInput> : IModelExporter where TOutput : class, IExportModel where TInput : class 
    {
        Task<TOutput> ExportAsync(TInput input);
    }
}