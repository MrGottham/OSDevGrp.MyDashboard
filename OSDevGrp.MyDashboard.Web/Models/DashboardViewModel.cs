using System.Collections.Generic;
using System.Linq;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Models
{
    public class DashboardViewModel : IViewModel
    {
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
    }
}