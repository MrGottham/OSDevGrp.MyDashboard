using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class DashboardViewModelBuilder : ViewModelBuilderBase<DashboardViewModel, IDashboard>
    {
        #region Private variables

        private readonly IViewModelBuilder<InformationViewModel, INews> _newsToInformationViewModelBuilder;
        private readonly IViewModelBuilder<SystemErrorViewModel, ISystemError> _systemErrorViewModelBuilder;
        private readonly IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> _dashboardSettingsViewModelBuilder;
        private readonly IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser> _redditAuthenticatedUserToObjectViewModelBuilder;
        private readonly IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit> _redditSubredditToObjectViewModelBuilder;
        private readonly IViewModelBuilder<InformationViewModel, IRedditLink> _redditLinkToInformationViewModelBuilder;
        private readonly IHtmlHelper _htmlHelper;
        private readonly IHttpHelper _httpHelper;
        private readonly ICookieHelper _cookieHelper;

        #endregion

        #region Constructor

        public DashboardViewModelBuilder(IEnumerable<IViewModelBuilder> viewModelBuilderCollection, IHtmlHelper htmlHelper, IHttpHelper httpHelper, ICookieHelper cookieHelper)
        {
            if (viewModelBuilderCollection == null) 
            {
                throw new ArgumentNullException(nameof(viewModelBuilderCollection));
            }
            if (htmlHelper == null)
            {
                throw new ArgumentNullException(nameof(htmlHelper));
            }
            if (httpHelper == null)
            {
                throw new ArgumentNullException(nameof(httpHelper));
            }
            if (cookieHelper == null)
            {
                throw new ArgumentNullException(nameof(cookieHelper));
            }

            IViewModelBuilder[] viewModelBuilderArray = viewModelBuilderCollection.ToArray();

            _newsToInformationViewModelBuilder = viewModelBuilderArray.OfType<IViewModelBuilder<InformationViewModel, INews>>().Single();
            _systemErrorViewModelBuilder = viewModelBuilderArray.OfType<IViewModelBuilder<SystemErrorViewModel, ISystemError>>().Single();
            _dashboardSettingsViewModelBuilder = viewModelBuilderArray.OfType<IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings>>().Single();
            _redditAuthenticatedUserToObjectViewModelBuilder = viewModelBuilderArray.OfType<IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser>>().Single();
            _redditSubredditToObjectViewModelBuilder = viewModelBuilderArray.OfType<IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit>>().Single();
            _redditLinkToInformationViewModelBuilder = viewModelBuilderArray.OfType<IViewModelBuilder<InformationViewModel, IRedditLink>>().Single();
            _htmlHelper = htmlHelper;
            _httpHelper = httpHelper;
            _cookieHelper = cookieHelper;
        }

        #endregion

        #region Methods

        protected override DashboardViewModel Build(IDashboard dashboard)
        {
            IList<InformationViewModel> informationViewModelCollection = new List<InformationViewModel>();
            IList<SystemErrorViewModel> systemErrorViewModelCollection = new List<SystemErrorViewModel>();
            DashboardSettingsViewModel dashboardSettingsViewModel = null;
            ObjectViewModel<IRedditAuthenticatedUser> objectViewModelForRedditAuthenticatedUser = null;
            IList<ObjectViewModel<IRedditSubreddit>> objectViewModelForRedditSubredditCollection = new List<ObjectViewModel<IRedditSubreddit>>();
            object syncRoot = new object();

            try
            {
                Task handleInformationViewModelsForNewsTask = HandleAsync<InformationViewModel>(
                    dashboard, 
                    m => GenerateViewModelBuilderTaskArrayForCollection(m, n => n.News, _newsToInformationViewModelBuilder),
                    viewModel => AddViewModelToViewModelCollection(viewModel, informationViewModelCollection, syncRoot),
                    exception => HandleException(exception, systemErrorViewModelCollection, syncRoot));
                Task handleSystemErrorViewModelsTask = HandleAsync<SystemErrorViewModel>(
                    dashboard, 
                    m => GenerateViewModelBuilderTaskArrayForCollection(m, n => n.SystemErrors, _systemErrorViewModelBuilder),
                    viewModel => AddViewModelToViewModelCollection(viewModel, systemErrorViewModelCollection, syncRoot),
                    exception => HandleException(exception, systemErrorViewModelCollection, syncRoot));
                Task handleDashboardSettingsViewModelTask = HandleAsync<DashboardSettingsViewModel>(
                    dashboard,
                    m => GenerateViewModelBuilderTaskArrayForObject(m, n => n.Settings, _dashboardSettingsViewModelBuilder),
                    viewModel => dashboardSettingsViewModel = viewModel,
                    exception => HandleException(exception, systemErrorViewModelCollection, syncRoot));
                Task handleObjectViewModelForRedditAuthenticatedUserTask = HandleAsync<ObjectViewModel<IRedditAuthenticatedUser>>(
                    dashboard,
                    m => GenerateViewModelBuilderTaskArrayForObject(m, n => n.RedditAuthenticatedUser, _redditAuthenticatedUserToObjectViewModelBuilder),
                    viewModel => objectViewModelForRedditAuthenticatedUser = viewModel,
                    exception => HandleException(exception, systemErrorViewModelCollection, syncRoot));
                Task handleObjectViewModelsForRedditSubredditTask = HandleAsync<ObjectViewModel<IRedditSubreddit>>(
                    dashboard,
                    m => GenerateViewModelBuilderTaskArrayForCollection(m, n => n.RedditSubreddits, _redditSubredditToObjectViewModelBuilder),
                    viewModel => AddViewModelToViewModelCollection(viewModel, objectViewModelForRedditSubredditCollection, syncRoot),
                    exception => HandleException(exception, systemErrorViewModelCollection, syncRoot));
                Task handleInformationViewModelsForRedditLinkTask = HandleAsync<InformationViewModel>(
                    dashboard, 
                    m => GenerateViewModelBuilderTaskArrayForCollection(m, n => n.RedditLinks, _redditLinkToInformationViewModelBuilder),
                    viewModel => AddViewModelToViewModelCollection(viewModel, informationViewModelCollection, syncRoot),
                    exception => HandleException(exception, systemErrorViewModelCollection, syncRoot));
               Task.WhenAll(new[] {handleInformationViewModelsForNewsTask, handleSystemErrorViewModelsTask, handleDashboardSettingsViewModelTask, handleObjectViewModelForRedditAuthenticatedUserTask, handleObjectViewModelsForRedditSubredditTask, handleInformationViewModelsForRedditLinkTask})
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception ex)
            {
                HandleException(ex, systemErrorViewModelCollection, syncRoot);
            }

            IEnumerable<ImageViewModel<InformationViewModel>> latestInformationsWithImage = GetLatestInformationsWithImage(informationViewModelCollection, systemErrorViewModelCollection, syncRoot);

            DashboardViewModel dashboardViewModel = new DashboardViewModel
            {
                Informations = informationViewModelCollection.OrderByDescending(informationViewModel => informationViewModel.Timestamp).ToList(),
                LatestInformationsWithImage = latestInformationsWithImage.OrderByDescending(latestInformationWithImage => latestInformationWithImage.ViewModel.Timestamp).ToList(),
                SystemErrors = systemErrorViewModelCollection.OrderByDescending(systemErrorViewModel => systemErrorViewModel.Timestamp).ToList(),
                Settings = dashboardSettingsViewModel,
                RedditAuthenticatedUser = objectViewModelForRedditAuthenticatedUser,
                RedditSubreddits = objectViewModelForRedditSubredditCollection.OrderByDescending(objectViewModelForRedditSubreddit => objectViewModelForRedditSubreddit.Object.Subscribers).ToList()
            };
            dashboardViewModel.ApplyRules(dashboard.Rules, _cookieHelper);

            _cookieHelper.ToCookie(dashboardViewModel);

            return dashboardViewModel;
        }

        private async Task HandleAsync<TViewModel>(IDashboard dashboard, Func<IDashboard, Task<TViewModel>[]> taskArrayGenerator, Action<TViewModel> resultHandler, Action<Exception> exceptionHandler) where TViewModel : IViewModel
        {
            if (dashboard == null)
            {
                throw new ArgumentNullException(nameof(dashboard));
            }
            if (taskArrayGenerator == null)
            {
                throw new ArgumentNullException(nameof(taskArrayGenerator));
            }
            if (resultHandler == null)
            {
                throw new ArgumentNullException(nameof(resultHandler));
            }
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(exceptionHandler));
            }

            try
            {
                Task<TViewModel>[] taskArray = taskArrayGenerator(dashboard);
                await Task.WhenAll(taskArray);

                foreach (TViewModel viewModel in taskArray.Where(task => task.IsCompletedSuccessfully).Select(task => task.Result))
                {
                    resultHandler(viewModel);
                }
            }
            catch (Exception ex)
            {
                exceptionHandler(ex);
            }
        }

        private Task<TViewModel>[] GenerateViewModelBuilderTaskArrayForObject<TViewModel, TObject>(IDashboard dashboard, Func<IDashboard, TObject> objectGetter, IViewModelBuilder<TViewModel, TObject> viewModelBuilder) where TViewModel : class, IViewModel where TObject : class
        {
            if (dashboard == null)
            {
                throw new ArgumentNullException(nameof(dashboard));
            }
            if (objectGetter == null)
            {
                throw new ArgumentNullException(nameof(objectGetter));
            }
            if (viewModelBuilder == null)
            {
                throw new ArgumentNullException(nameof(viewModelBuilder));
            }

            TObject obj = objectGetter(dashboard);
            if (obj == null)
            {
                return new Task<TViewModel>[0];
            }

            return new[] {viewModelBuilder.BuildAsync(obj)};
        }

        private Task<TViewModel>[] GenerateViewModelBuilderTaskArrayForCollection<TViewModel, TObject>(IDashboard dashboard, Func<IDashboard, IEnumerable<TObject>> collectionGetter, IViewModelBuilder<TViewModel, TObject> viewModelBuilder) where TViewModel : class, IViewModel where TObject : class
        {
            if (dashboard == null)
            {
                throw new ArgumentNullException(nameof(dashboard));
            }
            if (collectionGetter == null)
            {
                throw new ArgumentNullException(nameof(collectionGetter));
            }
            if (viewModelBuilder == null)
            {
                throw new ArgumentNullException(nameof(viewModelBuilder));
            }

            IEnumerable<TObject> collection = collectionGetter(dashboard);
            if (collection == null)
            {
                return new Task<TViewModel>[0];
            }

            return collection.Select(item => viewModelBuilder.BuildAsync(item)).ToArray();
        }

        private void AddViewModelToViewModelCollection<TViewModel>(TViewModel viewModel, IList<TViewModel> viewModelCollection, object syncRoot) where TViewModel : class, IViewModel 
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }
            if (viewModelCollection == null)
            {
                throw new ArgumentNullException(nameof(viewModelCollection));
            }
            if (syncRoot == null)
            {
                throw new ArgumentNullException(nameof(syncRoot));
            }

            lock (syncRoot)
            {
                viewModelCollection.Add(viewModel);
            }
        }

        private IEnumerable<ImageViewModel<InformationViewModel>> GetLatestInformationsWithImage(IEnumerable<InformationViewModel> informationViewModelCollection, IList<SystemErrorViewModel> systemErrorViewModelCollection, object syncRoot)
        {
            if (informationViewModelCollection == null)
            {
                throw new ArgumentNullException(nameof(informationViewModelCollection));
            }
            if (systemErrorViewModelCollection == null)
            {
                throw new ArgumentNullException(nameof(systemErrorViewModelCollection));
            }
            if (syncRoot == null)
            {
                throw new ArgumentNullException(nameof(syncRoot));
            }

            try
            {
                Task<ImageViewModel<InformationViewModel>>[] getLatestInformationsWithImageTaskArray = informationViewModelCollection
                    .Where(informationViewModel => string.IsNullOrWhiteSpace(informationViewModel.ImageUrl) == false && Uri.IsWellFormedUriString(informationViewModel.ImageUrl, UriKind.Absolute))
                    .OrderByDescending(informationViewModel => informationViewModel.Timestamp)
                    .Take(7)
                    .Select(informationViewModel => Task.Run<ImageViewModel<InformationViewModel>>(async () => {
                        byte[] image = await _httpHelper.ReadAsync(new Uri(informationViewModel.ImageUrl));
                        return new ImageViewModel<InformationViewModel>(informationViewModel, image);
                    }))
                    .ToArray();
                Task.WhenAll(getLatestInformationsWithImageTaskArray)
                    .GetAwaiter()
                    .GetResult();

                return getLatestInformationsWithImageTaskArray
                    .Where(getLatestInformationsWithImageTask => getLatestInformationsWithImageTask.IsCompletedSuccessfully)
                    .Select(getLatestInformationsWithImageTask => getLatestInformationsWithImageTask.Result)
                    .ToList();
            }
            catch (Exception ex)
            {
                HandleException(ex, systemErrorViewModelCollection, syncRoot);
            }
            return new List<ImageViewModel<InformationViewModel>>(0);
        }

        private void HandleException(Exception exception, IList<SystemErrorViewModel> systemErrorViewModelCollection, object syncRoot)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            if (systemErrorViewModelCollection == null)
            {
                throw new ArgumentNullException(nameof(systemErrorViewModelCollection));
            }
            if (syncRoot == null)
            {
                throw new ArgumentNullException(nameof(syncRoot));
            }

            AggregateException aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                aggregateException.Handle(ex => 
                {
                    HandleException(ex, systemErrorViewModelCollection, syncRoot);
                    return true;
                });
                return;
            }

            SystemErrorViewModel systemErrorViewModel = new SystemErrorViewModel
            {
                SystemErrorIdentifier = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now,
                Message = _htmlHelper.ConvertNewLines(exception.Message),
                Details = _htmlHelper.ConvertNewLines(exception.StackTrace)
            };
            AddViewModelToViewModelCollection(systemErrorViewModel, systemErrorViewModelCollection, syncRoot);
        }

        #endregion
    }
}