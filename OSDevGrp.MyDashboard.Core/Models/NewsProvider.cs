using System;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    public class NewsProvider : DataProviderBase, INewsProvider
    {
        #region Constructor

        public NewsProvider(string name, Uri uri) : base(name, uri)
        {
        }

        #endregion
    }
}
