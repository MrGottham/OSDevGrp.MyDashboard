using System;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Models
{
    public class SystemErrorViewModel : IViewModel
    {
        #region Properties

        public string SystemErrorIdentifier { get; set; }

        public DateTime Timestamp { get; set; }

        public string Message { get; set; }

        public string Details { get; set; }

        #endregion
    }
}