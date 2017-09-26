using System;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;

namespace OSDevGrp.MyDashboard.Core.Infrastructure
{
    public class ExceptionHandler : IExceptionHandler
    {
        #region Private variables

        private readonly IExceptionRepository _exceptionRepository;

        #endregion

        #region Constructor

        public ExceptionHandler(IExceptionRepository exceptionRepository)
        {
            if (exceptionRepository == null)
            {
                throw new ArgumentNullException(nameof(exceptionRepository));
            }

            _exceptionRepository = exceptionRepository;
        }

        #endregion

        #region Methods

        public Task HandleAsync(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            return Task.Run(() => AddToRepository(exception));
        }

        public Task HandleAsync(AggregateException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            return Task.Run(() => 
            {
                exception.Handle(ex => 
                {
                    AddToRepository(ex);
                    return true;
                });
            });
        }

        private void AddToRepository(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            try
            {
                Task task = _exceptionRepository.AddAsync(exception);
                task.Wait();
            }
            catch (AggregateException aggregateException)
            {
                aggregateException.Handle(ex => true);
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}