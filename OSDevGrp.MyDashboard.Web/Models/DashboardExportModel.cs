using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Models
{
    public class DashboardExportModel : IExportModel
    {
        #region Constructors

        public DashboardExportModel()
        {
            Items = new List<DashboardItemExportModel>(0);
        }

        public DashboardExportModel(IEnumerable<DashboardItemExportModel> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            Items = new List<DashboardItemExportModel>(items);
        }

        #endregion

        #region Properties

        [Required]
        public IReadOnlyList<DashboardItemExportModel> Items { get; set; }

        #endregion
    }
}