using System;
using System.Text;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class RedditSubredditToObjectViewModelBuilder : ViewModelBuilderBase<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit>
    {
        #region Methods

        protected override ObjectViewModel<IRedditSubreddit> Build(IRedditSubreddit redditSubreddit)
        {
            string displayNamePrefixed = redditSubreddit.DisplayNamePrefixed;
            string publicDescription = redditSubreddit.PublicDescriptionAsText;
            Uri url = redditSubreddit.Url;
            Uri imageUrl = redditSubreddit.BannerImageUrl ?? redditSubreddit.HeaderImageUrl;

            StringBuilder htmlBuilder = new StringBuilder("<div class=\"card mb-md-3\">");
            if (imageUrl != null)
            {
                htmlBuilder.Append($"<img class=\"card-img-top img-fluid\" src=\"{imageUrl.AbsoluteUri}\" />");
            }
            htmlBuilder.Append("<div class=\"card-body\">");
            htmlBuilder.Append("<h5 class=\"card-title\">");
            if (url != null)
            {
                htmlBuilder.Append($"<a href=\"#\" onclick=\"javascript:openLink('{url.AbsoluteUri}');\">{displayNamePrefixed}</a>");
            }
            else
            {
                htmlBuilder.Append(displayNamePrefixed);
            }
            htmlBuilder.Append("</h5>");
            htmlBuilder.Append("<p class=\"card-text\">");
            if (string.IsNullOrWhiteSpace(publicDescription) == false)
            {
                htmlBuilder.Append($"{publicDescription}<br><br>");
            }
            htmlBuilder.Append($"Subscribers: {redditSubreddit.Subscribers:N0}");
            htmlBuilder.Append("</p>");
            htmlBuilder.Append("</div>");
            htmlBuilder.Append("</div>");

            return new ObjectViewModel<IRedditSubreddit>
            {
                ObjectIdentifier = redditSubreddit.Identifier,
                Object = redditSubreddit,
                Timestamp = redditSubreddit.CreatedTime,
                Html = htmlBuilder.ToString()
            };
        }

        #endregion
    }
}