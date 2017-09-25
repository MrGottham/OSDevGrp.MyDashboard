using System;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    public abstract class InformationItemBase : DashboardItemBase, IInformationItem
    {
        #region Private variables

        private string _information;
        private string _details;

        #endregion
        
        #region Constructors

        protected InformationItemBase(string information) : this(information, DateTime.Now)
        {
        }

        protected InformationItemBase(string information, DateTime timestamp) : this(information, null, timestamp)
        {
        }

        protected InformationItemBase(string information, string details, DateTime timestamp) : this(Guid.NewGuid().ToString("D"), information, details, timestamp)
        {
        }

        protected InformationItemBase(string identifier, string information, string details, DateTime timestamp) : base(identifier, timestamp)
        {
            if (string.IsNullOrWhiteSpace(information))
            {
                throw new ArgumentNullException(nameof(information));
            }
            _information = information;
            _details = details;
        }

        #endregion

        #region Properties

        public virtual string Information
        {
            get
            {
                return _information;
            }
            protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _information = value;
            }
        }

        public virtual string Details
        {
            get
            {
                return _details;
            }
            protected set
            {
                _details = value;
            }
        }

        #endregion
    }
}