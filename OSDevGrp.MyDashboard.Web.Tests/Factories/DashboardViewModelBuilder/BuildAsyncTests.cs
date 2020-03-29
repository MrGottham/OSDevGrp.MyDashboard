using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;
using OSDevGrp.MyDashboard.Web.Tests.Helpers;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.DashboardViewModelBuilder
{
    [TestClass]
    public class BuildAsyncTests
    {
        #region Private variables

        private Mock<IViewModelBuilder<InformationViewModel, INews>> _newsToInformationViewModelBuilderMock;
        private Mock<IViewModelBuilder<SystemErrorViewModel, ISystemError>> _systemErrorViewModelBuilderMock;
        private Mock<IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings>> _dashboardSettingsViewModelBuilderMock;
        private Mock<IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser>> _redditAuthenticatedUserToObjectViewModelBuilderMock;
        private Mock<IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit>> _redditSubredditToObjectViewModelBuilder;
        private Mock<IViewModelBuilder<InformationViewModel, IRedditLink>> _redditLinkToInformationViewModelBuilderMock;
        private Mock<IHtmlHelper> _htmlHelperMock;
        private Mock<IHttpHelper> _httpHelperMock;
        private Mock<ICookieHelper> _cookieHelperMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _newsToInformationViewModelBuilderMock = new Mock<IViewModelBuilder<InformationViewModel, INews>>();
            _systemErrorViewModelBuilderMock = new Mock<IViewModelBuilder<SystemErrorViewModel, ISystemError>>();
            _dashboardSettingsViewModelBuilderMock = new Mock<IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings>>();
            _redditAuthenticatedUserToObjectViewModelBuilderMock = new Mock<IViewModelBuilder<ObjectViewModel<IRedditAuthenticatedUser>, IRedditAuthenticatedUser>>();
            _redditSubredditToObjectViewModelBuilder = new Mock<IViewModelBuilder<ObjectViewModel<IRedditSubreddit>, IRedditSubreddit>>();
            _redditLinkToInformationViewModelBuilderMock = new Mock<IViewModelBuilder<InformationViewModel, IRedditLink>>();
            _htmlHelperMock = new Mock<IHtmlHelper>();
            _httpHelperMock = new Mock<IHttpHelper>();
            _cookieHelperMock = new Mock<ICookieHelper>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullExceptionAttribute("input")]
        public async Task BuildAsync_WhenDashboardIsNull_ThrowsArgumentNullException()
        {
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            await sut.BuildAsync(null);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertNewsWasCalledOnDashboardOnce()
        {
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            await sut.BuildAsync(dashboardMock.Object);

            dashboardMock.Verify(m => m.News, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertSystemErrorsWasCalledOnDashboardOnce()
        {
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            await sut.BuildAsync(dashboardMock.Object);

            dashboardMock.Verify(m => m.SystemErrors, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertSettingsWasCalledOnDashboardOnce()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            Mock<IDashboard> dashboardMock = CreateDashboardMock(dashboardSettings: dashboardSettings);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            await sut.BuildAsync(dashboardMock.Object);

            dashboardMock.Verify(m => m.Settings, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertRedditAuthenticatedUserWasCalledOnDashboardOnce()
        {
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();
            Mock<IDashboard> dashboardMock = CreateDashboardMock(redditAuthenticatedUser: redditAuthenticatedUser);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            await sut.BuildAsync(dashboardMock.Object);

            dashboardMock.Verify(m => m.RedditAuthenticatedUser, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertRedditSubredditsWasCalledOnDashboardOnce()
        {
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            await sut.BuildAsync(dashboardMock.Object);

            dashboardMock.Verify(m => m.RedditSubreddits, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertRedditLinksWasCalledOnDashboardOnce()
        {
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            await sut.BuildAsync(dashboardMock.Object);

            dashboardMock.Verify(m => m.RedditLinks, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertBuildAsyncWasCalledOnNewsToInformationViewModelBuilderForEachNewsInDashboard()
        {
            List<INews> newsCollection = CreateNewsCollection(_random.Next(50, 75)).ToList();
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            await sut.BuildAsync(dashboard);

            newsCollection.ForEach(news => _newsToInformationViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<INews>(value => value == news)), Times.Once));
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertBuildAsyncWasCalledOnSystemErrorViewModelBuilderForEachSystemErrorInDashboard()
        {
            List<ISystemError> systemErrorCollection = CreateSystemErrorCollection(_random.Next(50, 75)).ToList();
            IDashboard dashboard = CreateDashboard(systemErrorCollection: systemErrorCollection);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            await sut.BuildAsync(dashboard);

            systemErrorCollection.ForEach(systemError => _systemErrorViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<ISystemError>(value => value == systemError)), Times.Once));
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertBuildAsyncWasCalledOnDashboardSettingsViewModelBuilderWithSettingsInDashboard()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IDashboard dashboard = CreateDashboard(dashboardSettings: dashboardSettings);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            await sut.BuildAsync(dashboard);

            _dashboardSettingsViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value == dashboardSettings)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertBuildAsyncWasCalledOnRedditAuthenticatedUserToObjectViewModelBuilderWithRedditAuthenticatedUserInDashboard()
        {
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();
            IDashboard dashboard = CreateDashboard(redditAuthenticatedUser: redditAuthenticatedUser);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            await sut.BuildAsync(dashboard);

            _redditAuthenticatedUserToObjectViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<IRedditAuthenticatedUser>(value => value == redditAuthenticatedUser)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertBuildAsyncWasCalledOnRedditSubredditToObjectViewModelBuilderForEachRedditSubredditInDashboard()
        {
            List<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(_random.Next(50, 75)).ToList();
            IDashboard dashboard = CreateDashboard(redditSubredditCollection: redditSubredditCollection);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            await sut.BuildAsync(dashboard);

            redditSubredditCollection.ForEach(redditSubreddit => _redditSubredditToObjectViewModelBuilder.Verify(m => m.BuildAsync(It.Is<IRedditSubreddit>(value => value == redditSubreddit)), Times.Once));
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertBuildAsyncWasCalledOnRedditLinkToInformationViewModelBuilderForEachRedditLinkInDashboard()
        {
            List<IRedditLink> redditLinkCollection = CreateRedditLinkCollection(_random.Next(50, 75)).ToList();
            IDashboard dashboard = CreateDashboard(redditLinkCollection: redditLinkCollection);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            await sut.BuildAsync(dashboard);

            redditLinkCollection.ForEach(redditLink => _redditLinkToInformationViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<IRedditLink>(value => value == redditLink)), Times.Once));
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertReadAsyncWasCalledOnHttpHelperForEachLatestInformationWithImageInDashboard()
        {
            List<INews> newsCollection = CreateNewsCollection(_random.Next(50, 75)).ToList();
            List<IRedditLink> redditLinkCollection = CreateRedditLinkCollection(_random.Next(50, 75)).ToList();
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, redditLinkCollection: redditLinkCollection);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            DashboardViewModel dashboardViewModel = await sut.BuildAsync(dashboard);

            int numberOfInformationWithImage = Math.Min(dashboardViewModel.Informations.Count(information => string.IsNullOrWhiteSpace(information.ImageUrl) == false), 7);
            if (numberOfInformationWithImage > 0)
            {
                _httpHelperMock.Verify(m => m.ReadAsync(It.IsAny<Uri>()), Times.Exactly(numberOfInformationWithImage));
            }
            else
            {
                _httpHelperMock.Verify(m => m.ReadAsync(It.IsAny<Uri>()), Times.Never);
            }

            foreach (ImageViewModel<InformationViewModel> latestInformationWithImage in dashboardViewModel.LatestInformationsWithImage)
            {
                _httpHelperMock.Verify(m => m.ReadAsync(It.Is<Uri>(value => string.Compare(latestInformationWithImage.ViewModel.ImageUrl, value.AbsoluteUri, false) == 0)), Times.Once);
            }
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertRulesWasCalledOnDashboard()
        {
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            await sut.BuildAsync(dashboardMock.Object);

            dashboardMock.Verify(m => m.Rules, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertToCookieWasCalledOnCookieHelperWithDashboardViewModel()
        {
            List<INews> newsCollection = CreateNewsCollection(_random.Next(50, 75)).ToList();
            List<ISystemError> systemErrorCollection = CreateSystemErrorCollection(_random.Next(10, 15)).ToList();
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();
            List<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(_random.Next(10, 25)).ToList();
            List<IRedditLink> redditLinkCollection = CreateRedditLinkCollection(_random.Next(50, 75)).ToList();
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection, dashboardSettings: dashboardSettings, redditAuthenticatedUser: redditAuthenticatedUser, redditSubredditCollection: redditSubredditCollection, redditLinkCollection: redditLinkCollection);

            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel();
            ObjectViewModel<IRedditAuthenticatedUser> objectViewModelForRedditAuthenticatedUser = CreateObjectViewModel<IRedditAuthenticatedUser>(redditAuthenticatedUser, DateTime.Now.AddDays(_random.Next(1, 365) * -1).AddMinutes(_random.Next(-120, 120)));
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(dashboardSettingsViewModel: dashboardSettingsViewModel, objectViewModelForRedditAuthenticatedUser: objectViewModelForRedditAuthenticatedUser);

            DashboardViewModel result = await sut.BuildAsync(dashboard);

            _cookieHelperMock.Verify(m => m.ToCookie(It.Is<DashboardViewModel>(value => value == result)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_ReturnsInitializedDashboardViewModel()
        {
            List<INews> newsCollection = CreateNewsCollection(_random.Next(50, 75)).ToList();
            List<ISystemError> systemErrorCollection = CreateSystemErrorCollection(_random.Next(10, 15)).ToList();
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();
            List<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(_random.Next(10, 25)).ToList();
            List<IRedditLink> redditLinkCollection = CreateRedditLinkCollection(_random.Next(50, 75)).ToList();
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection, dashboardSettings: dashboardSettings, redditAuthenticatedUser: redditAuthenticatedUser, redditSubredditCollection: redditSubredditCollection, redditLinkCollection: redditLinkCollection);

            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel();
            ObjectViewModel<IRedditAuthenticatedUser> objectViewModelForRedditAuthenticatedUser = CreateObjectViewModel<IRedditAuthenticatedUser>(redditAuthenticatedUser, DateTime.Now.AddDays(_random.Next(1, 365) * -1).AddMinutes(_random.Next(-120, 120)));
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(dashboardSettingsViewModel: dashboardSettingsViewModel, objectViewModelForRedditAuthenticatedUser: objectViewModelForRedditAuthenticatedUser);

            DashboardViewModel result = await sut.BuildAsync(dashboard);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(newsCollection.Count + redditLinkCollection.Count, result.Informations.Count());
            Assert.IsNotNull(result.LatestInformationsWithImage);
            Assert.AreEqual(Math.Min(result.Informations.Count(information => string.IsNullOrWhiteSpace(information.ImageUrl) == false), 7), result.LatestInformationsWithImage.Count());
            Assert.IsNotNull(result.SystemErrors);
            Assert.AreEqual(systemErrorCollection.Count, result.SystemErrors.Count());
            Assert.IsNotNull(result.Settings);
            Assert.AreEqual(dashboardSettingsViewModel, result.Settings);
            Assert.IsNotNull(result.RedditAuthenticatedUser);
            Assert.AreEqual(objectViewModelForRedditAuthenticatedUser, result.RedditAuthenticatedUser);
            Assert.IsNotNull(result.RedditSubreddits);
            Assert.AreEqual(redditSubredditCollection.Count, result.RedditSubreddits.Count());
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndNewsToInformationViewModelBuilderThrowsAggregateException_AddsExceptionToSystemViewModelsInDashboardViewModel()
        {
            IEnumerable<INews> newsCollection = CreateNewsCollection(1);
            IEnumerable<ISystemError> systemErrorCollection = CreateSystemErrorCollection(0);
            IEnumerable<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(0);
            IEnumerable<IRedditLink> redditLinkCollection = CreateRedditLinkCollection(0);
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection, redditSubredditCollection: redditSubredditCollection, redditLinkCollection: redditLinkCollection);

            string aggregateExceptionMessage = Guid.NewGuid().ToString();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(aggregateExceptionMessage: aggregateExceptionMessage);

            DashboardViewModel result = await sut.BuildAsync(dashboard);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(0, result.Informations.Count());
            Assert.IsNotNull(result.LatestInformationsWithImage);
            Assert.AreEqual(0, result.LatestInformationsWithImage.Count());
            Assert.IsNotNull(result.SystemErrors);
            Assert.AreEqual(1, result.SystemErrors.Count());
            Assert.IsNull(result.Settings);
            Assert.IsNull(result.RedditAuthenticatedUser);
            Assert.IsNotNull(result.RedditSubreddits);
            Assert.AreEqual(0, result.RedditSubreddits.Count());

            SystemErrorViewModel systemErrorViewModel = result.SystemErrors.First();
            Assert.IsNotNull(systemErrorViewModel);
            Assert.IsNotNull(systemErrorViewModel.SystemErrorIdentifier);
            Assert.IsTrue(systemErrorViewModel.Timestamp >= DateTime.Now.AddSeconds(-3) && systemErrorViewModel.Timestamp <= DateTime.Now);
            Assert.IsNotNull(systemErrorViewModel.Message);
            Assert.AreEqual($"HtmlHelper.ConvertNewLines:{aggregateExceptionMessage}", systemErrorViewModel.Message);
            Assert.IsNotNull(systemErrorViewModel.Details);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndSystemErrorViewModelBuilderThrowsAggregateException_AddsExceptionToSystemViewModelsInDashboardViewModel()
        {
            IEnumerable<INews> newsCollection = CreateNewsCollection(0);
            IEnumerable<ISystemError> systemErrorCollection = CreateSystemErrorCollection(1);
            IEnumerable<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(0);
            IEnumerable<IRedditLink> redditLinkCollection = CreateRedditLinkCollection(0);
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection, redditSubredditCollection: redditSubredditCollection, redditLinkCollection: redditLinkCollection);

            string aggregateExceptionMessage = Guid.NewGuid().ToString();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(aggregateExceptionMessage: aggregateExceptionMessage);

            DashboardViewModel result = await sut.BuildAsync(dashboard);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(0, result.Informations.Count());
            Assert.IsNotNull(result.LatestInformationsWithImage);
            Assert.AreEqual(0, result.LatestInformationsWithImage.Count());
            Assert.IsNotNull(result.SystemErrors);
            Assert.AreEqual(1, result.SystemErrors.Count());
            Assert.IsNull(result.Settings);
            Assert.IsNull(result.RedditAuthenticatedUser);
            Assert.IsNotNull(result.RedditSubreddits);
            Assert.AreEqual(0, result.RedditSubreddits.Count());

            SystemErrorViewModel systemErrorViewModel = result.SystemErrors.First();
            Assert.IsNotNull(systemErrorViewModel);
            Assert.IsNotNull(systemErrorViewModel.SystemErrorIdentifier);
            Assert.IsTrue(systemErrorViewModel.Timestamp >= DateTime.Now.AddSeconds(-3) && systemErrorViewModel.Timestamp <= DateTime.Now);
            Assert.IsNotNull(systemErrorViewModel.Message);
            Assert.AreEqual($"HtmlHelper.ConvertNewLines:{aggregateExceptionMessage}", systemErrorViewModel.Message);
            Assert.IsNotNull(systemErrorViewModel.Details);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndDashboardSettingsViewModelBuilderThrowsAggregateException_AddsExceptionToSystemViewModelsInDashboardViewModel()
        {
            IEnumerable<INews> newsCollection = CreateNewsCollection(0);
            IEnumerable<ISystemError> systemErrorCollection = CreateSystemErrorCollection(0);
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IEnumerable<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(0);
            IEnumerable<IRedditLink> redditLinkCollection = CreateRedditLinkCollection(0);
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection, dashboardSettings: dashboardSettings, redditSubredditCollection: redditSubredditCollection, redditLinkCollection: redditLinkCollection);

            string aggregateExceptionMessage = Guid.NewGuid().ToString();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(aggregateExceptionMessage: aggregateExceptionMessage);

            DashboardViewModel result = await sut.BuildAsync(dashboard);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(0, result.Informations.Count());
            Assert.IsNotNull(result.LatestInformationsWithImage);
            Assert.AreEqual(0, result.LatestInformationsWithImage.Count());
            Assert.IsNotNull(result.SystemErrors);
            Assert.AreEqual(1, result.SystemErrors.Count());
            Assert.IsNull(result.Settings);
            Assert.IsNull(result.RedditAuthenticatedUser);
            Assert.IsNotNull(result.RedditSubreddits);
            Assert.AreEqual(0, result.RedditSubreddits.Count());

            SystemErrorViewModel systemErrorViewModel = result.SystemErrors.First();
            Assert.IsNotNull(systemErrorViewModel);
            Assert.IsNotNull(systemErrorViewModel.SystemErrorIdentifier);
            Assert.IsTrue(systemErrorViewModel.Timestamp >= DateTime.Now.AddSeconds(-3) && systemErrorViewModel.Timestamp <= DateTime.Now);
            Assert.IsNotNull(systemErrorViewModel.Message);
            Assert.AreEqual($"HtmlHelper.ConvertNewLines:{aggregateExceptionMessage}", systemErrorViewModel.Message);
            Assert.IsNotNull(systemErrorViewModel.Details);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndRedditAuthenticatedUserToObjectViewModelBuilderThrowsAggregateException_AddsExceptionToSystemViewModelsInDashboardViewModel()
        {
            IEnumerable<INews> newsCollection = CreateNewsCollection(0);
            IEnumerable<ISystemError> systemErrorCollection = CreateSystemErrorCollection(0);
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();
            IEnumerable<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(0);
            IEnumerable<IRedditLink> redditLinkCollection = CreateRedditLinkCollection(0);
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection, redditAuthenticatedUser: redditAuthenticatedUser, redditSubredditCollection: redditSubredditCollection, redditLinkCollection: redditLinkCollection);

            string aggregateExceptionMessage = Guid.NewGuid().ToString();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(aggregateExceptionMessage: aggregateExceptionMessage);

            DashboardViewModel result = await sut.BuildAsync(dashboard);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(0, result.Informations.Count());
            Assert.IsNotNull(result.LatestInformationsWithImage);
            Assert.AreEqual(0, result.LatestInformationsWithImage.Count());
            Assert.IsNotNull(result.SystemErrors);
            Assert.AreEqual(1, result.SystemErrors.Count());
            Assert.IsNull(result.Settings);
            Assert.IsNull(result.RedditAuthenticatedUser);
            Assert.IsNotNull(result.RedditSubreddits);
            Assert.AreEqual(0, result.RedditSubreddits.Count());

            SystemErrorViewModel systemErrorViewModel = result.SystemErrors.First();
            Assert.IsNotNull(systemErrorViewModel);
            Assert.IsNotNull(systemErrorViewModel.SystemErrorIdentifier);
            Assert.IsTrue(systemErrorViewModel.Timestamp >= DateTime.Now.AddSeconds(-3) && systemErrorViewModel.Timestamp <= DateTime.Now);
            Assert.IsNotNull(systemErrorViewModel.Message);
            Assert.AreEqual($"HtmlHelper.ConvertNewLines:{aggregateExceptionMessage}", systemErrorViewModel.Message);
            Assert.IsNotNull(systemErrorViewModel.Details);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndRedditSubredditToObjectViewModelBuilderThrowsAggregateException_AddsExceptionToSystemViewModelsInDashboardViewModel()
        {
            IEnumerable<INews> newsCollection = CreateNewsCollection(0);
            IEnumerable<ISystemError> systemErrorCollection = CreateSystemErrorCollection(0);
            IEnumerable<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(1);
            IEnumerable<IRedditLink> redditLinkCollection = CreateRedditLinkCollection(0);
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection, redditSubredditCollection: redditSubredditCollection, redditLinkCollection: redditLinkCollection);

            string aggregateExceptionMessage = Guid.NewGuid().ToString();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(aggregateExceptionMessage: aggregateExceptionMessage);

            DashboardViewModel result = await sut.BuildAsync(dashboard);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(0, result.Informations.Count());
            Assert.IsNotNull(result.LatestInformationsWithImage);
            Assert.AreEqual(0, result.LatestInformationsWithImage.Count());
            Assert.IsNotNull(result.SystemErrors);
            Assert.AreEqual(1, result.SystemErrors.Count());
            Assert.IsNull(result.Settings);
            Assert.IsNull(result.RedditAuthenticatedUser);
            Assert.IsNotNull(result.RedditSubreddits);
            Assert.AreEqual(0, result.RedditSubreddits.Count());

            SystemErrorViewModel systemErrorViewModel = result.SystemErrors.First();
            Assert.IsNotNull(systemErrorViewModel);
            Assert.IsNotNull(systemErrorViewModel.SystemErrorIdentifier);
            Assert.IsTrue(systemErrorViewModel.Timestamp >= DateTime.Now.AddSeconds(-3) && systemErrorViewModel.Timestamp <= DateTime.Now);
            Assert.IsNotNull(systemErrorViewModel.Message);
            Assert.AreEqual($"HtmlHelper.ConvertNewLines:{aggregateExceptionMessage}", systemErrorViewModel.Message);
            Assert.IsNotNull(systemErrorViewModel.Details);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndRedditLinkToInformationViewModelBuilderThrowsAggregateException_AddsExceptionToSystemViewModelsInDashboardViewModel()
        {
            IEnumerable<INews> newsCollection = CreateNewsCollection(0);
            IEnumerable<ISystemError> systemErrorCollection = CreateSystemErrorCollection(0);
            IEnumerable<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(0);
            IEnumerable<IRedditLink> redditLinkCollection = CreateRedditLinkCollection(1);
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection, redditSubredditCollection: redditSubredditCollection, redditLinkCollection: redditLinkCollection);

            string aggregateExceptionMessage = Guid.NewGuid().ToString();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(aggregateExceptionMessage: aggregateExceptionMessage);

            DashboardViewModel result = await sut.BuildAsync(dashboard);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(0, result.Informations.Count());
            Assert.IsNotNull(result.LatestInformationsWithImage);
            Assert.AreEqual(0, result.LatestInformationsWithImage.Count());
            Assert.IsNotNull(result.SystemErrors);
            Assert.AreEqual(1, result.SystemErrors.Count());
            Assert.IsNull(result.Settings);
            Assert.IsNull(result.RedditAuthenticatedUser);
            Assert.IsNotNull(result.RedditSubreddits);
            Assert.AreEqual(0, result.RedditSubreddits.Count());

            SystemErrorViewModel systemErrorViewModel = result.SystemErrors.First();
            Assert.IsNotNull(systemErrorViewModel);
            Assert.IsNotNull(systemErrorViewModel.SystemErrorIdentifier);
            Assert.IsTrue(systemErrorViewModel.Timestamp >= DateTime.Now.AddSeconds(-3) && systemErrorViewModel.Timestamp <= DateTime.Now);
            Assert.IsNotNull(systemErrorViewModel.Message);
            Assert.AreEqual($"HtmlHelper.ConvertNewLines:{aggregateExceptionMessage}", systemErrorViewModel.Message);
            Assert.IsNotNull(systemErrorViewModel.Details);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndAggregateExceptionOccurs_AssertConvertNewLinesWasCalledOnHtmlHelperTwice()
        {
            IEnumerable<INews> newsCollection = CreateNewsCollection(1);
            IEnumerable<ISystemError> systemErrorCollection = CreateSystemErrorCollection(0);
            IEnumerable<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(0);
            IEnumerable<IRedditLink> redditLinkCollection = CreateRedditLinkCollection(0);
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection, redditSubredditCollection: redditSubredditCollection, redditLinkCollection: redditLinkCollection);

            string aggregateExceptionMessage = Guid.NewGuid().ToString();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(aggregateExceptionMessage: aggregateExceptionMessage);

            await sut.BuildAsync(dashboard);

            _htmlHelperMock.Verify(m => m.ConvertNewLines(It.IsAny<string>()), Times.Exactly(2));
        }

        private IViewModelBuilder<DashboardViewModel, IDashboard> CreateSut(DashboardSettingsViewModel dashboardSettingsViewModel = null, ObjectViewModel<IRedditAuthenticatedUser> objectViewModelForRedditAuthenticatedUser = null, string aggregateExceptionMessage = null)
        {
            _newsToInformationViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<INews>()))
                .Returns<INews>(news => Task.Run<InformationViewModel>(() => 
                {
                    if (string.IsNullOrWhiteSpace(aggregateExceptionMessage) == false)
                    {
                        throw new Exception(aggregateExceptionMessage);
                    }
                    return CreateInformationViewModel(DateTime.Now.AddMinutes(_random.Next(1, 30) * -1), _random.Next(100) > 50);
                }));

            _systemErrorViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<ISystemError>()))
                .Returns<ISystemError>(systemError => Task.Run<SystemErrorViewModel>(() => 
                {
                    if (string.IsNullOrWhiteSpace(aggregateExceptionMessage) == false)
                    {
                        throw new Exception(aggregateExceptionMessage);
                    }
                    return CreateSystemErrorViewModel(DateTime.Now.AddMinutes(_random.Next(1, 30) * -1));
                }));

            _dashboardSettingsViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<IDashboardSettings>()))
                .Returns<IDashboardSettings>(dashboardSettings => Task.Run<DashboardSettingsViewModel>(() => 
                {
                    if (string.IsNullOrWhiteSpace(aggregateExceptionMessage) == false)
                    {
                        throw new Exception(aggregateExceptionMessage);
                    }
                    return dashboardSettingsViewModel ?? CreateDashboardSettingsViewModel();
                }));

            _redditAuthenticatedUserToObjectViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<IRedditAuthenticatedUser>()))
                .Returns<IRedditAuthenticatedUser>(redditAuthenticatedUser => Task.Run<ObjectViewModel<IRedditAuthenticatedUser>>(() => 
                {
                    if (string.IsNullOrWhiteSpace(aggregateExceptionMessage) == false)
                    {
                        throw new Exception(aggregateExceptionMessage);
                    }
                    return objectViewModelForRedditAuthenticatedUser ?? CreateObjectViewModel<IRedditAuthenticatedUser>(redditAuthenticatedUser, DateTime.Now.AddDays(_random.Next(1, 365) * -1).AddMinutes(_random.Next(-120, 120)));
                }));

            _redditSubredditToObjectViewModelBuilder.Setup(m => m.BuildAsync(It.IsAny<IRedditSubreddit>()))
                .Returns<IRedditSubreddit>(redditSubreddit => Task.Run<ObjectViewModel<IRedditSubreddit>>(() => 
                {
                    if (string.IsNullOrWhiteSpace(aggregateExceptionMessage) == false)
                    {
                        throw new Exception(aggregateExceptionMessage);
                    }
                    return CreateObjectViewModel<IRedditSubreddit>(redditSubreddit, DateTime.Now.AddDays(_random.Next(1, 365) * -1).AddMinutes(_random.Next(-120, 120)));
                }));

            _redditLinkToInformationViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<IRedditLink>()))
                .Returns<IRedditLink>(redditLink => Task.Run<InformationViewModel>(() => 
                {
                    if (string.IsNullOrWhiteSpace(aggregateExceptionMessage) == false)
                    {
                        throw new Exception(aggregateExceptionMessage);
                    }
                    return CreateInformationViewModel(DateTime.Now.AddMinutes(_random.Next(1, 30) * -1), _random.Next(100) > 50);
                }));

            _htmlHelperMock.Setup(m => m.ConvertNewLines(It.IsAny<string>()))
                .Returns<string>(value => $"HtmlHelper.ConvertNewLines:{value}");
            _httpHelperMock.Setup(m => m.ReadAsync(It.IsAny<Uri>()))
                .Returns(Task.Run<byte[]>(() => Convert.FromBase64String(ImageHelper.ImageAsBase64)));

            return new OSDevGrp.MyDashboard.Web.Factories.DashboardViewModelBuilder(
                _newsToInformationViewModelBuilderMock.Object,
                _systemErrorViewModelBuilderMock.Object,
                _dashboardSettingsViewModelBuilderMock.Object,
                _redditAuthenticatedUserToObjectViewModelBuilderMock.Object,
                _redditSubredditToObjectViewModelBuilder.Object,
                _redditLinkToInformationViewModelBuilderMock.Object,
                _htmlHelperMock.Object,
                _httpHelperMock.Object,
                _cookieHelperMock.Object);
        }

        private IDashboard CreateDashboard(IEnumerable<INews> newsCollection = null, IEnumerable<ISystemError> systemErrorCollection = null, IDashboardSettings dashboardSettings = null, IRedditAuthenticatedUser redditAuthenticatedUser = null, IEnumerable<IRedditSubreddit> redditSubredditCollection = null, IEnumerable<IRedditLink> redditLinkCollection = null, IDashboardRules dashboardRules = null)
        {
            return CreateDashboardMock(newsCollection, systemErrorCollection, dashboardSettings, redditAuthenticatedUser, redditSubredditCollection, redditLinkCollection, dashboardRules).Object;
        }

        private Mock<IDashboard> CreateDashboardMock(IEnumerable<INews> newsCollection = null, IEnumerable<ISystemError> systemErrorCollection = null, IDashboardSettings dashboardSettings = null, IRedditAuthenticatedUser redditAuthenticatedUser = null, IEnumerable<IRedditSubreddit> redditSubredditCollection = null, IEnumerable<IRedditLink> redditLinkCollection = null, IDashboardRules dashboardRules = null)
        {
            Mock<IDashboard> dashboardMock = new Mock<IDashboard>();
            dashboardMock.Setup(m => m.News)
                .Returns(newsCollection ?? CreateNewsCollection(_random.Next(50, 100)));
            dashboardMock.Setup(m => m.SystemErrors)
                .Returns(systemErrorCollection ?? CreateSystemErrorCollection(_random.Next(5, 10)));
            dashboardMock.Setup(m => m.Settings)
                .Returns(dashboardSettings);
            dashboardMock.Setup(m => m.RedditAuthenticatedUser)
                .Returns(redditAuthenticatedUser);
            dashboardMock.Setup(m => m.RedditSubreddits)
                .Returns(redditSubredditCollection ?? CreateRedditSubredditCollection(_random.Next(10, 25)));
            dashboardMock.Setup(m => m.RedditLinks)
                .Returns(redditLinkCollection ?? CreateRedditLinkCollection(_random.Next(50, 100)));
            dashboardMock.Setup(m => m.Rules)
                .Returns(dashboardRules ?? CreateDashboardRules());
            return dashboardMock;
        }

        private IEnumerable<INews> CreateNewsCollection(int numberOfNews)
        {
            IList<INews> newsCollection = new List<INews>(numberOfNews);
            while (newsCollection.Count < numberOfNews)
            {
                Mock<INews> newsMock = new Mock<INews>();
                newsCollection.Add(newsMock.Object);
            }
            return newsCollection;
        }

        private IEnumerable<ISystemError> CreateSystemErrorCollection(int numberOfSystemErrors)
        {
            IList<ISystemError> systemErrorCollection = new List<ISystemError>(numberOfSystemErrors);
            while (systemErrorCollection.Count < numberOfSystemErrors)
            {
                Mock<ISystemError> systemErrorMock = new Mock<ISystemError>();
                systemErrorCollection.Add(systemErrorMock.Object);
            }
            return systemErrorCollection;
        }

        private IDashboardSettings CreateDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = new Mock<IDashboardSettings>();
            return dashboardSettingsMock.Object;
        }

        private IRedditAuthenticatedUser CreateRedditAuthenticatedUser()
        {
            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = new Mock<IRedditAuthenticatedUser>();
            return redditAuthenticatedUserMock.Object;
        }

        private IEnumerable<IRedditSubreddit> CreateRedditSubredditCollection(int numberOfRedditSubreddits)
        {
            IList<IRedditSubreddit> redditSubredditCollection = new List<IRedditSubreddit>(numberOfRedditSubreddits);
            while (redditSubredditCollection.Count < numberOfRedditSubreddits)
            {
                Mock<IRedditSubreddit> redditSubredditMock = new Mock<IRedditSubreddit>();
                redditSubredditMock.Setup(m => m.Subscribers)
                    .Returns(_random.Next(2500, 10000));
                redditSubredditCollection.Add(redditSubredditMock.Object);
            }
            return redditSubredditCollection;
        }

        private IEnumerable<IRedditLink> CreateRedditLinkCollection(int numberOfRedditLinks)
        {
            IList<IRedditLink> redditLinksCollection = new List<IRedditLink>(numberOfRedditLinks);
            while (redditLinksCollection.Count < numberOfRedditLinks)
            {
                Mock<IRedditLink> redditLinkMock = new Mock<IRedditLink>();
                redditLinksCollection.Add(redditLinkMock.Object);
            }
            return redditLinksCollection;
        }

        private IDashboardRules CreateDashboardRules()
        {
            Mock<IDashboardRules> dashboardRulesMock = new Mock<IDashboardRules>();
            return dashboardRulesMock.Object;
        }

        private InformationViewModel CreateInformationViewModel(DateTime timestamp, bool hasImageUrl)
        {
            return new InformationViewModel
            {
                Timestamp = timestamp,
                ImageUrl = hasImageUrl ? $"http://localhost/{Guid.NewGuid().ToString("D")}.png" : null
            };
        }

        private SystemErrorViewModel CreateSystemErrorViewModel(DateTime timestamp)
        {
            return new SystemErrorViewModel
            {
                Timestamp = timestamp
            };
        }

        private DashboardSettingsViewModel CreateDashboardSettingsViewModel()
        {
            return new DashboardSettingsViewModel();
        }

        private ObjectViewModel<TObject> CreateObjectViewModel<TObject>(TObject obj, DateTime timestamp) where TObject : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            return new ObjectViewModel<TObject>()
            {
                Object = obj,
                Timestamp = timestamp
            };
        }
    } 
}