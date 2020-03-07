using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;

namespace OSDevGrp.MyDashboard.Core.Logic
{
    public class SystemErrorLogic : ISystemErrorLogic
    {
        #region Private variables

        private readonly IExceptionRepository _exceptionRepository;
        private readonly IExceptionHandler _exceptionHandler;

        #endregion

        #region Constructor

        public SystemErrorLogic(IExceptionRepository exceptionRepository, IExceptionHandler exceptionHandler)
        {
            if (exceptionRepository == null) throw new ArgumentNullException(nameof(exceptionRepository));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));

            _exceptionRepository = exceptionRepository;
            _exceptionHandler = exceptionHandler;
        }

        #endregion

        #region Methods

        public async Task<IEnumerable<ISystemError>> GetSystemErrorsAsync()
        {
            try
            {
                IEnumerable<ISystemError> systemErrors = await _exceptionRepository.GetSystemErrorsAsync();
                if (systemErrors == null)
                {
                    return new List<ISystemError>(0);
                }
                return systemErrors
                    .OrderByDescending(systemError => systemError.Timestamp)
                    .ToList();
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            return new List<ISystemError>(0);
        }
        
        #endregion
    }
}