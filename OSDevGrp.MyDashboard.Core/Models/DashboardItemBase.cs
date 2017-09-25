using System;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    public abstract class DashboardItemBase : IDashboardItem
    {
        #region Private variables

        private string _identifier;
        private DateTime _timestamp;

        #endregion

        #region Constructor

        protected DashboardItemBase(string identifier, DateTime timestamp)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }
            _identifier = identifier;
            _timestamp = timestamp;
        }

        #endregion

        #region Properties

        public virtual string Identifier 
        {
            get
            {
                return _identifier;
            }
            protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _identifier = value;
            }
        }

        public virtual DateTime Timestamp
        {
            get
            {
                return _timestamp;
            }
            protected set
            {
                _timestamp = value;
            }
        }

        #endregion
    }
}