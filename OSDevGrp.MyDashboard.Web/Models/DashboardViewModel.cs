using System.Collections.Generic;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Models
{
    public class DashboardViewModel : IViewModel
    {
        public IEnumerable<InformationViewModel> Informations { get; set; }

        public IEnumerable<SystemErrorViewModel> SystemErrors { get; set; }
    }
}