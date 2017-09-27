using System;

namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface INews : IInformationItem
    {
        INewsProvider Provider { get; }

        Uri Link { get; set; }
    }
}