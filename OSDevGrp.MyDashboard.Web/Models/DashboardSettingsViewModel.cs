using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Core.Utilities;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Models
{
    [DataContract(Name = "DashboardSettings", Namespace = "urn:osdevgrp:mydashboard:1.0.0")]
    public class DashboardSettingsViewModel : IViewModel
    {
        #region Properties

        [Range(0, 250)]
        [Display(Name="Number of news", ShortName="News", Description="Number of news to receive")]
        [DataMember(Name = "NumberOfNews", IsRequired = true)]
        public int NumberOfNews { get; set; }
        
        [Display(Name="Use Reddit", ShortName="Reddit", Description="Collect data from Reddit")]
        [DataMember(Name = "UseReddit", IsRequired = true)]
        public bool UseReddit { get; set; }

        [Display(Name="Reddit Access Token", ShortName="Access Token", Description="Access Token received from Reddit")]
        [DataMember(Name = "RedditAccessToken", IsRequired = false)]
        public string RedditAccessToken { get; set; }

        #endregion

        #region Methods

        public IDashboardSettings ToDashboardSettings()
        {
            return new DashboardSettings()
            {
                NumberOfNews = NumberOfNews,
                UseReddit = UseReddit,
                RedditAccessToken = string.IsNullOrWhiteSpace(RedditAccessToken) ? null : OSDevGrp.MyDashboard.Core.Models.RedditAccessToken.Create(RedditAccessToken)
            };
        }

        public string ToBase64()
        {
            return JsonSerialization.ToBase64(this);
        }

        public static DashboardSettingsViewModel Create(string base64)
        {
            return JsonSerialization.FromBase64<DashboardSettingsViewModel>(base64);
        }

        #endregion
    }
}
