using System;
using System.Collections.Generic;
using System.Linq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    public class Dashboard : IDashboard
    {
        #region Private variables

        private IList<INews> _news = new List<INews>();
        private IList<ISystemError> _systemErrors = new List<ISystemError>();

        #endregion

        #region Properties

        public IEnumerable<INews> News 
        { 
            get
            {
                return _news;
            } 
        }

        public IEnumerable<ISystemError> SystemErrors 
        { 
            get
            {
                return _systemErrors;
            }
        }

        #endregion

        #region Methods

        public void Replace(IEnumerable<INews> news)
        {
            if (news == null)
            {
                throw new ArgumentNullException(nameof(news));
            }

            _news = news.ToList();
        }

        public void Replace(IEnumerable<ISystemError> systemErrors)
        {
            if (systemErrors == null)
            {
                throw new ArgumentNullException(nameof(systemErrors));
            }

            _systemErrors = systemErrors.ToList();
        }

        #endregion
    }
}