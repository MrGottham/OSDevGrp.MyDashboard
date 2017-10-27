using System;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class DashboardViewModelBuilder : IViewModelBuilder<DashboardViewModel, IDashboard>
    {
        #region Methods

        public Task<DashboardViewModel> BuildAsync(IDashboard dashboard)
        {
            if (dashboard == null)
            {
                throw new ArgumentNullException(nameof(dashboard));
            }
            
            throw new NotImplementedException();
        }

        #endregion
    }
}