using OSDevGrp.MyDashboard.Web.Contracts.Models;
using System;

namespace OSDevGrp.MyDashboard.Web.Models
{
    public class ObjectViewModel<TObject> : IViewModel where TObject : class
    {
        #region Properties

        public string ObjectIdentifier { get; set; }

        public TObject Object { get; set; }

        public DateTime Timestamp { get; set; }

        public string Html { get; set; }

        #endregion
    }
}