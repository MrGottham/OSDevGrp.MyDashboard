using System.Collections.Generic;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Models
{
    public class DashboardViewModel : IViewModel
    {
        public IEnumerable<NewsViewModel> News { get; set; }

        public IEnumerable<SystemErrorViewModel> SystemErrors { get; set; }
    }
}