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
        private IRedditAuthenticatedUser _redditAuthenticatedUser;
        private IList<ISystemError> _systemErrors = new List<ISystemError>();
        private IDashboardSettings _settings;

        #endregion

        #region Properties

        public IEnumerable<INews> News 
        { 
            get
            {
                return _news;
            } 
        }

        public IRedditAuthenticatedUser RedditAuthenticatedUser
        {
            get
            {
                return _redditAuthenticatedUser;
            }
        }

        public IEnumerable<ISystemError> SystemErrors 
        { 
            get
            {
                return _systemErrors;
            }
        }

        public IDashboardSettings Settings
        {
            get
            {
                return _settings;
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

        public void Replace(IRedditAuthenticatedUser redditAuthenticatedUser)
        {
            if (redditAuthenticatedUser == null)
            {
                throw new ArgumentNullException(nameof(redditAuthenticatedUser));
            }

            _redditAuthenticatedUser = redditAuthenticatedUser;
        }

        public void Replace(IEnumerable<ISystemError> systemErrors)
        {
            if (systemErrors == null)
            {
                throw new ArgumentNullException(nameof(systemErrors));
            }

            _systemErrors = systemErrors.ToList();
        }

        public void Replace(IDashboardSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            
            _settings = settings;
        }

        #endregion
    }
}