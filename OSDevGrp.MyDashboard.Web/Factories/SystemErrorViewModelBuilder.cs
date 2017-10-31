using System;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class SystemErrorViewModelBuilder : ViewModelBuilderBase<SystemErrorViewModel, ISystemError>
    {
        #region Methods

        protected override SystemErrorViewModel Build(ISystemError systemError)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}