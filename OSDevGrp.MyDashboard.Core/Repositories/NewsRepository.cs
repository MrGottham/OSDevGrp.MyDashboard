using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;

namespace OSDevGrp.MyDashboard.Core.Repositories
{
    public class NewsRepository : INewsRepository
    {
        #region Private variables

        private readonly IDataProviderFactory _dataProviderFactory;
        private readonly IExceptionHandler _exceptionHandler;

        #endregion

        #region Constructor

        public NewsRepository(IDataProviderFactory dataProviderFactory, IExceptionHandler exceptionHandler)
        {
            if (dataProviderFactory == null)
            {
                throw new ArgumentNullException(nameof(dataProviderFactory));
            }
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(exceptionHandler));
            }

            _dataProviderFactory = dataProviderFactory;
            _exceptionHandler = exceptionHandler;
        }

        #endregion

        #region Methods

        public Task<IEnumerable<INews>> GetNewsAsync()
        {
            return Task.Run<IEnumerable<INews>>(async () => 
            {
                try
                {
                    IEnumerable<INewsProvider> newsProviders = await _dataProviderFactory.GetNewsProvidersAsync();
                    if (newsProviders == null || newsProviders.Any() == false)
                    {
                        return new List<INews>(0);
                    }

                    Task<IEnumerable<INews>>[] getNewsFromNewsProviderTasks = newsProviders.Select(GetNewsFromNewsProviderAsync).ToArray();
                    Task.WaitAll(getNewsFromNewsProviderTasks);

                    return getNewsFromNewsProviderTasks
                        .Where(task => task.IsFaulted == false)
                        .SelectMany(task => task.Result);
                }
                catch (AggregateException ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                return new List<INews>(0);
            });
        }

        private Task<IEnumerable<INews>> GetNewsFromNewsProviderAsync(INewsProvider newsProvider)
        {
            if (newsProvider == null)
            {
                throw new ArgumentNullException(nameof(newsProvider));
            }

            return Task.Run<IEnumerable<INews>>(() => 
            {
                return new List<INews>(0);
            });
        }

        #endregion
    }
}