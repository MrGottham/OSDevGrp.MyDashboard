using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Models.Dashboard
{
    [TestClass]
    public class ReplaceTests
    {
        [TestMethod]
        [ExpectedArgumentNullException("news")]
        public void Replace_WhenCalledWithNewsEqualToNull_ThrowsArgumentNullExcpetion()
        {
            const IEnumerable<INews> news = null;

            IDashboard sut = CreateSut();

            sut.Replace(news);
        }
        
        [TestMethod]
        public void Replace_WhenCalledWithNewsNotEqualToNull_ExpectedReplacedCollection()
        {
            IEnumerable<INews> news = new List<INews>
            {
                new Mock<INews>().Object,
                new Mock<INews>().Object,
                new Mock<INews>().Object
            };

            IDashboard sut = CreateSut();

            sut.Replace(news);

            Assert.IsNotNull(sut.News);
            Assert.AreEqual(news.Count(), sut.News.Count());
            Assert.IsTrue(sut.News.All(item => news.Contains(item)));
        }
        
        [TestMethod]
        [ExpectedArgumentNullException("redditAuthenticatedUser")]
        public void Replace_WhenCalledWithRedditAuthenticatedUserEqualToNull_ThrowsArgumentNullExcpetion()
        {
            const IRedditAuthenticatedUser redditAuthenticatedUser = null;

            IDashboard sut = CreateSut();

            sut.Replace(redditAuthenticatedUser);
        }
        
        [TestMethod]
        public void Replace_WhenCalledWithRedditAuthenticatedUserNotEqualToNull_ExpectedReplacedDashboardSettings()
        {
            IRedditAuthenticatedUser redditAuthenticatedUser = new Mock<IRedditAuthenticatedUser>().Object;

            IDashboard sut = CreateSut();

            sut.Replace(redditAuthenticatedUser);

            Assert.IsNotNull(sut.RedditAuthenticatedUser);
            Assert.AreEqual(redditAuthenticatedUser, sut.RedditAuthenticatedUser);
        }

        [TestMethod]
        [ExpectedArgumentNullException("redditSubreddits")]
        public void Replace_WhenCalledWithRedditSubredditsEqualToNull_ThrowsArgumentNullExcpetion()
        {
            const IEnumerable<IRedditSubreddit> redditSubreddits = null;

            IDashboard sut = CreateSut();

            sut.Replace(redditSubreddits);
        }

        [TestMethod]
        public void Replace_WhenCalledWithRedditSubredditsNotEqualToNull_ExpectedReplacedCollection()
        {
            IEnumerable<IRedditSubreddit> redditSubreddits = new List<IRedditSubreddit>
            {
                new Mock<IRedditSubreddit>().Object,
                new Mock<IRedditSubreddit>().Object,
                new Mock<IRedditSubreddit>().Object
            };

            IDashboard sut = CreateSut();

            sut.Replace(redditSubreddits);

            Assert.IsNotNull(sut.RedditSubreddits);
            Assert.AreEqual(redditSubreddits.Count(), sut.RedditSubreddits.Count());
            Assert.IsTrue(sut.RedditSubreddits.All(redditSubreddit => redditSubreddits.Contains(redditSubreddit)));
        }

        [TestMethod]
        [ExpectedArgumentNullException("redditLinks")]
        public void Replace_WhenCalledWithRedditLinksEqualToNull_ThrowsArgumentNullExcpetion()
        {
            const IEnumerable<IRedditLink> redditLinks = null;

            IDashboard sut = CreateSut();

            sut.Replace(redditLinks);
        }

        [TestMethod]
        public void Replace_WhenCalledWithRedditLinksNotEqualToNull_ExpectedReplacedCollection()
        {
            IEnumerable<IRedditLink> redditLinks = new List<IRedditLink>
            {
                new Mock<IRedditLink>().Object,
                new Mock<IRedditLink>().Object,
                new Mock<IRedditLink>().Object
            };

            IDashboard sut = CreateSut();

            sut.Replace(redditLinks);

            Assert.IsNotNull(sut.RedditLinks);
            Assert.AreEqual(redditLinks.Count(), sut.RedditLinks.Count());
            Assert.IsTrue(sut.RedditLinks.All(redditLink => redditLinks.Contains(redditLink)));
        }

        [TestMethod]
        [ExpectedArgumentNullException("systemErrors")]
        public void Replace_WhenCalledWithSystemErrorsEqualToNull_ThrowsArgumentNullExcpetion()
        {
            const IEnumerable<ISystemError> systemErrors = null;

            IDashboard sut = CreateSut();

            sut.Replace(systemErrors);
        }
        
        [TestMethod]
        public void Replace_WhenCalledWithSystemErrorsNotEqualToNull_ExpectedReplacedCollection()
        {
            IEnumerable<ISystemError> systemErrors = new List<ISystemError>
            {
                new Mock<ISystemError>().Object,
                new Mock<ISystemError>().Object,
                new Mock<ISystemError>().Object
            };

            IDashboard sut = CreateSut();

            sut.Replace(systemErrors);

            Assert.IsNotNull(sut.SystemErrors);
            Assert.AreEqual(systemErrors.Count(), sut.SystemErrors.Count());
            Assert.IsTrue(sut.SystemErrors.All(systemError => systemErrors.Contains(systemError)));
        }
        
        [TestMethod]
        [ExpectedArgumentNullException("settings")]
        public void Replace_WhenCalledWithDashboardSettingsEqualToNull_ThrowsArgumentNullExcpetion()
        {
            const IDashboardSettings dashboardSettings = null;

            IDashboard sut = CreateSut();

            sut.Replace(dashboardSettings);
        }
        
        [TestMethod]
        public void Replace_WhenCalledWithDashboardSettingsNotEqualToNull_ExpectedReplacedDashboardSettings()
        {
            IDashboardSettings dashboardSettings = new Mock<IDashboardSettings>().Object;

            IDashboard sut = CreateSut();

            sut.Replace(dashboardSettings);

            Assert.IsNotNull(sut.Settings);
            Assert.AreEqual(dashboardSettings, sut.Settings);
        }
        
        private IDashboard CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Models.Dashboard();
        }
   }
}