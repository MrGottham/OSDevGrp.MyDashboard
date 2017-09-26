using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;

namespace OSDevGrp.MyDashboard.Core.Repositories
{
    public class ExceptionRepository : IExceptionRepository
    {
        #region Private variables

        private readonly List<Exception> _exceptions = new List<Exception>();
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
                    _exceptions.Add(exception);
                }
            });
        }

        #endregion
    }
}