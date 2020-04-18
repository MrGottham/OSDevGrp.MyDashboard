using System;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public  class RedditSubredditModelExporter : ModelExporterBase<DashboardItemExportModel, IRedditSubreddit> 
    {
        #region Constructor

        public RedditSubredditModelExporter(IExceptionHandler exceptionHandler)
            : base(exceptionHandler)
        {
        }

        #endregion

        #region Methods

        protected override Task<DashboardItemExportModel> DoExportAsync(IRedditSubreddit redditSubreddit)
        {
            if (redditSubreddit == null)
            {
                throw new ArgumentNullException(nameof(redditSubreddit));
            }

            DashboardItemExportModel dashboardItem = new DashboardItemExportModel(redditSubreddit.FullName, redditSubreddit.CreatedTime, redditSubreddit.Title)
            {
                Details = GetValue(redditSubreddit.DescriptionAsText) ?? GetValue(redditSubreddit.PublicDescriptionAsText),
                Provider = "Reddit",
                SourceUrl = GetValue(redditSubreddit.Url, url => url.AbsoluteUri),
                ImageUrl = GetValue(redditSubreddit.BannerImageUrl, imageUrl => imageUrl.AbsoluteUri) ?? GetValue(redditSubreddit.HeaderImageUrl, imageUrl => imageUrl.AbsoluteUri)
            };

            return Task.FromResult(dashboardItem);
        }

        #endregion
    }
}