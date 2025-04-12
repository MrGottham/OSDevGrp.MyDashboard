using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSDevGrp.MyDashboard.Web.Models
{
    public class DashboardViewModel : IViewModel
    {
        #region Properties

        public List<InformationViewModel> Informations { get; set; }

        public List<ImageViewModel<InformationViewModel>> LatestInformationsWithImage { get; set; }

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

        public List<SystemErrorViewModel> SystemErrors { get; set; }

        public DashboardSettingsViewModel Settings { get; set; }

        public ObjectViewModel<IRedditAuthenticatedUser> RedditAuthenticatedUser { get; set; }

        public List<ObjectViewModel<IRedditSubreddit>> RedditSubreddits { get; set; }

        public static string CookieName
        {
            get
            {
                Type type = typeof(DashboardViewModel);
                return $"{type.Namespace}.{type.Name}";
            }
        }

        #endregion

        #region Methods

        public void ApplyRules(IDashboardRules rules, ICookieHelper cookieHelper)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }
            if (cookieHelper == null)
            {
                throw new ArgumentNullException(nameof(cookieHelper));
            }

            if (Settings == null)
            {
                return;
            }

            Settings.ApplyRules(rules, cookieHelper);
        }

        #endregion
    }
}