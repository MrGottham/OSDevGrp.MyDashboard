using System.Collections.Generic;

namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IDashboard
    {
        IEnumerable<INews> News { get; }

        IEnumerable<ISystemError> SystemErrors { get; }

        void Replace(IEnumerable<INews> news);

        void Replace(IEnumerable<ISystemError> systemErrors);
    }
}