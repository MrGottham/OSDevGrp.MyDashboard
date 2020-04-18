using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public  class DashboardModelExporter : ModelExporterBase<DashboardExportModel, IDashboard> 
    {
        #region Private variables

        private readonly IModelExporter<DashboardItemExportModel, INews> _newsModelExporter;
        private readonly IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser> _redditAuthenticatedUserModelExporter;
        private readonly IModelExporter<DashboardItemExportModel, IRedditSubreddit> _redditSubredditModelExporter;
        private readonly IModelExporter<DashboardItemExportModel, IRedditLink> _redditLinkModelExporter;

        #endregion

        #region Constructor

        public DashboardModelExporter(IEnumerable<IModelExporter> modelExporterCollection, IExceptionHandler exceptionHandler)
            : base(exceptionHandler)
        {
            if (modelExporterCollection == null)
            {
                throw new ArgumentNullException(nameof(modelExporterCollection));
            }

            IModelExporter[] modelExporterArray = modelExporterCollection.ToArray();

            _newsModelExporter = modelExporterArray.OfType<IModelExporter<DashboardItemExportModel, INews>>().Single();
            _redditAuthenticatedUserModelExporter = modelExporterArray.OfType<IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser>>().Single();
            _redditSubredditModelExporter = modelExporterArray.OfType<IModelExporter<DashboardItemExportModel, IRedditSubreddit>>().Single();
            _redditLinkModelExporter = modelExporterArray.OfType<IModelExporter<DashboardItemExportModel, IRedditLink>>().Single();
        }

        #endregion

        #region Methods

        protected async override Task<DashboardExportModel> DoExportAsync(IDashboard dashboard)
        {
            if (dashboard == null)
            {
                throw new ArgumentException(nameof(dashboard));
            }

            List<Task<DashboardItemExportModel>> exportTaskCollection = new List<Task<DashboardItemExportModel>>();
            exportTaskCollection.AddRange(CreateExportTasks(dashboard.News));
            exportTaskCollection.AddRange(CreateExportTasks(dashboard.RedditAuthenticatedUser));
            exportTaskCollection.AddRange(CreateExportTasks(dashboard.RedditSubreddits));
            exportTaskCollection.AddRange(CreateExportTasks(dashboard.RedditLinks));

            DashboardItemExportModel[] dashboardItemExportModelCollection = await Task.WhenAll(exportTaskCollection);

            return new DashboardExportModel(dashboardItemExportModelCollection.Where(item => item != null).OrderByDescending(item => item.Timestamp).ToArray());
        }

        protected override DashboardExportModel Empty()
        {
            return new DashboardExportModel(new DashboardItemExportModel[0]);
        }

        private Task<DashboardItemExportModel>[] CreateExportTasks(IEnumerable<INews> newsCollection)
        {
            if (newsCollection == null)
            {
                return new Task<DashboardItemExportModel>[0];
            }

            return newsCollection.Select(_newsModelExporter.ExportAsync).ToArray();
        }

        private Task<DashboardItemExportModel>[] CreateExportTasks(IRedditAuthenticatedUser redditAuthenticatedUser)
        {
            if (redditAuthenticatedUser == null)
            {
                return new Task<DashboardItemExportModel>[0];
            }

            return new Task<DashboardItemExportModel>[1]
            {
                _redditAuthenticatedUserModelExporter.ExportAsync(redditAuthenticatedUser)
            };
        }

        private Task<DashboardItemExportModel>[] CreateExportTasks(IEnumerable<IRedditSubreddit> redditSubredditCollection)
        {
            if (redditSubredditCollection == null)
            {
                return new Task<DashboardItemExportModel>[0];
            }

            return redditSubredditCollection.Select(_redditSubredditModelExporter.ExportAsync).ToArray();
        }

        private Task<DashboardItemExportModel>[] CreateExportTasks(IEnumerable<IRedditLink> redditLinkCollection)
        {
            if (redditLinkCollection == null)
            {
                return new Task<DashboardItemExportModel>[0];
            }

            return redditLinkCollection.Select(_redditLinkModelExporter.ExportAsync).ToArray();
        }

        #endregion
    }
}