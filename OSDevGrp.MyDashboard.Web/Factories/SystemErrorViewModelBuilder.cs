using System;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class SystemErrorViewModelBuilder : IViewModelBuilder<SystemErrorViewModel, ISystemError>
    {
        #region Methods

        public Task<SystemErrorViewModel> BuildAsync(ISystemError systemError)
        {
            if (systemError == null)
            {
                throw new ArgumentNullException(nameof(systemError));
            }
            
            throw new NotImplementedException();
        }

        #endregion
    }
}