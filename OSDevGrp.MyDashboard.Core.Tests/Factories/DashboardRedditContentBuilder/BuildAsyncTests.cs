using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Factories.DashboardRedditContentBuilder
{
    [TestClass]
    public class BuildAsyncTests
    {
        #region Private variables

        private Mock<IRedditLogic> _redditLogicMock;
        private Mock<IExceptionHandler> _exceptionHandlerMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _redditLogicMock = new Mock<IRedditLogic>();
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
            _random = new Random(DateTime.Now.Millisecond);
        }
        
        [TestMethod]
        [ExpectedArgumentNullException("dashboardSettings")]
        public void BuildAsync_WhenDashboardSettingsIsNull_ThrowsArgumentNullException()
        {
            const IDashboardSettings dashboardSettings = null; 
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut();

            sut.BuildAsync(dashboardSettings, dashboard);
        }

        [TestMethod]
        [ExpectedArgumentNullException("dashboard")]
        public void BuildAsync_WhenDashboardIsNull_ThrowsArgumentNullException()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings(); 
            const IDashboard dashboard = null;

            IDashboardRedditContentBuilder sut = CreateSut();

            sut.BuildAsync(dashboardSettings, dashboard);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertRedditAccessTokenWasCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock(); 
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut();

            Task buildTask = sut.BuildAsync(dashboardSettingsMock.Object, dashboard);
            buildTask.Wait();

            dashboardSettingsMock.Verify(m => m.RedditAccessToken, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertExpiresWasCalledOnRedditAccessToken()
        {
            Mock<IRedditAccessToken> redditAccessTokenMock = CreateRedditAccessTokenMock();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessTokenMock.Object);
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut();

            Task buildTask = sut.BuildAsync(dashboardSettings, dashboard);
            buildTask.Wait();

            redditAccessTokenMock.Verify(m => m.Expires, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertGetAuthenticatedUserAsyncWasCalledOnRedditLogic()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken);
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut();

            Task buildTask = sut.BuildAsync(dashboardSettings, dashboard);
            buildTask.Wait();

            _redditLogicMock.Verify(m => m.GetAuthenticatedUserAsync(It.Is<IRedditAccessToken>(value => value == redditAccessToken)), Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledAndDoesNotHaveAuthenticatedUser_AssertReplaceWasNotCalledOnDashbaordWithAuthenticatedUser()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: false);

            Task buildTask = sut.BuildAsync(dashboardSettings, dashboardMock.Object);
            buildTask.Wait();

            dashboardMock.Verify(m => m.Replace(It.IsAny<IRedditAuthenticatedUser>()), Times.Never);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledAndDoesNotHaveAuthenticatedUser_AssertIncludeNsfwContentWasNotCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: false);

            Task buildTask = sut.BuildAsync(dashboardSettingsMock.Object, dashboard);
            buildTask.Wait();

            dashboardSettingsMock.Verify(m => m.IncludeNsfwContent, Times.Never);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledAndDoesNotHaveAuthenticatedUser_AssertOnlyNsfwContentWasNotCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: false);

            Task buildTask = sut.BuildAsync(dashboardSettingsMock.Object, dashboard);
            buildTask.Wait();

            dashboardSettingsMock.Verify(m => m.OnlyNsfwContent, Times.Never);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledAndHaveAuthenticatedUser_AssertReplaceWasCalledOnDashbaordWithAuthenticatedUser()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser);

            Task buildTask = sut.BuildAsync(dashboardSettings, dashboardMock.Object);
            buildTask.Wait();

            dashboardMock.Verify(m => m.Replace(It.Is<IRedditAuthenticatedUser>(value => value == redditAuthenticatedUser)), Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledAndHaveAuthenticatedUser_AssertOver18WasCalledOnRedditAuthenticatedUser()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IDashboard dashboard = CreateDashboard();

            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = CreateRedditAuthenticatedUserMock();
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUserMock.Object);

            Task buildTask = sut.BuildAsync(dashboardSettings, dashboard);
            buildTask.Wait();

            redditAuthenticatedUserMock.Verify(m => m.Over18, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledAndHaveAuthenticatedUser_AssertIncludeNsfwContentWasCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();
            IDashboard dashboard = CreateDashboard();

            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser);

            Task buildTask = sut.BuildAsync(dashboardSettingsMock.Object, dashboard);
            buildTask.Wait();

            dashboardSettingsMock.Verify(m => m.IncludeNsfwContent, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledAndHaveAuthenticatedUser_AssertOnlyNsfwContentWasCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();
            IDashboard dashboard = CreateDashboard();

            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser);

            Task buildTask = sut.BuildAsync(dashboardSettingsMock.Object, dashboard);
            buildTask.Wait();

            dashboardSettingsMock.Verify(m => m.OnlyNsfwContent, Times.Once);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut();

            Task buildTask = sut.BuildAsync(dashboardSettings, dashboard);
            buildTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IDashboard dashboard = CreateDashboard();

            Exception exception = new Exception();
            IDashboardRedditContentBuilder sut = CreateSut(exception: exception);

            Task buildTask = sut.BuildAsync(dashboardSettings, dashboard);
            buildTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(value => value == exception)), Times.Once);
        }

        private IDashboardRedditContentBuilder CreateSut(bool hasRedditAuthenticatedUser = true, IRedditAuthenticatedUser redditAuthenticatedUser = null, Exception exception = null)
        {
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.Run(() => { }));

            if (exception != null)
            {
                _redditLogicMock.Setup(m => m.GetAuthenticatedUserAsync(It.IsAny<IRedditAccessToken>()))
                    .Throws(exception);
            }
            else if (hasRedditAuthenticatedUser == false)
            {
                _redditLogicMock.Setup(m => m.GetAuthenticatedUserAsync(It.IsAny<IRedditAccessToken>()))
                    .Returns(Task.Run(() => (IRedditAuthenticatedUser) null));
            }
            else
            {
                _redditLogicMock.Setup(m => m.GetAuthenticatedUserAsync(It.IsAny<IRedditAccessToken>()))
                    .Returns(Task.Run(() => redditAuthenticatedUser ?? CreateRedditAuthenticatedUser()));
            }

            return new OSDevGrp.MyDashboard.Core.Factories.DashboardRedditContentBuilder(
                _redditLogicMock.Object,
                _exceptionHandlerMock.Object
            );
        }

        private IDashboardSettings CreateDashboardSettings(IRedditAccessToken redditAccessToken = null, bool? includeNsfwContent = null, bool? onlyNsfwContent = null)
        {
            return CreateDashboardSettingsMock(redditAccessToken, includeNsfwContent, onlyNsfwContent).Object;
        }

        private Mock<IDashboardSettings> CreateDashboardSettingsMock(IRedditAccessToken redditAccessToken = null, bool? includeNsfwContent = null, bool? onlyNsfwContent = null)
        {
            Mock<IDashboardSettings> dashboardSettingsMock = new Mock<IDashboardSettings>();
            dashboardSettingsMock.Setup(m => m.RedditAccessToken)
                .Returns(redditAccessToken ?? CreateRedditAccessToken());
            dashboardSettingsMock.Setup(m => m.IncludeNsfwContent)
                .Returns(includeNsfwContent ?? _random.Next(1, 100) > 50);
            dashboardSettingsMock.Setup(m => m.OnlyNsfwContent)
                .Returns(onlyNsfwContent ?? _random.Next(1, 100) > 50);
            return dashboardSettingsMock;
        }

        private IDashboard CreateDashboard()
        {
            return CreateDashboardMock().Object;
        }

        private Mock<IDashboard> CreateDashboardMock()
        {
            Mock<IDashboard> dashboardMock = new Mock<IDashboard>();
            return dashboardMock;
        }

        private IRedditAccessToken CreateRedditAccessToken(DateTime? expires = null)
        {
            return CreateRedditAccessTokenMock(expires).Object;
        }

        private Mock<IRedditAccessToken> CreateRedditAccessTokenMock(DateTime? expires = null)
        {
            Mock<IRedditAccessToken> redditAccessTokenMock = new Mock<IRedditAccessToken>();
            redditAccessTokenMock.Setup(m => m.Expires)
                .Returns(expires ?? DateTime.Now.AddSeconds(300));
            return redditAccessTokenMock;
        }

        private IRedditAuthenticatedUser CreateRedditAuthenticatedUser(bool? over18 = null)
        {
            return CreateRedditAuthenticatedUserMock(over18).Object;
        }

        private Mock<IRedditAuthenticatedUser> CreateRedditAuthenticatedUserMock(bool? over18 = null)
        {
            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = new Mock<IRedditAuthenticatedUser>();
            redditAuthenticatedUserMock.Setup(m => m.Over18)
                .Returns(over18 ?? _random.Next(1, 100) > 50);
            return redditAuthenticatedUserMock;
        }
    }
}