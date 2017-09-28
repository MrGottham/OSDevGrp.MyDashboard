using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;

namespace OSDevGrp.MyDashboard.Core.Logic
{
    public class NewsLogic : INewsLogic
    {
        #region Private variables

        private readonly INewsRepository _newRepository;
        private readonly IExceptionHandler _exceptionHandler;

        #endregion

        #region Constructor

        public NewsLogic(INewsRepository newsRepository, IExceptionHandler exceptionHandler)
        {
            if (newsRepository == null) throw new ArgumentNullException(nameof(newsRepository));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));

            _newRepository = newsRepository;
            _exceptionHandler = exceptionHandler;
        }

        #endregion

        #region Methods

        public Task<IEnumerable<INews>> GetNewsAsync(int numberOfNews)
        {
            return Task.Run<IEnumerable<INews>>(async () => 
            {
                try
                {
                    IEnumerable<INews> news = await _newRepository.GetNewsAsync();
                    if (news == null)
                    {
                        return new List<INews>(0);
                    }
                    return news
                        .OrderByDescending(m => m.Timestamp)
                        .Take(numberOfNews)
                        .ToList();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                return new List<INews>(0);
            });
        }

        #endregion
    }
}