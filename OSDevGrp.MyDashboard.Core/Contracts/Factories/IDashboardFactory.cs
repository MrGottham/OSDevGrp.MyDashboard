using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Contracts.Factories
{
    public interface IDashboardFactory
    {
        Task<IDashboard> BuildAsync(IDashboardSettings dashboardSettings);
    }
}
