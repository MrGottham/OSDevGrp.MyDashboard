using System;
using System.Collections.Generic;

namespace OSDevGrp.MyDashboard.Web.Contracts.Helpers
{
    public interface IHtmlHelper
    {
        string ConvertNewLines(string value);

        string ExtractImages(string value, out IList<Uri> imageUrlCollection);
    }
}