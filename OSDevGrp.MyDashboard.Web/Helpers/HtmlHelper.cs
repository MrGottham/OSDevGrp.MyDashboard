using System;
using System.Collections.Generic;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;

namespace OSDevGrp.MyDashboard.Web.Helpers
{
    public class HtmlHelper : IHtmlHelper
    {
        #region Methods

        public string ConvertNewLines(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            return value.Replace(Environment.NewLine, "<br />");
        }
        
        public string ExtractImages(string value, out IList<Uri> imageUrlCollection)
        {
            imageUrlCollection = new List<Uri>();
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            
            throw new NotImplementedException();
        }

        #endregion
    }
}