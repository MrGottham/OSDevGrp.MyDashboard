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

            StringBuilder htmlBuilder = new StringBuilder("<div class=\"card h-100\">");
            htmlBuilder.Append("<img class=\"card-img-top img-fluid\" src=\"/images/reddit_logo_and_wordmark.png\" />");
            htmlBuilder.Append("<div class=\"card-body\">");
            htmlBuilder.Append($"<h5 class=\"card-title\">{redditAuthenticatedUser.UserName}</h5>");
            if (commentKarma > 0)
            {
                htmlBuilder.Append($"<p class=\"card-text\">Comment karma: {commentKarma}</p>");
            }
            if (linkKarma > 0)
            {
                htmlBuilder.Append($"<p class=\"card-text\">Link karma: {linkKarma}</p>");
            }
            if (hasUnreadMail)
            {
                htmlBuilder.Append("<p class=\"card-text\">You have unread mail</p>");
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