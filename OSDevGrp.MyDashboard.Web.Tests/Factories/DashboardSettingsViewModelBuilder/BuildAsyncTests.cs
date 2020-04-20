using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.DashboardSettingsViewModelBuilder
{
    [TestClass]
    public class BuildAsyncTests
    {
        #region Private variables

        private Mock<ICookieHelper> _cookieHelperMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _cookieHelperMock = new Mock<ICookieHelper>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullExceptionAttribute("input")]
        public async Task BuildAsync_WhenDashboardSettingsIsNull_ThrowsArgumentNullException()
        {
            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            await sut.BuildAsync(null);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertNumberOfNewsWasCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            await sut.BuildAsync(dashboardSettingsMock.Object);

            dashboardSettingsMock.Verify(m => m.NumberOfNews, Times.Once());
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertUseRedditWasCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            await sut.BuildAsync(dashboardSettingsMock.Object);

            dashboardSettingsMock.Verify(m => m.UseReddit, Times.Once());
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertRedditAccessTokenWasCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            await sut.BuildAsync(dashboardSettingsMock.Object);

            dashboardSettingsMock.Verify(m => m.RedditAccessToken, Times.Once());
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereRedditAccessTokenIsNotNull_AssertToBase64WasCalledOnRedditAccessToken()
        {
            Mock<IRedditAccessToken> redditAccessTokenMock = CreateRedditAccessTokenMock();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessTokenMock.Object);

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            await sut.BuildAsync(dashboardSettings);

            redditAccessTokenMock.Verify(m => m.ToBase64(), Times.Once());
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertIncludeNsfwContentWasCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            await sut.BuildAsync(dashboardSettingsMock.Object);

            dashboardSettingsMock.Verify(m => m.IncludeNsfwContent, Times.Once());
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertOnlyNsfwContentWasCalledOnDashboardSettings()
        {
            Mock<IDashboardSettings> dashboardSettingsMock = CreateDashboardSettingsMock();

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            await sut.BuildAsync(dashboardSettingsMock.Object);

            dashboardSettingsMock.Verify(m => m.OnlyNsfwContent, Times.Once());
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertToCookieWasCalledOnCookieHelperWithDashboardSettingsViewModel()
        {
            int numberOfNews = _random.Next(25, 50);
            bool useReddit = _random.Next(100) > 50;
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(numberOfNews, useReddit, includeNsfwContent, onlyNsfwContent, redditAccessToken);

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            DashboardSettingsViewModel result = await sut.BuildAsync(dashboardSettings);

            _cookieHelperMock.Verify(m => m.ToCookie(It.Is<DashboardSettingsViewModel>(value => value == result)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_ReturnsInitializedDashboardSettingsViewModel()
        {
            int numberOfNews = _random.Next(25, 50);
            bool useReddit = _random.Next(100) > 50;
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();
            IDashboardSettings dashboardSettings = CreateDashboardSettings(numberOfNews, useReddit, includeNsfwContent, onlyNsfwContent, redditAccessToken);

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            DashboardSettingsViewModel result = await sut.BuildAsync(dashboardSettings);

            Assert.IsNotNull(result);
            Assert.AreEqual(numberOfNews, result.NumberOfNews);
            Assert.AreEqual(useReddit, result.UseReddit);
            Assert.IsFalse(result.AllowNsfwContent);
            if (includeNsfwContent)
            {
                Assert.IsTrue(result.IncludeNsfwContent.HasValue);
                Assert.IsTrue(result.IncludeNsfwContent.Value);
            }
            else
            {
                Assert.IsFalse(result.IncludeNsfwContent.HasValue);
                Assert.IsNull(result.IncludeNsfwContent);
            }
            if (onlyNsfwContent)
            {
                Assert.IsTrue(result.OnlyNsfwContent.HasValue);
                Assert.IsTrue(result.OnlyNsfwContent.Value);
            }
            else
            {
                Assert.IsFalse(result.OnlyNsfwContent.HasValue);
                Assert.IsNull(result.OnlyNsfwContent);
            }
            Assert.IsNotNull(result.RedditAccessToken);
            Assert.IsFalse(result.ExportData);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereRedditAccessTokenIsNull_ReturnsInitializedDashboardSettingsViewModel()
        {
            const IRedditAccessToken redditAccessToken = null;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken);

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            DashboardSettingsViewModel result = await sut.BuildAsync(dashboardSettings);

            Assert.IsNotNull(result);
            Assert.IsNull(result.RedditAccessToken);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereRedditAccessTokenIsNotNull_ReturnsInitializedDashboardSettingsViewModel()
        {
            string redditAccessTokenAsBase64 = Guid.NewGuid().ToString("D");
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken(redditAccessTokenAsBase64);
            IDashboardSettings dashboardSettings = CreateDashboardSettings(redditAccessToken: redditAccessToken);

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            DashboardSettingsViewModel result = await sut.BuildAsync(dashboardSettings);

            Assert.IsNotNull(result);
            Assert.AreEqual(redditAccessTokenAsBase64, result.RedditAccessToken);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereIncludeNsfwContentIsFalse_ReturnsInitializedDashboardSettingsViewModel()
        {
            const bool includeNsfwContent = false;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(includeNsfwContent: includeNsfwContent);

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            DashboardSettingsViewModel result = await sut.BuildAsync(dashboardSettings);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IncludeNsfwContent.HasValue);
            Assert.IsNull(result.IncludeNsfwContent);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereIncludeNsfwContentIsTrue_ReturnsInitializedDashboardSettingsViewModel()
        {
            const bool includeNsfwContent = true;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(includeNsfwContent: includeNsfwContent);

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            DashboardSettingsViewModel result = await sut.BuildAsync(dashboardSettings);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IncludeNsfwContent.HasValue);
            Assert.IsTrue(result.IncludeNsfwContent.Value);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereOnlyNsfwContentIsFalse_ReturnsInitializedDashboardSettingsViewModel()
        {
            const bool onlyNsfwContent = false;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(onlyNsfwContent: onlyNsfwContent);

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            DashboardSettingsViewModel result = await sut.BuildAsync(dashboardSettings);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.OnlyNsfwContent.HasValue);
            Assert.IsNull(result.OnlyNsfwContent);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalledWhereOnlyNsfwContentIsTrue_ReturnsInitializedDashboardSettingsViewModel()
        {
            const bool onlyNsfwContent = true;
            IDashboardSettings dashboardSettings = CreateDashboardSettings(onlyNsfwContent: onlyNsfwContent);

            IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> sut = CreateSut();

            DashboardSettingsViewModel result = await sut.BuildAsync(dashboardSettings);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.OnlyNsfwContent.HasValue);
            Assert.IsTrue(result.OnlyNsfwContent.Value);
        }

        private IViewModelBuilder<DashboardSettingsViewModel, IDashboardSettings> CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Factories.DashboardSettingsViewModelBuilder(_cookieHelperMock.Object);
        }

        private IDashboardSettings CreateDashboardSettings(int? numberOfNews = null, bool? useReddit = null, bool? includeNsfwContent = null, bool? onlyNsfwContent = null, IRedditAccessToken redditAccessToken = null)
        {
            return CreateDashboardSettingsMock(numberOfNews, useReddit, includeNsfwContent, onlyNsfwContent, redditAccessToken).Object;
        }

        private Mock<IDashboardSettings> CreateDashboardSettingsMock(int? numberOfNews = null, bool? useReddit = null, bool? includeNsfwContent = null, bool? onlyNsfwContent = null, IRedditAccessToken redditAccessToken = null)
        {
            Mock<IDashboardSettings> dashboardSettingsMock = new Mock<IDashboardSettings>();
            dashboardSettingsMock.Setup(m => m.NumberOfNews)
                .Returns(numberOfNews ?? _random.Next(25, 50));
            dashboardSettingsMock.Setup(m => m.UseReddit)
                .Returns(useReddit ?? _random.Next(100) > 50);
            dashboardSettingsMock.Setup(m => m.IncludeNsfwContent)
                .Returns(includeNsfwContent ?? _random.Next(100) > 50);
            dashboardSettingsMock.Setup(m => m.OnlyNsfwContent)
                .Returns(onlyNsfwContent ?? _random.Next(100) > 50);
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