using OSDevGrp.MyDashboard.Web.Contracts.Models;
using System;

namespace OSDevGrp.MyDashboard.Web.Models
{
    public class SystemErrorViewModel : IViewModel
    {
        #region Properties

        public string SystemErrorIdentifier { get; set; }

        public DateTime Timestamp { get; set; }

        public string Message { get; set; }

        public string TruncatedMessage
        {
            get
            {
                string message = Message?.Trim();
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