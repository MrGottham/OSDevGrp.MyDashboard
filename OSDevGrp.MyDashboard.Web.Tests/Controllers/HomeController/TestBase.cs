using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Core.Utilities;
using OSDevGrp.MyDashboard.Web.Models;
using System;
using System.Collections.Generic;

namespace OSDevGrp.MyDashboard.Web.Tests.Controllers.HomeController
{
    public abstract class TestBase
    {
        protected static DashboardSettingsViewModel BuildDashboardSettingsViewModel(Random random, int? numberOfNews = null, bool? useReddit = null, bool? allowNsfwContent = null, bool? includeNsfwContent = null, bool? onlyNsfwContent = null, string redditAccessToken = null, bool? exportData = null)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            return new DashboardSettingsViewModel
            {
                NumberOfNews = numberOfNews ?? random.Next(25, 50),
                UseReddit = useReddit ?? random.Next(100) > 50,
                AllowNsfwContent = allowNsfwContent ?? random.Next(100) > 50,
                IncludeNsfwContent = includeNsfwContent,
                OnlyNsfwContent = onlyNsfwContent,
                RedditAccessToken = redditAccessToken,
                ExportData = exportData ?? random.Next(100) > 50
            };
        }

        protected static IRedditAccessToken BuildRedditAccessToken(Random random, string accessToken = null, string tokenType = null, int? expiresIn = null, string scope = null, string refreshToken = null, DateTime? received = null)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            return new MyRedditAccessToken(
                accessToken ?? Guid.NewGuid().ToString("D"),
                tokenType ?? Guid.NewGuid().ToString("D"),
                expiresIn ?? random.Next(30, 60) * 60,
                scope ?? Guid.NewGuid().ToString("D"),
                refreshToken ?? Guid.NewGuid().ToString("D"),
                received ?? DateTime.UtcNow            );
        }

        protected static string BuildRedditAccessTokenAsBase64(Random random, string accessToken = null, string tokenType = null, int? expiresIn = null, string scope = null, string refreshToken = null, DateTime? received = null)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            IRedditAccessToken redditAccessToken = BuildRedditAccessToken(random, accessToken, tokenType, expiresIn, scope, refreshToken, received);
            return Convert.ToBase64String(JsonSerialization.ToByteArray(redditAccessToken));
        }

        protected static IRedditAuthenticatedUser BuildRedditAuthenticatedUser(Random random, bool? over18 = null)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            return BuildRedditAuthenticatedUserMock(random, over18).Object;
        }

        protected static Mock<IRedditAuthenticatedUser> BuildRedditAuthenticatedUserMock(Random random, bool? over18 = null)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = new Mock<IRedditAuthenticatedUser>();
            redditAuthenticatedUserMock.Setup(m => m.Over18)
                .Returns(over18 ?? random.Next(100) > 50);
            return redditAuthenticatedUserMock;
        }

        protected static IDashboard BuildDashboard()
        {
            return BuildDashboardMock().Object;
        }

        protected static Mock<IDashboard> BuildDashboardMock()
        {
            return new Mock<IDashboard>();
        }

        protected static DashboardViewModel BuildDashboardViewModel(Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            InformationViewModel informationViewModel = new InformationViewModel
            {
                InformationIdentifier = Guid.NewGuid().ToString("D"),
                Timestamp = DateTime.Now,
                Header = Guid.NewGuid().ToString("D"),
                Summary = Guid.NewGuid().ToString("D"),
                Details = Guid.NewGuid().ToString("D"),
                ImageUrl = Guid.NewGuid().ToString("D"),
                Provider = Guid.NewGuid().ToString("D"),
                Author = Guid.NewGuid().ToString("D"),
                ExternalUrl = Guid.NewGuid().ToString("D")
            };
            ImageViewModel<InformationViewModel> imageViewModel = new ImageViewModel<InformationViewModel>(informationViewModel, Convert.FromBase64String("R0lGODlhDgAOAKIAAAAAAP///wAAgP//AP8AAMDAwICAgP///yH5BAEAAAcALAAAAAAOAA4AAAM+aLq8YCPIOGV5YdV5IRFgCAbdFlFDNygkWRQGiXHZkj5rXKDWHBUu2A3C2jFkMRolMDyReMiXdCoFWK/YbAIAOw=="));

            SystemErrorViewModel systemErrorViewModel = new SystemErrorViewModel
            {
                SystemErrorIdentifier = Guid.NewGuid().ToString("D"),
                Timestamp = DateTime.Now,
                Message = Guid.NewGuid().ToString("D"),
                Details = Guid.NewGuid().ToString("D"),
            };

            ObjectViewModel<IRedditAuthenticatedUser> redditAuthenticatedUserObjectViewModel = new ObjectViewModel<IRedditAuthenticatedUser>
            {
                ObjectIdentifier = Guid.NewGuid().ToString("D"),
                Object = new RedditAuthenticatedUser(),
                Timestamp = DateTime.Now,
                Html = Guid.NewGuid().ToString("D")
            };

            ObjectViewModel<IRedditSubreddit> redditSubredditObjectViewModel = new ObjectViewModel<IRedditSubreddit>
            {
                ObjectIdentifier = Guid.NewGuid().ToString("D"),
                Object = new RedditSubreddit(),
                Timestamp = DateTime.Now,
                Html = Guid.NewGuid().ToString("D")
            };

            return new DashboardViewModel
            {
                Informations = new List<InformationViewModel> {informationViewModel},
                LatestInformationsWithImage = new List<ImageViewModel<InformationViewModel>> {imageViewModel},
                SystemErrors = new List<SystemErrorViewModel> {systemErrorViewModel},
                Settings = BuildDashboardSettingsViewModel(random),
                RedditAuthenticatedUser = redditAuthenticatedUserObjectViewModel,
                RedditSubreddits = new List<ObjectViewModel<IRedditSubreddit>> {redditSubredditObjectViewModel}
            };
        }

        protected static DashboardExportModel BuildDashboardExportModel()
        {
            return new DashboardExportModel(new DashboardItemExportModel[0]);
        }
    }
}