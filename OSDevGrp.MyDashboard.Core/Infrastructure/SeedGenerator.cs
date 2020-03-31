using System;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;

namespace OSDevGrp.MyDashboard.Core.Infrastructure
{
    public class SeedGenerator : ISeedGenerator
    {
        #region Methods

        public int Generate()
        {
            string value = $"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} {Guid.NewGuid()}";
            return value.GetHashCode();
        }

        #endregion
    }
}