using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Models
{
    [Serializable]
    [DataContract(Name = "DashboardSettings")]
    public class DashboardSettingsViewModel : IViewModel
    {
        #region Constructors

        public DashboardSettingsViewModel()
        {
        }

        protected DashboardSettingsViewModel(SerializationInfo info, StreamingContext context)
        {
        }

        #endregion

        #region Properties

        [Range(0, 250)]
        [Display(Name = "Number of news", ShortName = "News", Description = "Number of news to receive")]
        [DataMember(Name = "NumberOfNews", IsRequired = true)]
        public int NumberOfNews { get; set; }

        [Display(Name = "Use Reddit", ShortName = "Reddit", Description = "Collect data from Reddit")]
        [DataMember(Name = "UseReddit", IsRequired = true)]
        public bool UseReddit { get; set; }

        [Display(Name = "Allow NSFW content", ShortName = "Allow NSFW", Description = "Allow NSFW content from Reddit")]
        [DataMember(Name = "AllowNsfwContent", IsRequired = true)]
        public bool AllowNsfwContent { get; set; }

        [Display(Name = "Include NSFW content", ShortName = "Include NSFW", Description = "Include NSFW content from Reddit")]
        [DataMember(Name = "IncludeNsfwContent", IsRequired = false)]
        public bool? IncludeNsfwContent { get; set; }

        [Display(Name = "Include NSFW content", ShortName = "Include NSFW", Description = "Include NSFW content from Reddit")]
        [IgnoreDataMember]
        public bool NotNullableIncludeNsfwContent
        {
            get
            {
                return IncludeNsfwContent ?? false;
            }
            set
            {
                IncludeNsfwContent = value;
            }
        }

        [Display(Name = "Only NSFW content", ShortName = "Only NSFW", Description = "Show only NSFW content from Reddit")]
        [DataMember(Name = "OnlyNsfwContent", IsRequired = false)]
        public bool? OnlyNsfwContent { get; set; }

        [Display(Name = "Only NSFW content", ShortName = "Only NSFW", Description = "Show only NSFW content from Reddit")]
        [IgnoreDataMember]
        public bool NotNullableOnlyNsfwContent
        {
            get
            {
                return OnlyNsfwContent ?? false;
            }
            set
            {
                OnlyNsfwContent = value;
            }
        }

        [Display(Name = "Reddit Access Token", ShortName = "Access Token", Description = "Access Token received from Reddit")]
        [DataMember(Name = "RedditAccessToken", IsRequired = false)]
        public string RedditAccessToken { get; set; }

        [Display(Name = "Export data", ShortName = "Export", Description = "Indicates whether dashboard data should be exported")]
        [DataMember(Name = "ExportData", IsRequired = false)]
        public bool ExportData { get; set; }

        [Display(Name = "Cookie name", ShortName = "Cookie name", Description = "Cookie name for the dashboard settings view model")]
        [IgnoreDataMember]
        public static string CookieName
        {
            get
            {
                Type type = typeof(DashboardSettingsViewModel);
                return $"{type.Namespace}.{type.Name}";
            }
        }

        #endregion

        #region Methods

        public IDashboardSettings ToDashboardSettings()
        {
            return new DashboardSettings
            {
                NumberOfNews = NumberOfNews,
                UseReddit = UseReddit,
                RedditAccessToken = string.IsNullOrWhiteSpace(RedditAccessToken) ? null : OSDevGrp.MyDashboard.Core.Models.RedditAccessToken.Create(RedditAccessToken),
                IncludeNsfwContent = IncludeNsfwContent ?? false,
                OnlyNsfwContent = OnlyNsfwContent ?? false
            };
        }

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

            try
            {
                AllowNsfwContent = rules.AllowNsfwContent;
                if (AllowNsfwContent)
                {
                    return;
                }
                IncludeNsfwContent = null;
                OnlyNsfwContent = null;
            }
            finally
            {
                cookieHelper.ToCookie(this);
            }
        }

        #endregion
    }

    public static class DashboardSettingsViewModelExtensions
    {
        public static string GetBuildUrl(this DashboardSettingsViewModel dashboardSettingsViewModel, IContentHelper contentHelper)
        {
            if (dashboardSettingsViewModel == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettingsViewModel));
            }
            if (contentHelper == null)
            {
                throw new ArgumentNullException(nameof(contentHelper));
            }

            return contentHelper.AbsoluteUrl("Build", "Home", new {DashboardSettings = contentHelper.ToBase64String(dashboardSettingsViewModel)});
        }

        public static string GetTopContentUrl(this DashboardSettingsViewModel dashboardSettingsViewModel, IContentHelper contentHelper)
        {
            if (dashboardSettingsViewModel == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettingsViewModel));
            }
            if (contentHelper == null)
            {
                throw new ArgumentNullException(nameof(contentHelper));
            }

            return contentHelper.AbsoluteUrl("TopContent", "Home");
        }

        public static string GetSubContentUrl(this DashboardSettingsViewModel dashboardSettingsViewModel, IContentHelper contentHelper)
        {
            if (dashboardSettingsViewModel == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettingsViewModel));
            }
            if (contentHelper == null)
            {
                throw new ArgumentNullException(nameof(contentHelper));
            }

            return contentHelper.AbsoluteUrl("SubContent", "Home");
        }

        public static string GetSettingsUrl(this DashboardSettingsViewModel dashboardSettingsViewModel, IContentHelper contentHelper)
        {
            if (dashboardSettingsViewModel == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettingsViewModel));
            }
            if (contentHelper == null)
            {
                throw new ArgumentNullException(nameof(contentHelper));
            }

            return contentHelper.AbsoluteUrl("Settings", "Home");
        }
    }
}