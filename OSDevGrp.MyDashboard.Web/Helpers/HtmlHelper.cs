using System;
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

        #endregion
    }
}