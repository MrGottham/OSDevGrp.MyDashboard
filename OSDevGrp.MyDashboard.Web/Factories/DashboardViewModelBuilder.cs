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
        private readonly IHtmlHelper _htmlHelper;

        #endregion

        #region Constructor

        public DashboardViewModelBuilder(IViewModelBuilder<InformationViewModel, INews> newsToInformationViewModelBuilder, IViewModelBuilder<SystemErrorViewModel, ISystemError> systemErrorViewModelBuilder, IHtmlHelper htmlHelper)
        {
            if (newsToInformationViewModelBuilder == null) throw new ArgumentNullException(nameof(newsToInformationViewModelBuilder));
            if (systemErrorViewModelBuilder == null) throw new ArgumentNullException(nameof(systemErrorViewModelBuilder));
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));

            _newsToInformationViewModelBuilder = newsToInformationViewModelBuilder;
            _systemErrorViewModelBuilder = systemErrorViewModelBuilder;
            _htmlHelper = htmlHelper;
        }

        #endregion

        #region Methods

        protected override DashboardViewModel Build(IDashboard dashboard)
        {
            IList<InformationViewModel> informationViewModels = new List<InformationViewModel>();
            IList<SystemErrorViewModel> systemErrorViewModels = new List<SystemErrorViewModel>();
            object syncRoot = new object();

            try
            {
                Task handleInformationViewModelsTask = HandleAsync<InformationViewModel>(
                    dashboard, 
                    GenerateInformationViewModelBuilderTaskArray,
                    informationViewModel => AddInformationViewModel(informationViewModel, informationViewModels, syncRoot),
                    exception => HandleException(exception, systemErrorViewModels, syncRoot));
                Task handleSystemErrorViewModelsTask = HandleAsync<SystemErrorViewModel>(
                    dashboard, 
                    GenerateSystemErrorViewModelBuilderTaskArray,
                    systemErrorViewModel => AddSystemErrorViewModel(systemErrorViewModel, systemErrorViewModels, syncRoot),
                    exception => HandleException(exception, systemErrorViewModels, syncRoot));
                Task.WaitAll(new [] {handleInformationViewModelsTask, handleSystemErrorViewModelsTask});
            }
            catch (Exception ex)
            {
                HandleException(ex, systemErrorViewModels, syncRoot);
            }
            
            return new DashboardViewModel
            {
                Informations = informationViewModels.OrderByDescending(informationViewModel => informationViewModel.Timestamp),
                SystemErrors = systemErrorViewModels.OrderByDescending(systemErrorViewModel => systemErrorViewModel.Timestamp)
            };
        }

        private Task<InformationViewModel>[] GenerateInformationViewModelBuilderTaskArray(IDashboard dashboard)
        {
            if (dashboard == null)
            {
                throw new ArgumentNullException(nameof(dashboard));
            }

            if (dashboard.News == null)
            {
                return new Task<InformationViewModel>[0];
            }

            return dashboard.News
                .Select(news => _newsToInformationViewModelBuilder.BuildAsync(news))
                .ToArray();
        }

        private Task<SystemErrorViewModel>[] GenerateSystemErrorViewModelBuilderTaskArray(IDashboard dashboard)
        {
            if (dashboard == null)
            {
                throw new ArgumentNullException(nameof(dashboard));
            }

            if (dashboard.SystemErrors == null)
            {
                return new Task<SystemErrorViewModel>[0];
            }

            return dashboard.SystemErrors
                .Select(systemError => _systemErrorViewModelBuilder.BuildAsync(systemError))
                .ToArray();
        }

        private Task HandleAsync<TViewModel>(IDashboard dashboard, Func<IDashboard, Task<TViewModel>[]> taskArrayGenerator, Action<TViewModel> resultHandler, Action<Exception> exceptionHandler) where TViewModel : IViewModel
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

            return Task.Run(() => 
            {
                try
                {
                    Task<TViewModel>[] taskArray = taskArrayGenerator(dashboard);
                    Task.WaitAll(taskArray);

                    foreach (TViewModel viewModel in taskArray.Where(task => task.IsCompletedSuccessfully).Select(task => task.Result))
                    {
                        resultHandler(viewModel);
                    }
                }
                catch (Exception ex)
                {
                    exceptionHandler(ex);
                }
            });
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
            AddSystemErrorViewModel(systemErrorViewModel, systemErrorViewModelCollection, syncRoot);
        }

        private static void AddInformationViewModel(InformationViewModel informationViewModel, IList<InformationViewModel> informationViewModelCollection, object syncRoot)
        {
            if (informationViewModel == null)
            {
                throw new ArgumentNullException(nameof(informationViewModel));
            }
            if (informationViewModelCollection == null)
            {
                throw new ArgumentNullException(nameof(informationViewModelCollection));
            }
            if (syncRoot == null)
            {
                throw new ArgumentNullException(nameof(syncRoot));
            }

            lock (syncRoot)
            {
                informationViewModelCollection.Add(informationViewModel);
            }
        }

        private static void AddSystemErrorViewModel(SystemErrorViewModel systemErrorViewModel, IList<SystemErrorViewModel> systemErrorViewModelCollection, object syncRoot)
        {
            if (systemErrorViewModel == null)
            {
                throw new ArgumentNullException(nameof(systemErrorViewModel));
            }
            if (systemErrorViewModelCollection == null)
            {
                throw new ArgumentNullException(nameof(systemErrorViewModelCollection));
            }
            if (syncRoot == null)
            {
                throw new ArgumentNullException(nameof(syncRoot));
            }

            lock (syncRoot)
            {
                systemErrorViewModelCollection.Add(systemErrorViewModel);
            }
        }

        #endregion
    }
}