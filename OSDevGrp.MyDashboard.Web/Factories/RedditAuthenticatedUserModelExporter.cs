using System;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public  class RedditAuthenticatedUserModelExporter : ModelExporterBase<DashboardItemExportModel, IRedditAuthenticatedUser> 
    {
        #region Constructor

        public RedditAuthenticatedUserModelExporter(IExceptionHandler exceptionHandler)
            : base(exceptionHandler)
        {
        }

        #endregion

        #region Methods

        protected override Task<DashboardItemExportModel> DoExportAsync(IRedditAuthenticatedUser redditAuthenticatedUser)
        {
            if (redditAuthenticatedUser == null)
            {
                throw new ArgumentNullException(nameof(redditAuthenticatedUser));
            }

            DashboardItemExportModel dashboardItem = new DashboardItemExportModel(redditAuthenticatedUser.FullName, redditAuthenticatedUser.CreatedTime, redditAuthenticatedUser.UserName)
            {
                Provider = "Reddit"
            };

            return Task.FromResult(dashboardItem);
        }

        #endregion
    }
}