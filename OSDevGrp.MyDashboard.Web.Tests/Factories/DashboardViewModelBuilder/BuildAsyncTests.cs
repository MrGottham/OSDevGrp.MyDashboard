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
        private Mock<IHtmlHelper> _htmlHelperMock;
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
            _htmlHelperMock = new Mock<IHtmlHelper>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullExceptionAttribute("input")]
        public void BuildAsync_WhenDashboardIsNull_ThrowsArgumentNullException()
        {
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            sut.BuildAsync(null);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertNewsWasCalledOnDashboardOnce()
        {
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboardMock.Object);
            buildTask.Wait();

            dashboardMock.Verify(m => m.News, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertSystemErrorsWasCalledOnDashboardOnce()
        {
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboardMock.Object);
            buildTask.Wait();

            dashboardMock.Verify(m => m.SystemErrors, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertSettingsWasCalledOnDashboardOnce()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            Mock<IDashboard> dashboardMock = CreateDashboardMock(dashboardSettings: dashboardSettings);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboardMock.Object);
            buildTask.Wait();

            dashboardMock.Verify(m => m.Settings, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertRedditAuthenticatedUserWasCalledOnDashboardOnce()
        {
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();
            Mock<IDashboard> dashboardMock = CreateDashboardMock(redditAuthenticatedUser: redditAuthenticatedUser);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboardMock.Object);
            buildTask.Wait();

            dashboardMock.Verify(m => m.RedditAuthenticatedUser, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertRedditSubredditsWasCalledOnDashboardOnce()
        {
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboardMock.Object);
            buildTask.Wait();

            dashboardMock.Verify(m => m.RedditSubreddits, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertBuildAsyncWasCalledOnNewsToInformationViewModelBuilderForEachNewsInDashboard()
        {
            List<INews> newsCollection = CreateNewsCollection(_random.Next(50, 75)).ToList();
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            newsCollection.ForEach(news => _newsToInformationViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<INews>(value => value == news)), Times.Once));
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertBuildAsyncWasCalledOnSystemErrorViewModelBuilderForEachSystemErrorInDashboard()
        {
            List<ISystemError> systemErrorCollection = CreateSystemErrorCollection(_random.Next(50, 75)).ToList();
            IDashboard dashboard = CreateDashboard(systemErrorCollection: systemErrorCollection);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            systemErrorCollection.ForEach(systemError => _systemErrorViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<ISystemError>(value => value == systemError)), Times.Once));
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertBuildAsyncWasCalledOnDashboardSettingsViewModelBuilderWithSettingsInDashboard()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IDashboard dashboard = CreateDashboard(dashboardSettings: dashboardSettings);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            _dashboardSettingsViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value == dashboardSettings)), Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertBuildAsyncWasCalledOnRedditAuthenticatedUserToObjectViewModelBuilderWithRedditAuthenticatedUserInDashboard()
        {
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();
            IDashboard dashboard = CreateDashboard(redditAuthenticatedUser: redditAuthenticatedUser);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            _redditAuthenticatedUserToObjectViewModelBuilderMock.Verify(m => m.BuildAsync(It.Is<IRedditAuthenticatedUser>(value => value == redditAuthenticatedUser)), Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertBuildAsyncWasCalledOnRedditSubredditToObjectViewModelBuilderForEachRedditSubredditInDashboard()
        {
            List<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(_random.Next(50, 75)).ToList();
            IDashboard dashboard = CreateDashboard(redditSubredditCollection: redditSubredditCollection);

            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            redditSubredditCollection.ForEach(redditSubreddit => _redditSubredditToObjectViewModelBuilder.Verify(m => m.BuildAsync(It.Is<IRedditSubreddit>(value => value == redditSubreddit)), Times.Once));
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_ReturnsInitializedDashboardViewModel()
        {
            List<INews> newsCollection = CreateNewsCollection(_random.Next(50, 75)).ToList();
            List<ISystemError> systemErrorCollection = CreateSystemErrorCollection(_random.Next(10, 15)).ToList();
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();
            List<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(_random.Next(10, 25)).ToList();
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection, dashboardSettings: dashboardSettings, redditAuthenticatedUser: redditAuthenticatedUser, redditSubredditCollection: redditSubredditCollection);

            DashboardSettingsViewModel dashboardSettingsViewModel = CreateDashboardSettingsViewModel();
            ObjectViewModel<IRedditAuthenticatedUser> objectViewModelForRedditAuthenticatedUser = CreateObjectViewModel<IRedditAuthenticatedUser>();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(dashboardSettingsViewModel: dashboardSettingsViewModel, objectViewModelForRedditAuthenticatedUser: objectViewModelForRedditAuthenticatedUser);

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            DashboardViewModel result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(newsCollection.Count, result.Informations.Count());
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
        public void BuildAsync_WhenCalledAndNewsToInformationViewModelBuilderThrowsAggregateException_AddsExceptionToSystemViewModelsInDashboardViewModel()
        {
            IEnumerable<INews> newsCollection = CreateNewsCollection(1);
            IEnumerable<ISystemError> systemErrorCollection = CreateSystemErrorCollection(0);
            IEnumerable<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(0);
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection, redditSubredditCollection: redditSubredditCollection);

            string aggregateExceptionMessage = Guid.NewGuid().ToString();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(aggregateExceptionMessage: aggregateExceptionMessage);

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            DashboardViewModel result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(0, result.Informations.Count());
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
        public void BuildAsync_WhenCalledAndSystemErrorViewModelBuilderThrowsAggregateException_AddsExceptionToSystemViewModelsInDashboardViewModel()
        {
            IEnumerable<INews> newsCollection = CreateNewsCollection(0);
            IEnumerable<ISystemError> systemErrorCollection = CreateSystemErrorCollection(1);
            IEnumerable<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(0);
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection, redditSubredditCollection: redditSubredditCollection);

            string aggregateExceptionMessage = Guid.NewGuid().ToString();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(aggregateExceptionMessage: aggregateExceptionMessage);

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            DashboardViewModel result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(0, result.Informations.Count());
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
        public void BuildAsync_WhenCalledAndDashboardSettingsViewModelBuilderThrowsAggregateException_AddsExceptionToSystemViewModelsInDashboardViewModel()
        {
            IEnumerable<INews> newsCollection = CreateNewsCollection(0);
            IEnumerable<ISystemError> systemErrorCollection = CreateSystemErrorCollection(0);
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IEnumerable<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(0);
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection, dashboardSettings: dashboardSettings, redditSubredditCollection: redditSubredditCollection);

            string aggregateExceptionMessage = Guid.NewGuid().ToString();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(aggregateExceptionMessage: aggregateExceptionMessage);

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            DashboardViewModel result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(0, result.Informations.Count());
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
        public void BuildAsync_WhenCalledAndRedditAuthenticatedUserToObjectViewModelBuilderThrowsAggregateException_AddsExceptionToSystemViewModelsInDashboardViewModel()
        {
            IEnumerable<INews> newsCollection = CreateNewsCollection(0);
            IEnumerable<ISystemError> systemErrorCollection = CreateSystemErrorCollection(0);
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();
            IEnumerable<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(0);
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection, redditAuthenticatedUser: redditAuthenticatedUser, redditSubredditCollection: redditSubredditCollection);

            string aggregateExceptionMessage = Guid.NewGuid().ToString();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(aggregateExceptionMessage: aggregateExceptionMessage);

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            DashboardViewModel result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(0, result.Informations.Count());
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
        public void BuildAsync_WhenCalledAndRedditSubredditToObjectViewModelBuilderThrowsAggregateException_AddsExceptionToSystemViewModelsInDashboardViewModel()
        {
            IEnumerable<INews> newsCollection = CreateNewsCollection(0);
            IEnumerable<ISystemError> systemErrorCollection = CreateSystemErrorCollection(0);
            IEnumerable<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(1);
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection, redditSubredditCollection: redditSubredditCollection);

            string aggregateExceptionMessage = Guid.NewGuid().ToString();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(aggregateExceptionMessage: aggregateExceptionMessage);

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            DashboardViewModel result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(0, result.Informations.Count());
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
        public void BuildAsync_WhenCalledAndAggregateExceptionOccurs_AssertConvertNewLinesWasCalledOnHtmlHelperTwice()
        {
            IEnumerable<INews> newsCollection = CreateNewsCollection(1);
            IEnumerable<ISystemError> systemErrorCollection = CreateSystemErrorCollection(0);
            IEnumerable<IRedditSubreddit> redditSubredditCollection = CreateRedditSubredditCollection(0);
            IDashboard dashboard = CreateDashboard(newsCollection: newsCollection, systemErrorCollection: systemErrorCollection, redditSubredditCollection: redditSubredditCollection);

            string aggregateExceptionMessage = Guid.NewGuid().ToString();
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut(aggregateExceptionMessage: aggregateExceptionMessage);

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            _htmlHelperMock.Verify(m => m.ConvertNewLines(It.IsAny<string>()), Times.Exactly(2));
        }

        private IViewModelBuilder<DashboardViewModel, IDashboard> CreateSut(DashboardSettingsViewModel dashboardSettingsViewModel = null, ObjectViewModel<IRedditAuthenticatedUser> objectViewModelForRedditAuthenticatedUser = null, string aggregateExceptionMessage = null)
        {
            _newsToInformationViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<INews>()))
                .Returns(Task.Run<InformationViewModel>(() => 
                {
                    if (string.IsNullOrWhiteSpace(aggregateExceptionMessage) == false)
                    {
                        throw new Exception(aggregateExceptionMessage);
                    }
                    return CreateInformationViewModel(DateTime.Now.AddMinutes(_random.Next(1, 30) * -1));
                }));

            _systemErrorViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<ISystemError>()))
                .Returns(Task.Run<SystemErrorViewModel>(() => 
                {
                    if (string.IsNullOrWhiteSpace(aggregateExceptionMessage) == false)
                    {
                        throw new Exception(aggregateExceptionMessage);
                    }
                    return CreateSystemErrorViewModel(DateTime.Now.AddMinutes(_random.Next(1, 30) * -1));
                }));

            _dashboardSettingsViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<IDashboardSettings>()))
                .Returns(Task.Run<DashboardSettingsViewModel>(() => 
                {
                    if (string.IsNullOrWhiteSpace(aggregateExceptionMessage) == false)
                    {
                        throw new Exception(aggregateExceptionMessage);
                    }
                    return dashboardSettingsViewModel ?? CreateDashboardSettingsViewModel();
                }));

            _redditAuthenticatedUserToObjectViewModelBuilderMock.Setup(m => m.BuildAsync(It.IsAny<IRedditAuthenticatedUser>()))
                .Returns(Task.Run<ObjectViewModel<IRedditAuthenticatedUser>>(() => 
                {
                    if (string.IsNullOrWhiteSpace(aggregateExceptionMessage) == false)
                    {
                        throw new Exception(aggregateExceptionMessage);
                    }
                    return objectViewModelForRedditAuthenticatedUser ?? CreateObjectViewModel<IRedditAuthenticatedUser>();
                }));

            _redditSubredditToObjectViewModelBuilder.Setup(m => m.BuildAsync(It.IsAny<IRedditSubreddit>()))
                .Returns(Task.Run<ObjectViewModel<IRedditSubreddit>>(() => 
                {
                    if (string.IsNullOrWhiteSpace(aggregateExceptionMessage) == false)
                    {
                        throw new Exception(aggregateExceptionMessage);
                    }
                    return CreateObjectViewModel<IRedditSubreddit>();
                }));

            _htmlHelperMock.Setup(m => m.ConvertNewLines(It.IsAny<string>()))
                .Returns<string>(value => $"HtmlHelper.ConvertNewLines:{value}");

            return new OSDevGrp.MyDashboard.Web.Factories.DashboardViewModelBuilder(
                _newsToInformationViewModelBuilderMock.Object,
                _systemErrorViewModelBuilderMock.Object,
                _dashboardSettingsViewModelBuilderMock.Object,
                _redditAuthenticatedUserToObjectViewModelBuilderMock.Object,
                _redditSubredditToObjectViewModelBuilder.Object,
                _htmlHelperMock.Object);
        }

        private IDashboard CreateDashboard(IEnumerable<INews> newsCollection = null, IEnumerable<ISystemError> systemErrorCollection = null, IDashboardSettings dashboardSettings = null, IRedditAuthenticatedUser redditAuthenticatedUser = null, IEnumerable<IRedditSubreddit> redditSubredditCollection = null)
        {
            return CreateDashboardMock(newsCollection, systemErrorCollection, dashboardSettings, redditAuthenticatedUser, redditSubredditCollection).Object;
        }

        private Mock<IDashboard> CreateDashboardMock(IEnumerable<INews> newsCollection = null, IEnumerable<ISystemError> systemErrorCollection = null, IDashboardSettings dashboardSettings = null, IRedditAuthenticatedUser redditAuthenticatedUser = null, IEnumerable<IRedditSubreddit> redditSubredditCollection = null)
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
                redditSubredditCollection.Add(redditSubredditMock.Object);
            }
            return redditSubredditCollection;
        }

        private InformationViewModel CreateInformationViewModel(DateTime timestamp)
        {
            return new InformationViewModel
            {
                Timestamp = timestamp
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

        private ObjectViewModel<TObject> CreateObjectViewModel<TObject>() where TObject : class
        {
            return new ObjectViewModel<TObject>();
        }
    } 
}