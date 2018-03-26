using System;
using System.Text;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class RedditAuthenticatedUserToObjectViewModelBuilder : ViewModelBuilderBase<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser>
    {
        #region Methods

        protected override ObjectViewModel<IRedditAuthenticatedUser> Build(IRedditAuthenticatedUser redditAuthenticatedUser)
        {
            int commentKarma = redditAuthenticatedUser.CommentKarma;
            int linkKarma = redditAuthenticatedUser.LinkKarma;
            bool hasUnreadMail = redditAuthenticatedUser.HasUnreadMail;

            StringBuilder htmlBuilder = new StringBuilder("<div class=\"card\">");
            htmlBuilder.Append("<img class=\"card-img-top img-fluid\" src=\"/images/reddit_logo_and_wordmark.png\" />");
            htmlBuilder.Append("<div class=\"card-body\">");
            htmlBuilder.Append($"<h4 class=\"card-title\">{redditAuthenticatedUser.UserName}</h4>");
            if (commentKarma > 0 || linkKarma > 0 || hasUnreadMail)
            {
                htmlBuilder.Append("<p class=\"card-text\">");
                if (commentKarma > 0)
                {
                    htmlBuilder.Append($"Comment karma: {commentKarma}");
                }
                if (commentKarma > 0 && linkKarma > 0)
                {
                    htmlBuilder.Append("<br>");
                }
                if (linkKarma > 0)
                {
                    htmlBuilder.Append($"Link karma: {linkKarma}");
                }
                if ((commentKarma > 0 || linkKarma > 0) && hasUnreadMail)
                {
                    htmlBuilder.Append("<br><br>");
                }
                if (hasUnreadMail)
                {
                    htmlBuilder.Append("You have unread mail");
                }
                htmlBuilder.Append("</p>");
            }
            htmlBuilder.Append("</div>");
            htmlBuilder.Append("</div>");

            return new ObjectViewModel<IRedditAuthenticatedUser>
            {
                ObjectIdentifier = redditAuthenticatedUser.Identifier,
                Object = redditAuthenticatedUser,
                Timestamp = redditAuthenticatedUser.CreatedTime,
                Html = htmlBuilder.ToString()
            };
        }

        #endregion
    }
}