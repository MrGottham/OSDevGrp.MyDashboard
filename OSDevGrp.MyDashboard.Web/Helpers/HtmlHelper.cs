using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

            Regex regex = new Regex($"({Environment.NewLine}|\n)");
            return regex.Replace(value, "<br />");
        }
        
        public string ExtractImages(string value, out IList<Uri> imageUrlCollection)
        {
            imageUrlCollection = new List<Uri>();
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            Regex extractImageRegex = new Regex(@"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            MatchCollection matchCollection = extractImageRegex.Matches(value);
            foreach (Match match in matchCollection)
            {
                imageUrlCollection.Add(new Uri(match.Groups[1].Value));
            }
            return extractImageRegex.Replace(value, string.Empty);
        }

        #endregion
    }
}