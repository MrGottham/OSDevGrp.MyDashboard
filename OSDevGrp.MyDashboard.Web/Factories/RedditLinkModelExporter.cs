using System;
using System.Linq;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public  class RedditLinkModelExporter : ModelExporterBase<DashboardItemExportModel, IRedditLink> 
    {
        #region Constructor

        public RedditLinkModelExporter(IExceptionHandler exceptionHandler)
            : base(exceptionHandler)
        {
        }

        #endregion

        #region Methods

        protected override Task<DashboardItemExportModel> DoExportAsync(IRedditLink redditLink)
        {
            if (redditLink == null)
            {
                throw new ArgumentNullException(nameof(redditLink));
            }

            DashboardItemExportModel dashboardItem = new DashboardItemExportModel(redditLink.FullName, redditLink.CreatedTime, redditLink.Title)
            {
                Details = GetValue(redditLink.SelftextAsText),
                Provider = GetValue(redditLink.Subreddit, subreddit => subreddit.FullName),
                Author = GetValue(redditLink.Author),
                SourceUrl = GetValue(redditLink.Url, url => url.AbsoluteUri),
                ImageUrl = GetValue(redditLink.Images, images => images.FirstOrDefault(image => image.Url != null)?.Url.AbsoluteUri) ?? GetValue(redditLink.ThumbnailUrl, thumbnailUrl => thumbnailUrl.AbsoluteUri)
            };

            return Task.FromResult(dashboardItem);
        }

        #endregion
    }
}