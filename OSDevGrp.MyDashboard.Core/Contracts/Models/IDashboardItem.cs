using System;

namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IDashboardItem
    {
        string Identifier { get; }

        DateTime Timestamp { get; }
    }
}