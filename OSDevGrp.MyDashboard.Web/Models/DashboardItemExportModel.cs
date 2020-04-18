using System;
using System.ComponentModel.DataAnnotations;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Models
{
    public class DashboardItemExportModel : IExportModel
    {
        #region Constructors

        public DashboardItemExportModel()
        {
        }

        public DashboardItemExportModel(string identifier, DateTimeOffset timestamp, string information)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentNullException(identifier);
            }
            if (string.IsNullOrWhiteSpace(information))
            {
                throw new ArgumentNullException(information);
            }

            Identifier = identifier;
            Timestamp = timestamp;
            Information = information;
        }

        #endregion

        #region Properties

        [Required(AllowEmptyStrings = false)]
        public string Identifier { get; set; }

        [Required]
        public DateTimeOffset Timestamp { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Information { get; set; }

        public string Details { get; set; }

        public string Provider { get; set; }

        public string Author { get; set; }

        public string SourceUrl { get; set; }

        public string ImageUrl { get; set; }

        #endregion
    }
}