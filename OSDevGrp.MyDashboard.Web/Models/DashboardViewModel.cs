using System;
using System.Collections.Generic;
using System.Linq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Models
{
    public class DashboardViewModel : IViewModel
    {
        #region Properties

        public IEnumerable<InformationViewModel> Informations { get; set; }

        public IEnumerable<InformationViewModel> InformationsWithImageUrl
        {
            get
            {
                if (Informations == null)
                {
                    return null;
                }
                return Informations
                    .Where(information => string.IsNullOrWhiteSpace(information.ImageUrl) == false)
                    .OrderByDescending(information => information.Timestamp);
            }
        }

        public IEnumerable<SystemErrorViewModel> SystemErrors { get; set; }

        public DashboardSettingsViewModel Settings { get; set; }

        public ObjectViewModel<IRedditAuthenticatedUser> RedditAuthenticatedUser { get; set; }

        public IEnumerable<ObjectViewModel<IRedditSubreddit>> RedditSubreddits { get; set; }

        #endregion

        #region Methods

        public void ApplyRules(IDashboardRules rules)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            if (Settings == null)
            {
                return;
            }
            Settings.ApplyRules(rules);
        }

        #endregion
    }
}