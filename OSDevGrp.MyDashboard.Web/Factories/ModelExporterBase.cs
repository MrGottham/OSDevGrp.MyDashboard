using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public abstract class ModelExporterBase<TOutput, TInput> : IModelExporter<TOutput, TInput> where TOutput : class, IExportModel where TInput : class 
    {
        #region Private variables

        private readonly IExceptionHandler _exceptionHandler;

        #endregion

        #region Constructor

        public ModelExporterBase(IExceptionHandler exceptionHandler)
        {
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(exceptionHandler));
            }

            _exceptionHandler = exceptionHandler;
        }

        #endregion

        #region Properties

        protected IExceptionHandler ExceptionHandler => _exceptionHandler;

        #endregion

        #region Methods

        public async Task<TOutput> ExportAsync(TInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            try
            {
                return await DoExportAsync(input); 
            }
            catch (AggregateException ex)
            {
                await ExceptionHandler.HandleAsync(ex);
                return Empty();
            }
            catch (Exception ex)
            {
                await ExceptionHandler.HandleAsync(ex);
                return Empty();
            }
        }

        protected abstract Task<TOutput> DoExportAsync(TInput input);

        protected virtual TOutput Empty()
        {
            return default(TOutput);
        }

        protected string GetValue<T>(T obj, Func<T, string> valueGetter) where T : class
        {
            if (valueGetter == null)
            {
                throw new ArgumentNullException(nameof(valueGetter));
            }

            if (obj == null)
            {
                return null;
            }

            return GetValue(valueGetter(obj));
        }

        protected string GetValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        #endregion
    }
}