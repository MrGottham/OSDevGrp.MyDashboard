using System;

namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IDashboardItem : IIdentifiable
    {
        DateTime Timestamp { get; }
    }
}