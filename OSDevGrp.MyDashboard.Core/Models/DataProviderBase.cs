using System;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Models
{
    public class DataProviderBase : IDataProvider
    {
        #region Private variables
        
        private string _name;
        private Uri _uri;
        
        #endregion
        
        #region Constructor
        
        protected DataProviderBase(string name, Uri uri)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }
            _name = name;
            _uri = uri;
        }

        #endregion
        
        #region Properties

        public virtual string Name
        {
            get
            {
                return _name;
            }
            protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _name = value;
            }
        }

        public virtual Uri Uri
        {
            get
            {
                return _uri;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _uri = value;
            }
        }

        #endregion
    }
}