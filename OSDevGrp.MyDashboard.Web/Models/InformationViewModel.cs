using System;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Models
{
    public class InformationViewModel : IViewModel
    {
        #region Properties

        public string InformationIdentifier { get; set; }

        public DateTime Timestamp { get; set; }

        public string Header { get; set; }

        public string Summary { get; set; }

        public string Details { get; set; }

        public string ImageUrl { get; set; }

        public string Provider { get; set; }

        public string Author { get; set; }

        public string ExternalUrl { get; set; }

        #endregion
    }
}