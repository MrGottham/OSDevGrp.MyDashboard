using System;
using System.Collections.Generic;

namespace OSDevGrp.MyDashboard.Web.Contracts.Helpers
{
    public interface IHtmlHelper
    {
        string ConvertNewLines(string value);

        string RemoveEndingComment(string value);

        string Convert(string value, bool convertNewLines = true, bool removeEndingComment = true);

        string ExtractImages(string value, out IList<Uri> imageUrlCollection);
    }
}