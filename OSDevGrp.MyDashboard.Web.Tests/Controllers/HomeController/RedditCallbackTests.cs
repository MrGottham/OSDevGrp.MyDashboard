using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;
using System;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Web.Tests.Controllers.HomeController
{
    [TestClass]
    public class RedditCallbackTests : TestBase
    {
        #region Private variables

        private Mock<IDashboardFactory> _dashboardFactoryMock;
        private Mock<IViewModelBuilder<DashboardViewModel, IDashboard>> _dashboardViewModelBuilderMock;
        private Mock<IModelExporter<DashboardExportModel, IDashboard>> _dashboardModelExporterMock;
        private Mock<IRedditAccessTokenProviderFactory> _redditAccessTokenProviderFactoryMock;
        private Mock<IRedditLogic> _redditLogicMock;
        private Mock<IContentHelper> _contentHelperMock;
        private Mock<ICookieHelper> _cookieHelperMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _dashboardFactoryMock = new Mock<IDashboardFactory>();
            _dashboardViewModelBuilderMock = new Mock<IViewModelBuilder<DashboardViewModel, IDashboard>>();
            _dashboardModelExporterMock = new Mock<IModelExporter<DashboardExportModel, IDashboard>>();
            _redditAccessTokenProviderFactoryMock = new Mock<IRedditAccessTokenProviderFactory>();
            _redditLogicMock = new Mock<IRedditLogic>();
            _contentHelperMock = new Mock<IContentHelper>();
            _cookieHelperMock = new Mock<ICookieHelper>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCodeIsNull_ThrowsArgumentNullException()
        {
            const string code = null;
            string state = Guid.NewGuid().ToString("D");

            Web.Controllers.HomeController sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.RedditCallback(code, state));

            Assert.AreEqual("code", result.ParamName);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCodeIsEmpty_ThrowsArgumentNullException()
        {
            string code = string.Empty;
            string state = Guid.NewGuid().ToString("D");

            Web.Controllers.HomeController sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.RedditCallback(code, state));

            Assert.AreEqual("code", result.ParamName);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCodeIsWhitespace_ThrowsArgumentNullException()
        {
            const string code = " ";
            string state = Guid.NewGuid().ToString("D");

            Web.Controllers.HomeController sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.RedditCallback(code, state));

            Assert.AreEqual("code", result.ParamName);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCodeIsWhitespaces_ThrowsArgumentNullException()
        {
            const string code = "  ";
            string state = Guid.NewGuid().ToString("D");

            Web.Controllers.HomeController sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.RedditCallback(code, state));

            Assert.AreEqual("code", result.ParamName);
        }

        [TestMethod]
        public async Task RedditCallback_WhenStateIsNull_ThrowsArgumentNullException()
        {
            string code = Guid.NewGuid().ToString("D");
            const string state = null;

            Web.Controllers.HomeController sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.RedditCallback(code, state));

            Assert.AreEqual("state", result.ParamName);
        }

        [TestMethod]
        public async Task RedditCallback_WhenStateIsEmpty_ThrowsArgumentNullException()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = string.Empty;

            Web.Controllers.HomeController sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.RedditCallback(code, state));

            Assert.AreEqual("state", result.ParamName);
        }

        [TestMethod]
        public async Task RedditCallback_WhenStateIsWhitespace_ThrowsArgumentNullException()
        {
            string code = Guid.NewGuid().ToString("D");
            const string state = " ";

            Web.Controllers.HomeController sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.RedditCallback(code, state));

            Assert.AreEqual("state", result.ParamName);
        }

        [TestMethod]
        public async Task RedditCallback_WhenStateIsWhitespaces_ThrowsArgumentNullException()
        {
            string code = Guid.NewGuid().ToString("D");
            const string state = "  ";

            Web.Controllers.HomeController sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.RedditCallback(code, state));

            Assert.AreEqual("state", result.ParamName);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithError_AssertToDashboardSettingsViewModelWasNotCalledOnContentHelperWithState()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");
            string error = Guid.NewGuid().ToString("D");

            Web.Controllers.HomeController sut = CreateSut();

            await sut.RedditCallback(code, state, error);

            _contentHelperMock.Verify(m => m.ToDashboardSettingsViewModel(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithError_AssertAbsoluteUrlWasNotCalledOnContentHelper()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");
            string error = Guid.NewGuid().ToString("D");

            Web.Controllers.HomeController sut = CreateSut();

            await sut.RedditCallback(code, state, error);

            _contentHelperMock.Verify(m => m.AbsoluteUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithError_AssertGetRedditAccessTokenAsyncWasNotCalledOnRedditAccessTokenProviderFactory()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");
            string error = Guid.NewGuid().ToString("D");

            Web.Controllers.HomeController sut = CreateSut();

            await sut.RedditCallback(code, state, error);

            _redditAccessTokenProviderFactoryMock.Verify(m => m.GetRedditAccessTokenAsync(It.IsAny<string>(), It.IsAny<Uri>()), Times.Never);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithError_AssertGetAuthenticatedUserAsyncWasNotCalledOnRedditLogic()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");
            string error = Guid.NewGuid().ToString("D");

            Web.Controllers.HomeController sut = CreateSut();

            await sut.RedditCallback(code, state, error);

            _redditLogicMock.Verify(m => m.GetAuthenticatedUserAsync(It.IsAny<IRedditAccessToken>()), Times.Never);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithError_ReturnsUnauthorizedObjectResult()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");
            string error = Guid.NewGuid().ToString("D");

            Web.Controllers.HomeController sut = CreateSut();

            IActionResult result = await sut.RedditCallback(code, state, error);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));

            UnauthorizedObjectResult unauthorizedObjectResult = (UnauthorizedObjectResult) result;
            Assert.IsNotNull(unauthorizedObjectResult);
            Assert.IsInstanceOfType(unauthorizedObjectResult.Value, typeof(string));

            string value = (string) unauthorizedObjectResult.Value;
            Assert.IsNotNull(value);
            Assert.AreEqual($"Unable to get the access token from Reddit: {error}", value);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithoutError_AssertToDashboardSettingsViewModelWasCalledOnContentHelperWithState()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");

            Web.Controllers.HomeController sut = CreateSut();

            await sut.RedditCallback(code, state);

            _contentHelperMock.Verify(m => m.ToDashboardSettingsViewModel(It.Is<string>(value => string.CompareOrdinal(value, state) == 0)), Times.Once);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithoutErrorAndStateCannotBeConvertedToDashboardSettingsViewModel_AssertAbsoluteUrlWasNotCalledOnContentHelper()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");

            Web.Controllers.HomeController sut = CreateSut(false);

            await sut.RedditCallback(code, state);

            _contentHelperMock.Verify(m => m.AbsoluteUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithoutErrorAndStateCannotBeConvertedToDashboardSettingsViewModel_AssertGetRedditAccessTokenAsyncWasNotCalledOnRedditAccessTokenProviderFactory()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");

            Web.Controllers.HomeController sut = CreateSut(false);

            await sut.RedditCallback(code, state);

            _redditAccessTokenProviderFactoryMock.Verify(m => m.GetRedditAccessTokenAsync(It.IsAny<string>(), It.IsAny<Uri>()), Times.Never);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithoutErrorAndStateCannotBeConvertedToDashboardSettingsViewModel_AssertGetAuthenticatedUserAsyncWasNotCalledOnRedditLogic()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");

            Web.Controllers.HomeController sut = CreateSut(false);

            await sut.RedditCallback(code, state);

            _redditLogicMock.Verify(m => m.GetAuthenticatedUserAsync(It.IsAny<IRedditAccessToken>()), Times.Never);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithoutErrorAndStateCannotBeConvertedToDashboardSettingsViewModel_ReturnsBadRequestResult()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");

            Web.Controllers.HomeController sut = CreateSut(false);

            IActionResult result = await sut.RedditCallback(code, state);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithoutErrorAndStateCanBeConvertedToDashboardSettingsViewModel_AssertAbsoluteUrlWasCalledOnContentHelper()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");

            Web.Controllers.HomeController sut = CreateSut();

            await sut.RedditCallback(code, state);

            _contentHelperMock.Verify(m => m.AbsoluteUrl(
                    It.Is<string>(value => string.CompareOrdinal(value, "RedditCallback") == 0), 
                    It.Is<string>(value => string.CompareOrdinal(value, "Home") == 0)), 
                Times.Once);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithoutErrorAndStateCanBeConvertedToDashboardSettingsViewModel_AssertGetRedditAccessTokenAsyncWasCalledOnRedditAccessTokenProviderFactory()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");

            Web.Controllers.HomeController sut = CreateSut();

            await sut.RedditCallback(code, state);

            _redditAccessTokenProviderFactoryMock.Verify(m => m.GetRedditAccessTokenAsync(
                    It.Is<string>(value => string.CompareOrdinal(value, code) == 0), 
                    It.Is<Uri>(value => value != null && string.CompareOrdinal(value.AbsoluteUri, "http://localhost/Home/RedditCallback") == 0)), 
                Times.Once);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithoutErrorAndStateCanBeConvertedToDashboardSettingsViewModelAndRedditAccessTokenWasReturned_AssertGetAuthenticatedUserAsyncWasCalledOnRedditLogicWithResolvedRedditAccessToken()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random);

            IRedditAccessToken redditAccessToken = BuildRedditAccessToken(_random);
            Web.Controllers.HomeController sut = CreateSut(dashboardSettingsViewModel: dashboardSettingsViewModel, redditAccessToken: redditAccessToken);

            await sut.RedditCallback(code, state);

            _redditLogicMock.Verify(m => m.GetAuthenticatedUserAsync(It.Is<IRedditAccessToken>(value => value != null && value == redditAccessToken)), Times.Once);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithoutErrorAndStateCanBeConvertedToDashboardSettingsViewModelAndRedditAuthenticatedUserWhoIsOver18WasReturnedForRedditAccessToken_ReturnsViewResultWithDashboardSettingsViewModel()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");

            int numberOfNews = _random.Next(25, 50);
            bool useReddit = _random.Next(100) > 50;
            bool allowNsfwContent = _random.Next(100) > 50;
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;
            bool exportData = _random.Next(100) > 50;
            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, numberOfNews, useReddit, allowNsfwContent, includeNsfwContent, onlyNsfwContent, null, exportData);

            IRedditAccessToken redditAccessToken = BuildRedditAccessToken(_random);
            IRedditAuthenticatedUser redditAuthenticatedUser = BuildRedditAuthenticatedUser(_random, over18: true);
            Web.Controllers.HomeController sut = CreateSut(dashboardSettingsViewModel: dashboardSettingsViewModel, hasRedditAccessToken: true, redditAccessToken: redditAccessToken, hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser);

            IActionResult result = await sut.RedditCallback(code, state);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult) result;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewName);
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(DashboardSettingsViewModel));

            DashboardSettingsViewModel dashboardSettingsViewModelFromViewResult = (DashboardSettingsViewModel) viewResult.Model;
            Assert.IsNotNull(dashboardSettingsViewModelFromViewResult);
            Assert.AreEqual(numberOfNews, dashboardSettingsViewModelFromViewResult.NumberOfNews);
            Assert.AreEqual(useReddit, dashboardSettingsViewModelFromViewResult.UseReddit);
            Assert.IsTrue(dashboardSettingsViewModelFromViewResult.AllowNsfwContent);
            Assert.IsNotNull(dashboardSettingsViewModelFromViewResult.IncludeNsfwContent);
            Assert.AreEqual(includeNsfwContent, dashboardSettingsViewModelFromViewResult.IncludeNsfwContent.Value);
            Assert.AreEqual(includeNsfwContent, dashboardSettingsViewModelFromViewResult.NotNullableIncludeNsfwContent);
            Assert.IsNotNull(dashboardSettingsViewModelFromViewResult.OnlyNsfwContent);
            Assert.AreEqual(onlyNsfwContent, dashboardSettingsViewModelFromViewResult.OnlyNsfwContent.Value);
            Assert.AreEqual(onlyNsfwContent, dashboardSettingsViewModelFromViewResult.NotNullableOnlyNsfwContent);
            Assert.IsNotNull(dashboardSettingsViewModelFromViewResult.RedditAccessToken);
            Assert.AreEqual(redditAccessToken.ToBase64(), dashboardSettingsViewModelFromViewResult.RedditAccessToken);
            Assert.AreEqual(exportData, dashboardSettingsViewModelFromViewResult.ExportData);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithoutErrorAndStateCanBeConvertedToDashboardSettingsViewModelAndRedditAuthenticatedUserWhoIsBelow18WasReturnedForRedditAccessToken_ReturnsViewResultWithDashboardSettingsViewModel()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");

            int numberOfNews = _random.Next(25, 50);
            bool useReddit = _random.Next(100) > 50;
            bool allowNsfwContent = _random.Next(100) > 50;
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;
            bool exportData = _random.Next(100) > 50;
            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, numberOfNews, useReddit, allowNsfwContent, includeNsfwContent, onlyNsfwContent, null, exportData);

            IRedditAccessToken redditAccessToken = BuildRedditAccessToken(_random);
            IRedditAuthenticatedUser redditAuthenticatedUser = BuildRedditAuthenticatedUser(_random, over18: false);
            Web.Controllers.HomeController sut = CreateSut(dashboardSettingsViewModel: dashboardSettingsViewModel, hasRedditAccessToken: true, redditAccessToken: redditAccessToken, hasRedditAuthenticatedUser: true, redditAuthenticatedUser: redditAuthenticatedUser);

            IActionResult result = await sut.RedditCallback(code, state);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult) result;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewName);
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(DashboardSettingsViewModel));

            DashboardSettingsViewModel dashboardSettingsViewModelFromViewResult = (DashboardSettingsViewModel) viewResult.Model;
            Assert.IsNotNull(dashboardSettingsViewModelFromViewResult);
            Assert.AreEqual(numberOfNews, dashboardSettingsViewModelFromViewResult.NumberOfNews);
            Assert.AreEqual(useReddit, dashboardSettingsViewModelFromViewResult.UseReddit);
            Assert.IsFalse(dashboardSettingsViewModelFromViewResult.AllowNsfwContent);
            Assert.IsNull(dashboardSettingsViewModelFromViewResult.IncludeNsfwContent);
            Assert.IsFalse(dashboardSettingsViewModelFromViewResult.NotNullableIncludeNsfwContent);
            Assert.IsNull(dashboardSettingsViewModelFromViewResult.OnlyNsfwContent);
            Assert.IsFalse(dashboardSettingsViewModelFromViewResult.NotNullableOnlyNsfwContent);
            Assert.IsNotNull(dashboardSettingsViewModelFromViewResult.RedditAccessToken);
            Assert.AreEqual(redditAccessToken.ToBase64(), dashboardSettingsViewModelFromViewResult.RedditAccessToken);
            Assert.AreEqual(exportData, dashboardSettingsViewModelFromViewResult.ExportData);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithoutErrorAndStateCanBeConvertedToDashboardSettingsViewModelAndNoRedditAccessTokenWasReturned_AssertGetAuthenticatedUserAsyncWasNotCalledOnRedditLogic()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random);

            Web.Controllers.HomeController sut = CreateSut(dashboardSettingsViewModel: dashboardSettingsViewModel, hasRedditAccessToken: false);

            await sut.RedditCallback(code, state);
 
            _redditLogicMock.Verify(m => m.GetAuthenticatedUserAsync(It.IsAny<IRedditAccessToken>()), Times.Never);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithoutErrorAndStateCanBeConvertedToDashboardSettingsViewModelAndNoRedditAccessTokenWasReturned_ReturnsViewResultWithDashboardSettingsViewModel()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");

            int numberOfNews = _random.Next(25, 50);
            bool useReddit = _random.Next(100) > 50;
            bool allowNsfwContent = _random.Next(100) > 50;
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;
            bool exportData = _random.Next(100) > 50;
            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, numberOfNews, useReddit, allowNsfwContent, includeNsfwContent, onlyNsfwContent, null, exportData);

            Web.Controllers.HomeController sut = CreateSut(dashboardSettingsViewModel: dashboardSettingsViewModel, hasRedditAccessToken: false);

            IActionResult result = await sut.RedditCallback(code, state);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult) result;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewName);
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(DashboardSettingsViewModel));

            DashboardSettingsViewModel dashboardSettingsViewModelFromViewResult = (DashboardSettingsViewModel) viewResult.Model;
            Assert.IsNotNull(dashboardSettingsViewModelFromViewResult);
            Assert.AreEqual(numberOfNews, dashboardSettingsViewModelFromViewResult.NumberOfNews);
            Assert.IsFalse(dashboardSettingsViewModelFromViewResult.UseReddit);
            Assert.IsFalse(dashboardSettingsViewModelFromViewResult.AllowNsfwContent);
            Assert.IsNull(dashboardSettingsViewModelFromViewResult.IncludeNsfwContent);
            Assert.IsFalse(dashboardSettingsViewModelFromViewResult.NotNullableIncludeNsfwContent);
            Assert.IsNull(dashboardSettingsViewModelFromViewResult.OnlyNsfwContent);
            Assert.IsFalse(dashboardSettingsViewModelFromViewResult.NotNullableOnlyNsfwContent);
            Assert.IsNull(dashboardSettingsViewModelFromViewResult.RedditAccessToken);
            Assert.AreEqual(exportData, dashboardSettingsViewModelFromViewResult.ExportData);
        }

        [TestMethod]
        public async Task RedditCallback_WhenCalledWithoutErrorAndStateCanBeConvertedToDashboardSettingsViewModelAndNoRedditAuthenticatedUserWasReturnedForRedditAccessToken_ReturnsViewResultWithDashboardSettingsViewModel()
        {
            string code = Guid.NewGuid().ToString("D");
            string state = Guid.NewGuid().ToString("D");

            int numberOfNews = _random.Next(25, 50);
            bool useReddit = _random.Next(100) > 50;
            bool allowNsfwContent = _random.Next(100) > 50;
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;
            bool exportData = _random.Next(100) > 50;
            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel(_random, numberOfNews, useReddit, allowNsfwContent, includeNsfwContent, onlyNsfwContent, null, exportData);

            Web.Controllers.HomeController sut = CreateSut(dashboardSettingsViewModel: dashboardSettingsViewModel, hasRedditAccessToken: true, hasRedditAuthenticatedUser: false);

            IActionResult result = await sut.RedditCallback(code, state);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult) result;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewName);
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(DashboardSettingsViewModel));

            DashboardSettingsViewModel dashboardSettingsViewModelFromViewResult = (DashboardSettingsViewModel) viewResult.Model;
            Assert.IsNotNull(dashboardSettingsViewModelFromViewResult);
            Assert.AreEqual(numberOfNews, dashboardSettingsViewModelFromViewResult.NumberOfNews);
            Assert.IsFalse(dashboardSettingsViewModelFromViewResult.UseReddit);
            Assert.IsFalse(dashboardSettingsViewModelFromViewResult.AllowNsfwContent);
            Assert.IsNull(dashboardSettingsViewModelFromViewResult.IncludeNsfwContent);
            Assert.IsFalse(dashboardSettingsViewModelFromViewResult.NotNullableIncludeNsfwContent);
            Assert.IsNull(dashboardSettingsViewModelFromViewResult.OnlyNsfwContent);
            Assert.IsFalse(dashboardSettingsViewModelFromViewResult.NotNullableOnlyNsfwContent);
            Assert.IsNull(dashboardSettingsViewModelFromViewResult.RedditAccessToken);
            Assert.AreEqual(exportData, dashboardSettingsViewModelFromViewResult.ExportData);
        }

        private Web.Controllers.HomeController CreateSut(bool isDashboardSettingsViewModel = true, DashboardSettingsViewModel dashboardSettingsViewModel = null, bool hasRedditAccessToken = true, IRedditAccessToken redditAccessToken = null, bool hasRedditAuthenticatedUser = true, IRedditAuthenticatedUser redditAuthenticatedUser = null)
        {
            _contentHelperMock.Setup(m => m.ToDashboardSettingsViewModel(It.IsAny<string>()))
                .Returns(isDashboardSettingsViewModel ? dashboardSettingsViewModel ?? BuildDashboardSettingsViewModel(_random) : null);
            _contentHelperMock.Setup(m => m.AbsoluteUrl(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((action, controller) => new Uri($"http://localhost/{controller}/{action}").AbsoluteUri);

            _redditAccessTokenProviderFactoryMock.Setup(m => m.GetRedditAccessTokenAsync(It.IsAny<string>(), It.IsAny<Uri>()))
                .Returns(Task.FromResult(hasRedditAccessToken ? redditAccessToken ?? BuildRedditAccessToken(_random) : null));

            _redditLogicMock.Setup(m => m.GetAuthenticatedUserAsync(It.IsAny<IRedditAccessToken>()))
                .Returns(Task.FromResult(hasRedditAuthenticatedUser ? redditAuthenticatedUser ?? BuildRedditAuthenticatedUser(_random) : null));

            return new Web.Controllers.HomeController(
                _dashboardFactoryMock.Object,
                _dashboardViewModelBuilderMock.Object,
                _dashboardModelExporterMock.Object,
                _redditAccessTokenProviderFactoryMock.Object,
                _redditLogicMock.Object,
                _contentHelperMock.Object,
                _cookieHelperMock.Object);
        }
    }
}