using System;

namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IRedditSubreddit : IRedditCreatedThing
    {
        string DisplayName { get; }

        string DisplayNamePrefixed { get; }

        string Title { get; }

        string HeaderTitle { get; }

        string DescriptionAsText { get; }

        string DescriptionAsHtml { get; }

        string PublicDescriptionAsText { get; }

        string PublicDescriptionAsHtml { get; }

        Uri BannerImageUrl { get; }

        Uri HeaderImageUrl { get; }

        Uri IconImageUrl { get; }

        bool UserIsBanned { get; }

        bool Over18 { get; }

        long Subscribers { get; }

        Uri Url { get; }
    }
}