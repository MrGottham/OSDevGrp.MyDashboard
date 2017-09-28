using System;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    public class SystemError : InformationItemBase, ISystemError
    {
        #region Constructor

        public SystemError(Exception exception) : base(exception.Message, exception.StackTrace, DateTime.Now)
        {
        }

        #endregion
    }
}
