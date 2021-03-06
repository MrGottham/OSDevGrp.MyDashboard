using System;
using System.Runtime.Serialization;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Models
{
    [Serializable]
    public class SystemErrorViewModel : IViewModel
    {
        #region Constructors

        public SystemErrorViewModel()
        {
        }

        protected SystemErrorViewModel(SerializationInfo info, StreamingContext context)
        {
        }

        #endregion

        #region Properties

        public string SystemErrorIdentifier { get; set; }

        public DateTime Timestamp { get; set; }

        public string Message { get; set; }

        public string TruncatedMessage
        {
            get
            {
                string message = Message == null ? null : Message.Trim();
                if (message == null || message.Length < 100)
                {
                    return message;
                }
                return $"{message.Substring(0, 98).Trim()}..";
            }
        }

        public string Details { get; set; }

        #endregion
    }
}