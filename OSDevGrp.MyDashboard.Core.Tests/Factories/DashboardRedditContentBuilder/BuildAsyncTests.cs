using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

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
        public async Task BuildAsync_WhenDashboardSettingsIsNull_ThrowsArgumentNullException()
        {
            const IDashboardSettings dashboardSettings = null; 
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.BuildAsync(dashboardSettings, dashboard));

            Assert.AreEqual("dashboardSettings", result.ParamName);
        }

        [TestMethod]
        public async Task BuildAsync_WhenDashboardIsNull_ThrowsArgumentNullException()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings(); 
            const IDashboard dashboard = null;

            IDashboardRedditContentBuilder sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.BuildAsync(dashboardSettings, dashboard));

            Assert.AreEqual("dashboard", result.ParamName);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertRedditAccessTokenWasCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock(); 
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut();

            await sut.BuildAsync(dashboardSettingsMock.Object, dashboard);

            dashboardSettingsMock.Verify(m => m.RedditAccessToken, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertExpiresWasCalledOnRedditAccessToken()
        {
            Mock<IRedditAccessToken> redditAccessTokenMock = CreateRedditAccessTokenMock();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessTokenMock.Object);
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut();

            await sut.BuildAsync(dashboardSettings, dashboard);

            redditAccessTokenMock.Verify(m => m.Expires, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithUnexpiredRedditAccessToken_AssertRenewAccessTokenAsyncWasNotCalledOnRedditLogic()
        {
            DateTime expires = DateTime.Now.AddSeconds(_random.Next(60, 300));
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken(expires: expires);
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken);
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut();

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.RenewAccessTokenAsync(It.IsAny<IRedditAccessToken>()), Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithUnexpiredRedditAccessToken_AssertRedditAccessTokenSetterWasNotCalledOnDashboardSettings()
        {
            DateTime expires = DateTime.Now.AddSeconds(_random.Next(60, 300));
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken(expires: expires);
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock(redditAccessToken: redditAccessToken);
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut();

            await sut.BuildAsync(dashboardSettingsMock.Object, dashboard);

            dashboardSettingsMock.VerifySet(m => m.RedditAccessToken = It.IsAny<IRedditAccessToken>(), Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithUnexpiredRedditAccessToken_AssertGetAuthenticatedUserAsyncWasCalledOnRedditLogic()
        {
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken);
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut();

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.GetAuthenticatedUserAsync(It.Is<IRedditAccessToken>(value => value == redditAccessToken)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithExpiredRedditAccessToken_AssertRenewAccessTokenAsyncWasCalledOnRedditLogic()
        {
            DateTime expires = DateTime.Now.AddSeconds(_random.Next(60, 300) * -1);
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken(expires: expires);
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken);
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut();

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.RenewAccessTokenAsync(It.Is<IRedditAccessToken>(value => value == redditAccessToken)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithExpiredRedditAccessTokenAndRenewedAccessTokenIsNull_AssertRedditAccessTokenSetterWasNotCalledOnDashboardSettings()
        {
            DateTime expires = DateTime.Now.AddSeconds(_random.Next(60, 300) * -1);
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken(expires: expires);
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock(redditAccessToken: redditAccessToken);
            IDashboard dashboard = CreateDashboard();

            const IRedditAccessToken renewedRedditAccessToken = null;
            IDashboardRedditContentBuilder sut = CreateSut(renewedRedditAccessToken: renewedRedditAccessToken);

            await sut.BuildAsync(dashboardSettingsMock.Object, dashboard);

            dashboardSettingsMock.VerifySet(m => m.RedditAccessToken = It.IsAny<IRedditAccessToken>(), Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithExpiredRedditAccessTokenAndRenewedAccessTokenIsNull_AssertGetAuthenticatedUserAsyncWasNotCalledOnRedditLogic()
        {
            DateTime expires = DateTime.Now.AddSeconds(_random.Next(60, 300) * -1);
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken(expires: expires);
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken);
            IDashboard dashboard = CreateDashboard();

            const IRedditAccessToken renewedRedditAccessToken = null;
            IDashboardRedditContentBuilder sut = CreateSut(renewedRedditAccessToken: renewedRedditAccessToken);

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.GetAuthenticatedUserAsync(It.IsAny<IRedditAccessToken>()), Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithExpiredRedditAccessTokenAndRenewedAccessTokenIsNotNull_AssertRedditAccessTokenSetterWasCalledOnDashboardSettings()
        {
            DateTime expires = DateTime.Now.AddSeconds(_random.Next(60, 300) * -1);
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken(expires: expires);
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock(redditAccessToken: redditAccessToken);
            IDashboard dashboard = CreateDashboard();

            IRedditAccessToken renewedRedditAccessToken = CreateRedditAccessToken();
            IDashboardRedditContentBuilder sut = CreateSut(renewedRedditAccessToken: renewedRedditAccessToken);

            await sut.BuildAsync(dashboardSettingsMock.Object, dashboard);

            dashboardSettingsMock.VerifySet(m => m.RedditAccessToken = It.Is<IRedditAccessToken>(value => value == renewedRedditAccessToken), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWithExpiredRedditAccessTokenAndRenewedAccessTokenIsNotNull_AssertGetAuthenticatedUserAsyncWasCalledOnRedditLogic()
        {
            DateTime expires = DateTime.Now.AddSeconds(_random.Next(60, 300) * -1);
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken(expires: expires);
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken);
            IDashboard dashboard = CreateDashboard();

            IRedditAccessToken renewedRedditAccessToken = CreateRedditAccessToken();
            IDashboardRedditContentBuilder sut = CreateSut(renewedRedditAccessToken: renewedRedditAccessToken);

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.GetAuthenticatedUserAsync(It.Is<IRedditAccessToken>(value => value == renewedRedditAccessToken)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndDoesNotHaveAuthenticatedUser_AssertReplaceWasNotCalledOnDashbaordWithAuthenticatedUser()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: false);

            await sut.BuildAsync(dashboardSettings, dashboardMock.Object);

            dashboardMock.Verify(m => m.Replace(It.IsAny<IRedditAuthenticatedUser>()), Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndDoesNotHaveAuthenticatedUser_AssertIncludeNsfwContentWasNotCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: false);

            await sut.BuildAsync(dashboardSettingsMock.Object, dashboard);

            dashboardSettingsMock.Verify(m => m.IncludeNsfwContent, Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndDoesNotHaveAuthenticatedUser_AssertOnlyNsfwContentWasNotCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: false);

            await sut.BuildAsync(dashboardSettingsMock.Object, dashboard);

            dashboardSettingsMock.Verify(m => m.OnlyNsfwContent, Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndDoesNotHaveAuthenticatedUser_AssertGetSubredditsForAuthenticatedUserAsyncWasNotCalledOnRedditLogic()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: false);

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.GetSubredditsForAuthenticatedUserAsync(
                    It.IsAny<IRedditAccessToken>(), 
                    It.IsAny<bool>(), 
                    It.IsAny<bool>()), 
                Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndDoesNotHaveAuthenticatedUser_AssertGetNsfwSubredditsAsyncWasNotCalledOnRedditLogic()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: false);

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.GetNsfwSubredditsAsync(
                    It.IsAny<IRedditAccessToken>(), 
                    It.IsAny<int>()), 
                Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndDoesNotHaveAuthenticatedUser_AssertReplaceWasNotCalledOnDashboardWithSubredditCollection()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: false);

            await sut.BuildAsync(dashboardSettings, dashboardMock.Object);

            dashboardMock.Verify(m => m.Replace(It.IsAny<IEnumerable<IRedditSubreddit>>()), Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndDoesNotHaveAuthenticatedUser_AssertGetLinksAsyncWasNotCalledOnRedditLogic()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: false);

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.GetLinksAsync(
                    It.IsAny<IRedditAccessToken>(), 
                    It.IsAny<IEnumerable<IRedditSubreddit>>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()), 
                Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndDoesNotHaveAuthenticatedUser_AssertReplaceWasNotCalledOnDashboardWithLinkCollection()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: false);

            await sut.BuildAsync(dashboardSettings, dashboardMock.Object);

            dashboardMock.Verify(m => m.Replace(It.IsAny<IEnumerable<IRedditLink>>()), Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUser_AssertReplaceWasCalledOnDashbaordWithAuthenticatedUser()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser);

            await sut.BuildAsync(dashboardSettings, dashboardMock.Object);

            dashboardMock.Verify(m => m.Replace(It.Is<IRedditAuthenticatedUser>(value => value == redditAuthenticatedUser)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUser_AssertOver18WasCalledOnRedditAuthenticatedUser()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IDashboard dashboard = CreateDashboard();

            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = CreateRedditAuthenticatedUserMock();
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUserMock.Object);

            await sut.BuildAsync(dashboardSettings, dashboard);

            redditAuthenticatedUserMock.Verify(m => m.Over18, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUser_AssertIncludeNsfwContentWasCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true);

            await sut.BuildAsync(dashboardSettingsMock.Object, dashboard);

            dashboardSettingsMock.Verify(m => m.IncludeNsfwContent, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUser_AssertOnlyNsfwContentWasCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true);

            await sut.BuildAsync(dashboardSettingsMock.Object, dashboard);

            dashboardSettingsMock.Verify(m => m.OnlyNsfwContent, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUserNotOver18_AssertGetSubredditsForAuthenticatedUserAsyncWasCalledOnRedditLogic()
        {
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);
            IDashboard dashboard = CreateDashboard();

            const bool over18 = false;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser);

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.GetSubredditsForAuthenticatedUserAsync(
                    It.Is<IRedditAccessToken>(value => value == redditAccessToken), 
                    It.Is<bool>(value => value == false), 
                    It.Is<bool>(value => value == false)), 
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUserOver18WithIncludeNsfwContent_AssertGetSubredditsForAuthenticatedUserAsyncWasCalledOnRedditLogic()
        {
            const bool includeNsfwContent = true;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);
            IDashboard dashboard = CreateDashboard();

            const bool over18 = true;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser);

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.GetSubredditsForAuthenticatedUserAsync(
                    It.Is<IRedditAccessToken>(value => value == redditAccessToken), 
                    It.Is<bool>(value => value == true), 
                    It.Is<bool>(value => value == onlyNsfwContent)), 
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUserOver18WithoutIncludeNsfwContent_AssertGetSubredditsForAuthenticatedUserAsyncWasCalledOnRedditLogic()
        {
            const bool includeNsfwContent = false;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);
            IDashboard dashboard = CreateDashboard();

            const bool over18 = true;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser);

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.GetSubredditsForAuthenticatedUserAsync(
                    It.Is<IRedditAccessToken>(value => value == redditAccessToken), 
                    It.Is<bool>(value => value == false), 
                    It.Is<bool>(value => value == onlyNsfwContent)), 
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUserOver18WithOnlyNsfwContent_AssertGetSubredditsForAuthenticatedUserAsyncWasCalledOnRedditLogic()
        {
            bool includeNsfwContent = _random.Next(1, 100) > 50;;
            const bool onlyNsfwContent = true;
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);
            IDashboard dashboard = CreateDashboard();

            const bool over18 = true;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser);

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.GetSubredditsForAuthenticatedUserAsync(
                    It.Is<IRedditAccessToken>(value => value == redditAccessToken), 
                    It.Is<bool>(value => value == includeNsfwContent), 
                    It.Is<bool>(value => value == true)), 
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUserOver18WithoutOnlyNsfwContent_AssertGetSubredditsForAuthenticatedUserAsyncWasCalledOnRedditLogic()
        {
            bool includeNsfwContent = _random.Next(1, 100) > 50;;
            const bool onlyNsfwContent = false;
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);
            IDashboard dashboard = CreateDashboard();

            const bool over18 = true;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser);

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.GetSubredditsForAuthenticatedUserAsync(
                    It.Is<IRedditAccessToken>(value => value == redditAccessToken), 
                    It.Is<bool>(value => value == includeNsfwContent), 
                    It.Is<bool>(value => value == false)), 
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUserNotOver18_AssertGetNsfwSubredditsAsyncWasNotCalledOnRedditLogic()
        {
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);
            IDashboard dashboard = CreateDashboard();

            const bool over18 = false;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser);

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.GetNsfwSubredditsAsync(
                    It.IsAny<IRedditAccessToken>(), 
                    It.IsAny<int>()), 
                Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUserOver18WithoutIncludeNsfwContentAndWithoutOnlyNsfwContent_AssertGetNsfwSubredditsAsyncWasNotCalledOnRedditLogic()
        {
            const bool includeNsfwContent = false;
            const bool onlyNsfwContent = false;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);
            IDashboard dashboard = CreateDashboard();

            const bool over18 = true;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser);

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.GetNsfwSubredditsAsync(
                    It.IsAny<IRedditAccessToken>(), 
                    It.IsAny<int>()), 
                Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUserOver18WithIncludeNsfwContentAndWithoutOnlyNsfwContent_AssertGetNsfwSubredditsAsyncWasCalledOnRedditLogic()
        {
            const bool includeNsfwContent = true;
            const bool onlyNsfwContent = false;
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);
            IDashboard dashboard = CreateDashboard();

            const bool over18 = true;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser);

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.GetNsfwSubredditsAsync(
                    It.Is<IRedditAccessToken>(value => value == redditAccessToken), 
                    It.Is<int>(value => value == 4)), 
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUserOver18WithoutIncludeNsfwContentAndWithOnlyNsfwContent_AssertGetNsfwSubredditsAsyncWasCalledOnRedditLogic()
        {
            const bool includeNsfwContent = false;
            const bool onlyNsfwContent = true;
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);
            IDashboard dashboard = CreateDashboard();

            const bool over18 = true;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser);

            await sut.BuildAsync(dashboardSettings, dashboard);

            _redditLogicMock.Verify(m => m.GetNsfwSubredditsAsync(
                    It.Is<IRedditAccessToken>(value => value == redditAccessToken), 
                    It.Is<int>(value => value == 4)), 
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUser_AssertReplaceWasCalledOnDashboardWithSubredditCollection()
        {
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            bool over18 = _random.Next(1, 100) > 50;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);

            IRedditSubreddit subreddit1ForAuthenticatedUser = CreateSubreddit();
            IRedditSubreddit subreddit2ForAuthenticatedUser = CreateSubreddit();
            IRedditSubreddit subreddit3ForAuthenticatedUser = CreateSubreddit();
            IEnumerable<IRedditSubreddit> subredditsForAuthenticatedUser = CreateSubredditCollection(subreddit1ForAuthenticatedUser, subreddit2ForAuthenticatedUser, subreddit3ForAuthenticatedUser);
            IRedditSubreddit nsfwSubreddit1 = CreateSubreddit();
            IRedditSubreddit nsfwSubReddit2 = CreateSubreddit();
            IRedditSubreddit nsfwSubReddit3 = CreateSubreddit();
            IEnumerable<IRedditSubreddit> nsfwSubreddits = CreateSubredditCollection(nsfwSubreddit1, nsfwSubReddit2, nsfwSubReddit3);
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser, subredditsForAuthenticatedUser: subredditsForAuthenticatedUser, nsfwSubreddits: nsfwSubreddits);

            await sut.BuildAsync(dashboardSettings, dashboardMock.Object);

            List<IRedditSubreddit> expectedSubreddits = new List<IRedditSubreddit>(subredditsForAuthenticatedUser);
            if (over18 && (includeNsfwContent || onlyNsfwContent))
            {
                expectedSubreddits.AddRange(nsfwSubreddits);
            }

            dashboardMock.Verify(m => m.Replace(It.Is<IEnumerable<IRedditSubreddit>>(value => 
                    value != null &&
                    value.Count() == expectedSubreddits.Count() &&
                    expectedSubreddits.All(subreddit => value.Contains(subreddit)))), 
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUserNotOver18_AssertGetLinksAsyncForAuthenticatedUserAsyncWasCalledOnRedditLogic()
        {
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);
            IDashboard dashboard = CreateDashboard();

            const bool over18 = false;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);
            IEnumerable<IRedditSubreddit> subredditsForAuthenticatedUser = CreateSubredditCollection(CreateSubreddit(), CreateSubreddit(), CreateSubreddit());
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser, subredditsForAuthenticatedUser: subredditsForAuthenticatedUser);

            await sut.BuildAsync(dashboardSettings, dashboard);

            List<IRedditSubreddit> expectedSubreddits = new List<IRedditSubreddit>(subredditsForAuthenticatedUser);

            _redditLogicMock.Verify(m => m.GetLinksAsync(
                    It.Is<IRedditAccessToken>(value => value == redditAccessToken),
                    It.Is<IEnumerable<IRedditSubreddit>>(value => value != null && value.Count() == expectedSubreddits.Count() && expectedSubreddits.All(subreddit => value.Contains(subreddit))),
                    It.Is<bool>(value => value == false), 
                    It.Is<bool>(value => value == false)), 
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUserOver18WithIncludeNsfwContent_AssertGetLinksAsyncWasCalledOnRedditLogic()
        {
            const bool includeNsfwContent = true;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);
            IDashboard dashboard = CreateDashboard();

            const bool over18 = true;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);
            IEnumerable<IRedditSubreddit> subredditsForAuthenticatedUser = CreateSubredditCollection(CreateSubreddit(), CreateSubreddit(), CreateSubreddit());
            IEnumerable<IRedditSubreddit> nsfwSubreddits = CreateSubredditCollection(CreateSubreddit(), CreateSubreddit(), CreateSubreddit());
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser, subredditsForAuthenticatedUser: subredditsForAuthenticatedUser, nsfwSubreddits: nsfwSubreddits);

            await sut.BuildAsync(dashboardSettings, dashboard);

            List<IRedditSubreddit> expectedSubreddits = new List<IRedditSubreddit>(subredditsForAuthenticatedUser);
            expectedSubreddits.AddRange(nsfwSubreddits);

            _redditLogicMock.Verify(m => m.GetLinksAsync(
                    It.Is<IRedditAccessToken>(value => value == redditAccessToken),
                    It.Is<IEnumerable<IRedditSubreddit>>(value => value != null && value.Count() == expectedSubreddits.Count() && expectedSubreddits.All(subreddit => value.Contains(subreddit))),
                    It.Is<bool>(value => value == true), 
                    It.Is<bool>(value => value == onlyNsfwContent)), 
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUserOver18WithoutIncludeNsfwContent_AssertGetLinksAsyncWasCalledOnRedditLogic()
        {
            const bool includeNsfwContent = false;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);
            IDashboard dashboard = CreateDashboard();

            const bool over18 = true;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);
            IEnumerable<IRedditSubreddit> subredditsForAuthenticatedUser = CreateSubredditCollection(CreateSubreddit(), CreateSubreddit(), CreateSubreddit());
            IEnumerable<IRedditSubreddit> nsfwSubreddits = CreateSubredditCollection(CreateSubreddit(), CreateSubreddit(), CreateSubreddit());
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser, subredditsForAuthenticatedUser: subredditsForAuthenticatedUser, nsfwSubreddits: nsfwSubreddits);

            await sut.BuildAsync(dashboardSettings, dashboard);

            List<IRedditSubreddit> expectedSubreddits = new List<IRedditSubreddit>(subredditsForAuthenticatedUser);
            if (onlyNsfwContent)
            {
                expectedSubreddits.AddRange(nsfwSubreddits);
            }

            _redditLogicMock.Verify(m => m.GetLinksAsync(
                    It.Is<IRedditAccessToken>(value => value == redditAccessToken),
                    It.Is<IEnumerable<IRedditSubreddit>>(value => value != null && value.Count() == expectedSubreddits.Count() && expectedSubreddits.All(subreddit => value.Contains(subreddit))),
                    It.Is<bool>(value => value == false), 
                    It.Is<bool>(value => value == onlyNsfwContent)), 
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUserOver18WithOnlyNsfwContent_AssertGetLinksAsyncWasCalledOnRedditLogic()
        {
            bool includeNsfwContent = _random.Next(1, 100) > 50;;
            const bool onlyNsfwContent = true;
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);
            IDashboard dashboard = CreateDashboard();

            const bool over18 = true;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);
            IEnumerable<IRedditSubreddit> subredditsForAuthenticatedUser = CreateSubredditCollection(CreateSubreddit(), CreateSubreddit(), CreateSubreddit());
            IEnumerable<IRedditSubreddit> nsfwSubreddits = CreateSubredditCollection(CreateSubreddit(), CreateSubreddit(), CreateSubreddit());
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser, subredditsForAuthenticatedUser: subredditsForAuthenticatedUser, nsfwSubreddits: nsfwSubreddits);

            await sut.BuildAsync(dashboardSettings, dashboard);

            List<IRedditSubreddit> expectedSubreddits = new List<IRedditSubreddit>(subredditsForAuthenticatedUser);
            expectedSubreddits.AddRange(nsfwSubreddits);

            _redditLogicMock.Verify(m => m.GetLinksAsync(
                    It.Is<IRedditAccessToken>(value => value == redditAccessToken),
                    It.Is<IEnumerable<IRedditSubreddit>>(value => value != null && value.Count() == expectedSubreddits.Count() && expectedSubreddits.All(subreddit => value.Contains(subreddit))),
                    It.Is<bool>(value => value == includeNsfwContent), 
                    It.Is<bool>(value => value == true)), 
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUserOver18WithoutOnlyNsfwContent_AssertGetLinksAsyncWasCalledOnRedditLogic()
        {
            bool includeNsfwContent = _random.Next(1, 100) > 50;;
            const bool onlyNsfwContent = false;
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);
            IDashboard dashboard = CreateDashboard();

            const bool over18 = true;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);
            IEnumerable<IRedditSubreddit> subredditsForAuthenticatedUser = CreateSubredditCollection(CreateSubreddit(), CreateSubreddit(), CreateSubreddit());
            IEnumerable<IRedditSubreddit> nsfwSubreddits = CreateSubredditCollection(CreateSubreddit(), CreateSubreddit(), CreateSubreddit());
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser, subredditsForAuthenticatedUser: subredditsForAuthenticatedUser, nsfwSubreddits: nsfwSubreddits);

            await sut.BuildAsync(dashboardSettings, dashboard);

            List<IRedditSubreddit> expectedSubreddits = new List<IRedditSubreddit>(subredditsForAuthenticatedUser);
            if (includeNsfwContent)
            {
                expectedSubreddits.AddRange(nsfwSubreddits);
            }

            _redditLogicMock.Verify(m => m.GetLinksAsync(
                    It.Is<IRedditAccessToken>(value => value == redditAccessToken),
                    It.Is<IEnumerable<IRedditSubreddit>>(value => value != null && value.Count() == expectedSubreddits.Count() && expectedSubreddits.All(subreddit => value.Contains(subreddit))),
                    It.Is<bool>(value => value == includeNsfwContent), 
                    It.Is<bool>(value => value == false)), 
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndHaveAuthenticatedUser_AssertReplaceWasCalledOnDashboardWithLinkCollection()
        {
            bool includeNsfwContent = _random.Next(1, 100) > 50;
            bool onlyNsfwContent = _random.Next(1, 100) > 50;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);
            Mock<IDashboard> dashboardMock = CreateDashboardMock();

            bool over18 = _random.Next(1, 100) > 50;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);

            IEnumerable<IRedditLink> links = CreateLinkCollection(CreateLink(), CreateLink(), CreateLink(), CreateLink(), CreateLink());
            IDashboardRedditContentBuilder sut = CreateSut(hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser, links: links);

            await sut.BuildAsync(dashboardSettings, dashboardMock.Object);

            dashboardMock.Verify(m => m.Replace(It.Is<IEnumerable<IRedditLink>>(value =>
                    value != null &&
                    value.Count() == links.Count() &&
                    links.All(link => value.Contains(link)))), 
                Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IDashboard dashboard = CreateDashboard();

            IDashboardRedditContentBuilder sut = CreateSut();

            await sut.BuildAsync(dashboardSettings, dashboard);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            IDashboardSettings dashboardSettings = CreateDashboardSettings();
            IDashboard dashboard = CreateDashboard();

            Exception exception = new Exception();
            IDashboardRedditContentBuilder sut = CreateSut(exception: exception);

            await sut.BuildAsync(dashboardSettings, dashboard);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(value => value == exception)), Times.Once);
        }

        private IDashboardRedditContentBuilder CreateSut(bool hasRedditAuthenticatedUser = true, IRedditAuthenticatedUser redditAuthenticatedUser = null, IEnumerable<IRedditSubreddit> subredditsForAuthenticatedUser = null, IEnumerable<IRedditSubreddit> nsfwSubreddits = null, IEnumerable<IRedditLink> links = null, IRedditAccessToken renewedRedditAccessToken = null, Exception exception = null)
        {
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
            _redditLogicMock.Setup(m => m.GetSubredditsForAuthenticatedUserAsync(It.IsAny<IRedditAccessToken>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.Run<IEnumerable<IRedditSubreddit>>(() => subredditsForAuthenticatedUser ?? CreateSubredditCollection(CreateSubreddit(), CreateSubreddit(), CreateSubreddit())));
            _redditLogicMock.Setup(m => m.GetNsfwSubredditsAsync(It.IsAny<IRedditAccessToken>(), It.IsAny<int>()))
                .Returns(Task.Run<IEnumerable<IRedditSubreddit>>(() => nsfwSubreddits ?? CreateSubredditCollection(CreateSubreddit(), CreateSubreddit(), CreateSubreddit())));
            _redditLogicMock.Setup(m => m.GetLinksAsync(It.IsAny<IRedditAccessToken>(), It.IsAny<IEnumerable<IRedditSubreddit>>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.Run<IEnumerable<IRedditLink>>(() => links ?? CreateLinkCollection(CreateLink(), CreateLink(), CreateLink(), CreateLink(), CreateLink())));
            _redditLogicMock.Setup(m => m.RenewAccessTokenAsync(It.IsAny<IRedditAccessToken>()))
                .Returns(Task.Run<IRedditAccessToken>(() => renewedRedditAccessToken));

            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.Run(() => { }));

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

        private IEnumerable<IRedditSubreddit> CreateSubredditCollection(params IRedditSubreddit[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            return new List<IRedditSubreddit>(args);
        }
 
        private IRedditSubreddit CreateSubreddit()
        {
            return CreateSubredditMock().Object;
        }

        private Mock<IRedditSubreddit> CreateSubredditMock()
        {
            Mock<IRedditSubreddit> subredditMock = new Mock<IRedditSubreddit>();
            subredditMock.Setup(m => m.Subscribers)
                .Returns(_random.Next(2500, 10000));
            return subredditMock;
        }

        private IEnumerable<IRedditLink> CreateLinkCollection(params IRedditLink[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            return new List<IRedditLink>(args);
        }

        private IRedditLink CreateLink()
        {
            return CreateLinkMock().Object;
        }

        private Mock<IRedditLink> CreateLinkMock()
        {
            Mock<IRedditLink> linkMock = new Mock<IRedditLink>();
            return linkMock;
        }
    }
}