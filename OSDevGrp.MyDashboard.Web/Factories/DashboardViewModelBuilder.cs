using System;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class DashboardViewModelBuilder : ViewModelBuilderBase<DashboardViewModel, IDashboard>
    {
        #region Methods

        protected override DashboardViewModel Build(IDashboard dashboard)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}