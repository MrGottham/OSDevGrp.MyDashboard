using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    public class DashboardSettings : IDashboardSettings
    {
        #region Constructor

        public DashboardSettings()
        {
            NumberOfNews = 50;
            UseReddit = false;
        }

        #endregion

        #region Properties

        public int NumberOfNews { get; set; }

        public bool UseReddit { get; set; }

        #endregion
    }
}