using System;
using System.ComponentModel.DataAnnotations;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Models
{
    public class DashboardSettingsViewModel : IViewModel
    {
        #region Properties

        [Range(0, 250)]
        [Display(Name="Number of news", ShortName="News", Description="Number of news to receive")]
        public int NumberOfNews { get; set; }

        #endregion

        #region Methods

        public IDashboardSettings ToDashboardSettings()
        {
            return new DashboardSettings()
            {
                NumberOfNews = NumberOfNews
            };
        }

        #endregion
    }
}
