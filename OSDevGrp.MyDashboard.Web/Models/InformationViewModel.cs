using OSDevGrp.MyDashboard.Web.Contracts.Models;
using System;

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

        public string AuthorOrProvider => string.IsNullOrWhiteSpace(Author) == false ? Author : Provider;

        public string ExternalUrl { get; set; }

        public int AgeInDays
        {
            get
            {
                TimeSpan span = DateTime.Now - Timestamp;
                return Convert.ToInt32(Math.Floor(span.TotalDays));
            }
        }

        #endregion
    }
}