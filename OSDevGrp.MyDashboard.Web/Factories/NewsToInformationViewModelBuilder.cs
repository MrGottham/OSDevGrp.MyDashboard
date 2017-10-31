using System;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class NewsToInformationViewModelBuilder : ViewModelBuilderBase<InformationViewModel, INews>
    {
        #region Methods

        protected override InformationViewModel Build(INews news)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}