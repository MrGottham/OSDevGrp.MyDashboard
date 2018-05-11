using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    public class DashboardRules : IDashboardRules
    {
        #region Private variables

        private readonly IRedditAuthenticatedUser _redditAuthenticatedUser;

        #endregion

        #region Constructor

        public DashboardRules(IRedditAuthenticatedUser redditAuthenticatedUser)
        {
            _redditAuthenticatedUser = redditAuthenticatedUser;
        }

        #endregion

        #region Properties

        public bool AllowNsfwContent 
        { 
            get
            {
                return _redditAuthenticatedUser != null && _redditAuthenticatedUser.Over18;
            }
        }

        #endregion
    }
}