using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;

namespace OSDevGrp.MyDashboard.Core.Infrastructure
{
    public class ExceptionHandler : IExceptionHandler
    {
        #region Private variables

        private readonly IExceptionRepository _exceptionRepository;
        private readonly ILoggerFactory _loggerFactory;

        #endregion

        #region Constructor

        public ExceptionHandler(IExceptionRepository exceptionRepository, ILoggerFactory loggerFactory)
        {
            if (exceptionRepository == null)
            {
                throw new ArgumentNullException(nameof(exceptionRepository));
            }
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _exceptionRepository = exceptionRepository;
            _loggerFactory = loggerFactory;
        }

        #endregion

        #region Methods

        public Task HandleAsync(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            StackFrame stackFrame = new StackTrace().GetFrame(1);

            return Task.Run(() => 
            {
                AddToRepository(exception);
                LogException(stackFrame, exception);
            });
        }

        public Task HandleAsync(AggregateException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            StackFrame stackFrame = new StackTrace().GetFrame(1);

            return Task.Run(() => 
            {
                exception.Handle(ex => 
                {
                    AddToRepository(ex);
                    LogException(stackFrame, ex);
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
                _exceptionRepository.AddAsync(exception).GetAwaiter().GetResult();
            }
            catch (AggregateException aggregateException)
            {
                aggregateException.Handle(ex => true);
            }
            catch (Exception ex)
            {
                LogException(new StackTrace().GetFrame(0), ex);
            }
        }

        private void LogException(StackFrame stackFrame, Exception exception)
        {
            if (stackFrame == null)
            {
                throw new ArgumentNullException(nameof(stackFrame));
            }
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            MethodBase methodBase = stackFrame.GetMethod();

            ILogger logger = _loggerFactory.CreateLogger(methodBase.DeclaringType?.Namespace ?? GetType().Namespace);
            logger.LogError(exception, $"{methodBase.Name}: {exception.Message}");
        }

        #endregion
    }
}