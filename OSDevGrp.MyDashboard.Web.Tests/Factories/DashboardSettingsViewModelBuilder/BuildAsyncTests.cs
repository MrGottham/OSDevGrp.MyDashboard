using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.DashboardSettingsViewModelBuilder
{
    [TestClass]
    public class BuildAsyncTests
    {
        #region Private variables

        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullExceptionAttribute("input")]
        public void BuildAsync_WhenDashboardSettingsIsNull_ThrowsArgumentNullException()
        {
            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            sut.BuildAsync(null);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertNumberOfNewsWasCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            Task<DashboardSettingsViewModel> buildTask = sut.BuildAsync(dashboardSettingsMock.Object);
            buildTask.Wait();

            dashboardSettingsMock.Verify(m => m.NumberOfNews, Times.Once());
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertUseRedditWasCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            Task<DashboardSettingsViewModel> buildTask = sut.BuildAsync(dashboardSettingsMock.Object);
            buildTask.Wait();

            dashboardSettingsMock.Verify(m => m.UseReddit, Times.Once());
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertRedditAccessTokenWasCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            Task<DashboardSettingsViewModel> buildTask = sut.BuildAsync(dashboardSettingsMock.Object);
            buildTask.Wait();

            dashboardSettingsMock.Verify(m => m.RedditAccessToken, Times.Once());
        }

        [TestMethod]
        public void BuildAsync_WhenCalledWhereRedditAccessTokenIsNotNull_AssertToBase64WasCalledOnRedditAccessToken()
        {
            Mock<IRedditAccessToken> redditAccessTokenMock = CreateRedditAccessTokenMock();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessTokenMock.Object);

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            Task<DashboardSettingsViewModel> buildTask = sut.BuildAsync(dashboardSettings);
            buildTask.Wait();

            redditAccessTokenMock.Verify(m => m.ToBase64(), Times.Once());
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_ReturnsInitializedDashboardSettingsViewModel()
        {
            int numberOfNews = _random.Next(25, 50);
            bool useReddit = _random.Next(100) > 50;
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(numberOfNews, useReddit, redditAccessToken);

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            Task<DashboardSettingsViewModel> buildTask = sut.BuildAsync(dashboardSettings);
            buildTask.Wait();

            DashboardSettingsViewModel result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(numberOfNews, result.NumberOfNews);
            Assert.AreEqual(useReddit, result.UseReddit);
            Assert.IsNotNull(result.RedditAccessToken);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledWhereRedditAccessTokenIsNull_ReturnsInitializedDashboardSettingsViewModel()
        {
            const IRedditAccessToken redditAccessToken = null;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken);

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            Task<DashboardSettingsViewModel> buildTask = sut.BuildAsync(dashboardSettings);
            buildTask.Wait();

            DashboardSettingsViewModel result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.IsNull(result.RedditAccessToken);
        }

        [TestMethod]
        public void BuildAsync_WhenCalledWhereRedditAccessTokenIsNotNull_ReturnsInitializedDashboardSettingsViewModel()
        {
            string redditAccessTokenAsBase64 = Guid.NewGuid().ToString("D");
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken(redditAccessTokenAsBase64);
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken);

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            Task<DashboardSettingsViewModel> buildTask = sut.BuildAsync(dashboardSettings);
            buildTask.Wait();

            DashboardSettingsViewModel result = buildTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(redditAccessTokenAsBase64, result.RedditAccessToken);
        }

        private IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Factories.DashboardSettingsViewModelBuilder();
        }

        private IDashboardSettings CreateDashboardSettings(int? numberOfNews = null, bool? useReddit = null, IRedditAccessToken redditAccessToken = null)
        {
            return CreateDashboardSettingsMock(numberOfNews, useReddit, redditAccessToken).Object;
        }

        private Mock<IDashboardSettings> CreateDashboardSettingsMock(int? numberOfNews = null, bool? useReddit = null, IRedditAccessToken redditAccessToken = null)
        {
            Mock<IDashboardSettings> dashboardSettingsMock = new Mock<IDashboardSettings>();
            dashboardSettingsMock.Setup(m => m.NumberOfNews)
                .Returns(numberOfNews ?? _random.Next(25, 50));
            dashboardSettingsMock.Setup(m => m.UseReddit)
                .Returns(useReddit ?? _random.Next(100) > 50);
            dashboardSettingsMock.Setup(m => m.RedditAccessToken)
                .Returns(redditAccessToken);
            return dashboardSettingsMock;
        }

        private IRedditAccessToken CreateRedditAccessToken(string redditAccessTokenAsBase64 = null)
        {
            return CreateRedditAccessTokenMock(redditAccessTokenAsBase64).Object;
        }

        private Mock<IRedditAccessToken> CreateRedditAccessTokenMock(string redditAccessTokenAsBase64 = null)
        {
            Mock<IRedditAccessToken> redditAccessTokenMock = new Mock<IRedditAccessToken>();
            redditAccessTokenMock.Setup(m => m.ToBase64())
                .Returns(redditAccessTokenAsBase64 ?? Guid.NewGuid().ToString("D"));
            return redditAccessTokenMock;
        }
    }
}