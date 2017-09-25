using System;

namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IDataProvider
    {
        string Name { get; }

        Uri Uri { get; }
    }
}