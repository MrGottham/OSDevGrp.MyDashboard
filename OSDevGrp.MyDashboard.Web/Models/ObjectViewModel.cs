using System;
using System.Runtime.Serialization;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Models
{
    [Serializable]
    public class ObjectViewModel<TObject> : IViewModel where TObject : class
    {
        #region Constructors

        public ObjectViewModel()
        {
        }

        protected ObjectViewModel(SerializationInfo info, StreamingContext context)
        {
        }

        #endregion

        #region Properties

        public string ObjectIdentifier { get; set; }

        public TObject Object { get; set; }

        public DateTime Timestamp { get; set; }

        public string Html { get; set; }

        #endregion
    }
}