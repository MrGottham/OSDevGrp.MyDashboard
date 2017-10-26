using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Contracts.Factories
{
    public interface IDashboardContentBuilder
    {
        bool ShouldBuild(IDashboardSettings dashboardSettings);

        Task BuildAsync(IDashboardSettings dashboardSettings, IDashboard dashboard); 
    }
}
