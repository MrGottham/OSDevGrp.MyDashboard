using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Models;
using System;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public  class NewsModelExporter : ModelExporterBase<DashboardItemExportModel, INews> 
    {
        #region Constructor

        public NewsModelExporter(IExceptionHandler exceptionHandler)
            : base(exceptionHandler)
        {
        }

        #endregion

        #region Methods

        protected override Task<DashboardItemExportModel> DoExportAsync(INews news)
        {
            if (news == null)
            {
                throw new ArgumentException(nameof(news));
            }

            DashboardItemExportModel dashboardItem = new DashboardItemExportModel(news.Identifier, news.Timestamp, news.Information)
            {
                Details = GetValue(news.Details),
                Provider = GetValue(news.Provider, provider => provider.Name),
                SourceUrl = GetValue(news.Link, link => link.AbsoluteUri),
                ImageUrl = GetValue(news.MediaUrl, mediaUrl => mediaUrl.AbsoluteUri),
                Author = GetValue(news.Author, author => author.Name)
            };

            return Task.FromResult(dashboardItem);
        }

        #endregion
    }
}