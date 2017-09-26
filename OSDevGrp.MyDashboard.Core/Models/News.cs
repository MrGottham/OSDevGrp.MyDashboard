using System;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    public class News : InformationItemBase, INews
    {
        #region Constructor

        public News(string inforamtion, string details, DateTime timestamp, INewsProvider provider) : base(Guid.NewGuid().ToString("D"), inforamtion, details, timestamp)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            Provider = provider;
        }

        #endregion

        #region Properties

        public INewsProvider Provider
        {
            get;
            private set;
        }

        #endregion
    }
}
