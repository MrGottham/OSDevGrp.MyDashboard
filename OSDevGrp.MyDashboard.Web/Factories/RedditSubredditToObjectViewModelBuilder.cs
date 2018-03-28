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
            StringBuilder htmlBuilder = new StringBuilder("<div>");
            htmlBuilder.Append($"<h5>{redditSubreddit.DisplayNamePrefixed}</h5>");
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