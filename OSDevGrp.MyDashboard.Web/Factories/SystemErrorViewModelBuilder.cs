using System;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class SystemErrorViewModelBuilder : ViewModelBuilderBase<SystemErrorViewModel, ISystemError>
    {
        #region Private variables

        private readonly IHtmlHelper _htmlHelper;

        #endregion

        #region Constructor

        public SystemErrorViewModelBuilder(IHtmlHelper htmlHelper)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException(nameof(htmlHelper));
            }
            
            _htmlHelper = htmlHelper;
        }

        #endregion

        #region Methods

        protected override SystemErrorViewModel Build(ISystemError systemError)
        {
            return new SystemErrorViewModel
            {
                SystemErrorIdentifier = systemError.Identifier,
                Timestamp = systemError.Timestamp,
                Message = _htmlHelper.ConvertNewLines(systemError.Information),
                Details = _htmlHelper.ConvertNewLines(systemError.Details)
            };
        }

        #endregion
    }
}