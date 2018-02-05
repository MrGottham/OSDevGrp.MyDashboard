using System;
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
    public class ShouldBuildTests
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
        public void ShouldBuild_WhenDashboardSettingsIsNull_ThrowsArgumentNullException()
        {
            IDashboardRedditContentBuilder sut = CreateSut();

            sut.ShouldBuild(null);
        }

        [TestMethod]
        public void ShouldBuild_WhenCalled_AssertUseRedditWasCalledOnDashboardSettings()
        {
            bool useReddit = _random.Next(100) > 50;
            bool hasRedditAccessToken = _random.Next(100) > 50;
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock(useReddit: useReddit, hasRedditAccessToken: hasRedditAccessToken);

            IDashboardRedditContentBuilder sut = CreateSut();

            bool result = sut.ShouldBuild(dashboardSettingsMock.Object);

            dashboardSettingsMock.Verify(m => m.UseReddit, Times.Once);
        }

        [TestMethod]
        public void ShouldBuild_WhenCalled_AssertRedditAccessTokenWasCalledOnDashboardSettings()
        {
            const bool useReddit = true;
            bool hasRedditAccessToken = _random.Next(100) > 50;
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock(useReddit: useReddit, hasRedditAccessToken: hasRedditAccessToken);

            IDashboardRedditContentBuilder sut = CreateSut();

            bool result = sut.ShouldBuild(dashboardSettingsMock.Object);

            dashboardSettingsMock.Verify(m => m.RedditAccessToken, Times.Once);
        }

        [TestMethod]
        public void ShouldBuild_WhenUseRedditIsFalse_ExpectFalse()
        {
            const bool useReddit = false;
            bool hasRedditAccessToken = _random.Next(100) > 50;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(useReddit: useReddit, hasRedditAccessToken: hasRedditAccessToken);

            IDashboardRedditContentBuilder sut = CreateSut();

            bool result = sut.ShouldBuild(dashboardSettings);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldBuild_WhenUseRedditIsTrueAndNoRedditAccessToken_ExpectFalse()
        {
            const bool useReddit = true;
            const bool hasRedditAccessToken = false;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(useReddit: useReddit, hasRedditAccessToken: hasRedditAccessToken);

            IDashboardRedditContentBuilder sut = CreateSut();

            bool result = sut.ShouldBuild(dashboardSettings);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldBuild_WhenUseRedditIsTrueAndRedditAccessToken_ExpectTrue()
        {
            const bool useReddit = true;
            const bool hasRedditAccessToken = true;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(useReddit: useReddit, hasRedditAccessToken: hasRedditAccessToken);

            IDashboardRedditContentBuilder sut = CreateSut();

            bool result = sut.ShouldBuild(dashboardSettings);

            Assert.IsTrue(result);
        }

        private IDashboardRedditContentBuilder CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Factories.DashboardRedditContentBuilder(
                _redditLogicMock.Object,
                _exceptionHandlerMock.Object
            );
        }

        private IDashboardSettings CreateDashboardSettings(bool? useReddit = null, bool hasRedditAccessToken = false)
        {
            return CreateDashboardSettingsMock(useReddit, hasRedditAccessToken).Object;
        }

        private Mock<IDashboardSettings> CreateDashboardSettingsMock(bool? useReddit = null, bool hasRedditAccessToken = false)
        {
            Mock<IDashboardSettings> dashboardSettingsMock = new Mock<IDashboardSettings>();
            dashboardSettingsMock.Setup(m => m.UseReddit)
                .Returns(useReddit ?? _random.Next(100) > 50);
            dashboardSettingsMock.Setup(m => m.RedditAccessToken)
                .Returns(hasRedditAccessToken ? CreateRedditAccessToken() : null);
            return dashboardSettingsMock;
        }

        private IRedditAccessToken CreateRedditAccessToken()
        {
            return CreateRedditAccessTokenMock().Object;
        }

        private Mock<IRedditAccessToken> CreateRedditAccessTokenMock()
        {
            Mock<IRedditAccessToken> redditAccessTokenMock = new Mock<IRedditAccessToken>();
            return redditAccessTokenMock;
        }
    }
}