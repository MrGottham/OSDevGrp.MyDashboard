using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;
using OSDevGrp.MyDashboard.Core.Models;

namespace OSDevGrp.MyDashboard.Core.Repositories
{
    public class ExceptionRepository : IExceptionRepository
    {
        #region Private variables

        private readonly IList<ISystemError> _systemErrors = new List<ISystemError>();
        private static readonly object SyncRoot = new object();

        #endregion

        #region Methods

        public Task AddAsync(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            return Task.Run(() => 
            {
                lock (SyncRoot)
                {
                    _systemErrors.Add(new SystemError(exception));
                }
            });
        }
        
        public Task<IEnumerable<ISystemError>> GetSystemErrorsAsync()
        {
            return Task.Run<IEnumerable<ISystemError>>(() =>
            {
                lock (SyncRoot)
                {
                    IEnumerable<ISystemError> systemErrors = new List<ISystemError>(_systemErrors.OrderByDescending(m => m.Timestamp));
                    _systemErrors.Clear();

                    return systemErrors;
                }
            });
        }

        #endregion
    }
}