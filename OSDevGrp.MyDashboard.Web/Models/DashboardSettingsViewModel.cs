using System.ComponentModel.DataAnnotations;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Models
{
    public class DashboardSettingsViewModel : IViewModel
    {
        [Range(0, 250)]
        [Display(Name="Number of news", ShortName="News", Description="Number of news to receive")]
        public int NumberOfNews { get; set; }
    }
}
