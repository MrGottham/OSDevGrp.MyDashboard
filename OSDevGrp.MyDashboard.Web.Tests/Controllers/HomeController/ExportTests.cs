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
using System.Text;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Web.Tests.Controllers.HomeController
{
    [TestClass]
    public class ExportTests : TestBase
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
        public async Task Export_WhenNumberOfNewsIsLowerThan0_AssertBuildAsyncWasNotCalledOnDashboardFactory()
        {
            Web.Controllers.HomeController sut = CreateSut();

            int numberOfNews = _random.Next(1, 5) * -1;
            await sut.Export(numberOfNews);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.IsAny<IDashboardSettings>()), Times.Never);
        }

        [TestMethod]
        public async Task Export_WhenNumberOfNewsIsLowerThan0_AssertExportAsyncWasNotCalledOnDashboardModelExporter()
        {
            Web.Controllers.HomeController sut = CreateSut();

            int numberOfNews = _random.Next(1, 5) * -1;
            await sut.Export(numberOfNews);

            _dashboardModelExporterMock.Verify(m => m.ExportAsync(It.IsAny<IDashboard>()), Times.Never);
        }

        [TestMethod]
        public async Task Export_WhenNumberOfNewsIsLowerThan0_ReturnsBadRequestObjectResult()
        {
            Web.Controllers.HomeController sut = CreateSut();

            int numberOfNews = _random.Next(1, 5) * -1;
            IActionResult result = await sut.Export(numberOfNews);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            BadRequestObjectResult badRequestObjectResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestObjectResult);
            Assert.IsNotNull(badRequestObjectResult.Value);
            Assert.IsInstanceOfType(badRequestObjectResult.Value, typeof(string));
            Assert.AreEqual("The submitted value for 'numberOfNews' cannot be lower than 0.", badRequestObjectResult.Value);
        }

        [TestMethod]
        public async Task Export_WhenNumberOfNewsIsGreaterThan250_AssertBuildAsyncWasNotCalledOnDashboardFactory()
        {
            Web.Controllers.HomeController sut = CreateSut();

            int numberOfNews = _random.Next(251, 255);
            await sut.Export(numberOfNews);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.IsAny<IDashboardSettings>()), Times.Never);
        }

        [TestMethod]
        public async Task Export_WhenNumberOfNewsIsGreaterThan250_AssertExportAsyncWasNotCalledOnDashboardModelExporter()
        {
            Web.Controllers.HomeController sut = CreateSut();

            int numberOfNews = _random.Next(251, 255);
            await sut.Export(numberOfNews);

            _dashboardModelExporterMock.Verify(m => m.ExportAsync(It.IsAny<IDashboard>()), Times.Never);
        }

        [TestMethod]
        public async Task Export_WhenNumberOfNewsIsGreaterThan250_ReturnsBadRequestObjectResult()
        {
            Web.Controllers.HomeController sut = CreateSut();

            int numberOfNews = _random.Next(251, 255);
            IActionResult result = await sut.Export(numberOfNews);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            BadRequestObjectResult badRequestObjectResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestObjectResult);
            Assert.IsNotNull(badRequestObjectResult.Value);
            Assert.IsInstanceOfType(badRequestObjectResult.Value, typeof(string));
            Assert.AreEqual("The submitted value for 'numberOfNews' cannot be greater than 250.", badRequestObjectResult.Value);
        }

        [TestMethod]
        public async Task Export_WhenCalledWithoutNumberOfNews_AssertBuildAsyncWasCalledOnDashboardFactoryWithDashboardSettingsWhereNumberOfNewsIsEqualTo100()
        {
            Web.Controllers.HomeController sut = CreateSut();

            await sut.Export();

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value != null && value.NumberOfNews == 100)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenCalledWithoutNumberOfNews_AssertExportAsyncWasCalledOnDashboardModelExporterWithBuildDashboardFromDashboardFactory()
        {
            IDashboard dashboard = BuildDashboard();
            Web.Controllers.HomeController sut = CreateSut(dashboard);

            await sut.Export();

            _dashboardModelExporterMock.Verify(m => m.ExportAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenCalledWithoutNumberOfNews_ReturnsOkObjectResult()
        {
            DashboardExportModel dashboardExportModel = BuildDashboardExportModel();
            Web.Controllers.HomeController sut = CreateSut(dashboardExportModel: dashboardExportModel);

            IActionResult result = await sut.Export();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            OkObjectResult okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.IsNotNull(okObjectResult.Value);
            Assert.AreEqual(dashboardExportModel, okObjectResult.Value);
        }

        [TestMethod]
        public async Task Export_WhenCalledWithNumberOfNews_AssertBuildAsyncWasCalledOnDashboardFactoryWithDashboardSettingsWhereNumberOfNewsIsEqualToNumberOfNews()
        {
            Web.Controllers.HomeController sut = CreateSut();

            int numberOfNews = _random.Next(0, 250);
            await sut.Export(numberOfNews);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value != null && value.NumberOfNews == numberOfNews)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenCalledWithNumberOfNews_AssertExportAsyncWasCalledOnDashboardModelExporterWithBuildDashboardFromDashboardFactory()
        {
            IDashboard dashboard = BuildDashboard();
            Web.Controllers.HomeController sut = CreateSut(dashboard);

            int numberOfNews = _random.Next(0, 250);
            await sut.Export(numberOfNews);

            _dashboardModelExporterMock.Verify(m => m.ExportAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenCalledWithNumberOfNews_ReturnsOkObjectResult()
        {
            DashboardExportModel dashboardExportModel = BuildDashboardExportModel();
            Web.Controllers.HomeController sut = CreateSut(dashboardExportModel: dashboardExportModel);

            int numberOfNews = _random.Next(0, 250);
            IActionResult result = await sut.Export(numberOfNews);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            OkObjectResult okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.IsNotNull(okObjectResult.Value);
            Assert.AreEqual(dashboardExportModel, okObjectResult.Value);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenIsNull_AssertBuildAsyncWasCalledOnDashboardFactoryWithDashboardSettingsWithoutRedditValues()
        {
            Web.Controllers.HomeController sut = CreateSut();

            await sut.Export(includeNsfwContent: _random.Next(100) > 50, onlyNsfwContent: _random.Next(100) > 50);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value != null && value.RedditAccessToken == null && value.UseReddit == false && value.IncludeNsfwContent == false && value.OnlyNsfwContent == false)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenIsNull_AssertExportAsyncWasCalledOnDashboardModelExporterWithBuildDashboardFromDashboardFactory()
        {
            IDashboard dashboard = BuildDashboard();
            Web.Controllers.HomeController sut = CreateSut(dashboard);

            await sut.Export(includeNsfwContent: _random.Next(100) > 50, onlyNsfwContent: _random.Next(100) > 50);

            _dashboardModelExporterMock.Verify(m => m.ExportAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenIsNull_ReturnsOkObjectResult()
        {
            DashboardExportModel dashboardExportModel = BuildDashboardExportModel();
            Web.Controllers.HomeController sut = CreateSut(dashboardExportModel: dashboardExportModel);

            IActionResult result = await sut.Export(includeNsfwContent: _random.Next(100) > 50, onlyNsfwContent: _random.Next(100) > 50);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            OkObjectResult okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.IsNotNull(okObjectResult.Value);
            Assert.AreEqual(dashboardExportModel, okObjectResult.Value);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenIsEmpty_AssertBuildAsyncWasCalledOnDashboardFactoryWithDashboardSettingsWithoutRedditValues()
        {
            Web.Controllers.HomeController sut = CreateSut();

            await sut.Export(redditAccessToken: string.Empty, includeNsfwContent: _random.Next(100) > 50, onlyNsfwContent: _random.Next(100) > 50);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value != null && value.RedditAccessToken == null && value.UseReddit == false && value.IncludeNsfwContent == false && value.OnlyNsfwContent == false)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenIsEmpty_AssertExportAsyncWasCalledOnDashboardModelExporterWithBuildDashboardFromDashboardFactory()
        {
            IDashboard dashboard = BuildDashboard();
            Web.Controllers.HomeController sut = CreateSut(dashboard);

            await sut.Export(redditAccessToken: string.Empty, includeNsfwContent: _random.Next(100) > 50, onlyNsfwContent: _random.Next(100) > 50);

            _dashboardModelExporterMock.Verify(m => m.ExportAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenIsEmpty_ReturnsOkObjectResult()
        {
            DashboardExportModel dashboardExportModel = BuildDashboardExportModel();
            Web.Controllers.HomeController sut = CreateSut(dashboardExportModel: dashboardExportModel);

            IActionResult result = await sut.Export(redditAccessToken: string.Empty, includeNsfwContent: _random.Next(100) > 50, onlyNsfwContent: _random.Next(100) > 50);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            OkObjectResult okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.IsNotNull(okObjectResult.Value);
            Assert.AreEqual(dashboardExportModel, okObjectResult.Value);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenIsWhiteSpace_AssertBuildAsyncWasCalledOnDashboardFactoryWithDashboardSettingsWithoutRedditValues()
        {
            Web.Controllers.HomeController sut = CreateSut();

            await sut.Export(redditAccessToken: " ", includeNsfwContent: _random.Next(100) > 50, onlyNsfwContent: _random.Next(100) > 50);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value != null && value.RedditAccessToken == null && value.UseReddit == false && value.IncludeNsfwContent == false && value.OnlyNsfwContent == false)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenIsWhiteSpace_AssertExportAsyncWasCalledOnDashboardModelExporterWithBuildDashboardFromDashboardFactory()
        {
            IDashboard dashboard = BuildDashboard();
            Web.Controllers.HomeController sut = CreateSut(dashboard);

            await sut.Export(redditAccessToken: " ", includeNsfwContent: _random.Next(100) > 50, onlyNsfwContent: _random.Next(100) > 50);

            _dashboardModelExporterMock.Verify(m => m.ExportAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenIsWhiteSpace_ReturnsOkObjectResult()
        {
            DashboardExportModel dashboardExportModel = BuildDashboardExportModel();
            Web.Controllers.HomeController sut = CreateSut(dashboardExportModel: dashboardExportModel);

            IActionResult result = await sut.Export(redditAccessToken: " ", includeNsfwContent: _random.Next(100) > 50, onlyNsfwContent: _random.Next(100) > 50);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            OkObjectResult okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.IsNotNull(okObjectResult.Value);
            Assert.AreEqual(dashboardExportModel, okObjectResult.Value);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenIsWhiteSpaces_AssertBuildAsyncWasCalledOnDashboardFactoryWithDashboardSettingsWithoutRedditValues()
        {
            Web.Controllers.HomeController sut = CreateSut();

            await sut.Export(redditAccessToken: "  ", includeNsfwContent: _random.Next(100) > 50, onlyNsfwContent: _random.Next(100) > 50);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value != null && value.RedditAccessToken == null && value.UseReddit == false && value.IncludeNsfwContent == false && value.OnlyNsfwContent == false)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenIsWhiteSpaces_AssertExportAsyncWasCalledOnDashboardModelExporterWithBuildDashboardFromDashboardFactory()
        {
            IDashboard dashboard = BuildDashboard();
            Web.Controllers.HomeController sut = CreateSut(dashboard);

            await sut.Export(redditAccessToken: "  ", includeNsfwContent: _random.Next(100) > 50, onlyNsfwContent: _random.Next(100) > 50);

            _dashboardModelExporterMock.Verify(m => m.ExportAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenIsWhiteSpaces_ReturnsOkObjectResult()
        {
            DashboardExportModel dashboardExportModel = BuildDashboardExportModel();
            Web.Controllers.HomeController sut = CreateSut(dashboardExportModel: dashboardExportModel);

            IActionResult result = await sut.Export(redditAccessToken: "  ", includeNsfwContent: _random.Next(100) > 50, onlyNsfwContent: _random.Next(100) > 50);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            OkObjectResult okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.IsNotNull(okObjectResult.Value);
            Assert.AreEqual(dashboardExportModel, okObjectResult.Value);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenIsNotBase64_AssertBuildAsyncWasNotCalledOnDashboardFactory()
        {
            Web.Controllers.HomeController sut = CreateSut();

            string redditAccessToken = Guid.NewGuid().ToString("D");
            await sut.Export(redditAccessToken: redditAccessToken);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.IsAny<IDashboardSettings>()), Times.Never);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenIsNotBase64_AssertExportAsyncWasNotCalledOnDashboardModelExporter()
        {
            Web.Controllers.HomeController sut = CreateSut();

            string redditAccessToken = Guid.NewGuid().ToString("D");
            await sut.Export(redditAccessToken: redditAccessToken);

            _dashboardModelExporterMock.Verify(m => m.ExportAsync(It.IsAny<IDashboard>()), Times.Never);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenIsNotBase64_ReturnsBadRequestObjectResult()
        {
            Web.Controllers.HomeController sut = CreateSut();

            string redditAccessToken = Guid.NewGuid().ToString("D");
            IActionResult result = await sut.Export(redditAccessToken: redditAccessToken);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            BadRequestObjectResult badRequestObjectResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestObjectResult);
            Assert.IsNotNull(badRequestObjectResult.Value);
            Assert.IsInstanceOfType(badRequestObjectResult.Value, typeof(string));
            Assert.AreEqual("The submitted value for 'redditAccessToken' is invalid.", badRequestObjectResult.Value);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCannotBeDeserialized_AssertBuildAsyncWasNotCalledOnDashboardFactory()
        {
            Web.Controllers.HomeController sut = CreateSut();

            string redditAccessToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("D")));
            await sut.Export(redditAccessToken: redditAccessToken);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.IsAny<IDashboardSettings>()), Times.Never);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCannotBeDeserialized_AssertExportAsyncWasNotCalledOnDashboardModelExporter()
        {
            Web.Controllers.HomeController sut = CreateSut();

            string redditAccessToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("D")));
            await sut.Export(redditAccessToken: redditAccessToken);

            _dashboardModelExporterMock.Verify(m => m.ExportAsync(It.IsAny<IDashboard>()), Times.Never);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCannotBeDeserialized_ReturnsBadRequestObjectResult()
        {
            Web.Controllers.HomeController sut = CreateSut();

            string redditAccessToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("D")));
            IActionResult result = await sut.Export(redditAccessToken: redditAccessToken);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            BadRequestObjectResult badRequestObjectResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestObjectResult);
            Assert.IsNotNull(badRequestObjectResult.Value);
            Assert.IsInstanceOfType(badRequestObjectResult.Value, typeof(string));
            Assert.AreEqual("The submitted value for 'redditAccessToken' is invalid.", badRequestObjectResult.Value);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCanBeDeserialized_AssertBuildAsyncWasCalledOnDashboardFactoryWithDashboardSettingsWhereRedditAccessTokenIsEqualToRedditAccessToken()
        {
            Web.Controllers.HomeController sut = CreateSut();

            string redditAccessToken = BuildRedditAccessTokenAsBase64(_random);
            await sut.Export(redditAccessToken: redditAccessToken);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value != null && value.RedditAccessToken != null && string.CompareOrdinal(value.RedditAccessToken.ToBase64(), redditAccessToken) == 0)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCanBeDeserialized_AssertExportAsyncWasCalledOnDashboardModelExporterWithBuildDashboardFromDashboardFactory()
        {
            IDashboard dashboard = BuildDashboard();
            Web.Controllers.HomeController sut = CreateSut(dashboard);

            string redditAccessToken = BuildRedditAccessTokenAsBase64(_random);
            await sut.Export(redditAccessToken: redditAccessToken);

            _dashboardModelExporterMock.Verify(m => m.ExportAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCanBeDeserialized_ReturnsOkObjectResult()
        {
            DashboardExportModel dashboardExportModel = BuildDashboardExportModel();
            Web.Controllers.HomeController sut = CreateSut(dashboardExportModel: dashboardExportModel);

            string redditAccessToken = BuildRedditAccessTokenAsBase64(_random);
            IActionResult result = await sut.Export(redditAccessToken: redditAccessToken);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            OkObjectResult okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.IsNotNull(okObjectResult.Value);
            Assert.AreEqual(dashboardExportModel, okObjectResult.Value);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCanBeDeserializedAndIncludeNsfwContentIsNotSet_AssertBuildAsyncWasCalledOnDashboardFactoryWithDashboardSettingsWhereIncludeNsfwContentIsFalse()
        {
            Web.Controllers.HomeController sut = CreateSut();

            string redditAccessToken = BuildRedditAccessTokenAsBase64(_random);
            await sut.Export(redditAccessToken: redditAccessToken);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value != null && value.IncludeNsfwContent == false)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCanBeDeserializedAndIncludeNsfwContentIsNotSet_AssertExportAsyncWasCalledOnDashboardModelExporterWithBuildDashboardFromDashboardFactory()
        {
            IDashboard dashboard = BuildDashboard();
            Web.Controllers.HomeController sut = CreateSut(dashboard);

            string redditAccessToken = BuildRedditAccessTokenAsBase64(_random);
            await sut.Export(redditAccessToken: redditAccessToken);

            _dashboardModelExporterMock.Verify(m => m.ExportAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCanBeDeserializedAndIncludeNsfwContentIsNotSet_ReturnsOkObjectResult()
        {
            DashboardExportModel dashboardExportModel = BuildDashboardExportModel();
            Web.Controllers.HomeController sut = CreateSut(dashboardExportModel: dashboardExportModel);

            string redditAccessToken = BuildRedditAccessTokenAsBase64(_random);
            IActionResult result = await sut.Export(redditAccessToken: redditAccessToken);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            OkObjectResult okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.IsNotNull(okObjectResult.Value);
            Assert.AreEqual(dashboardExportModel, okObjectResult.Value);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCanBeDeserializedAndIncludeNsfwContentIsSet_AssertBuildAsyncWasCalledOnDashboardFactoryWithDashboardSettingsWhereIncludeNsfwContentIsTrue()
        {
            Web.Controllers.HomeController sut = CreateSut();

            string redditAccessToken = BuildRedditAccessTokenAsBase64(_random);
            await sut.Export(redditAccessToken: redditAccessToken, includeNsfwContent: true);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value != null && value.IncludeNsfwContent)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCanBeDeserializedAndIncludeNsfwContentIsSet_AssertExportAsyncWasCalledOnDashboardModelExporterWithBuildDashboardFromDashboardFactory()
        {
            IDashboard dashboard = BuildDashboard();
            Web.Controllers.HomeController sut = CreateSut(dashboard);

            string redditAccessToken = BuildRedditAccessTokenAsBase64(_random);
            await sut.Export(redditAccessToken: redditAccessToken, includeNsfwContent: true);

            _dashboardModelExporterMock.Verify(m => m.ExportAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCanBeDeserializedAndIncludeNsfwContentIsSet_ReturnsOkObjectResult()
        {
            DashboardExportModel dashboardExportModel = BuildDashboardExportModel();
            Web.Controllers.HomeController sut = CreateSut(dashboardExportModel: dashboardExportModel);

            string redditAccessToken = BuildRedditAccessTokenAsBase64(_random);
            IActionResult result = await sut.Export(redditAccessToken: redditAccessToken, includeNsfwContent: true);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            OkObjectResult okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.IsNotNull(okObjectResult.Value);
            Assert.AreEqual(dashboardExportModel, okObjectResult.Value);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCanBeDeserializedAndOnlyNsfwContentIsNotSet_AssertBuildAsyncWasCalledOnDashboardFactoryWithDashboardSettingsWhereOnlyNsfwContentIsFalse()
        {
            Web.Controllers.HomeController sut = CreateSut();

            string redditAccessToken = BuildRedditAccessTokenAsBase64(_random);
            await sut.Export(redditAccessToken: redditAccessToken);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value != null && value.OnlyNsfwContent == false)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCanBeDeserializedAndOnlyNsfwContentIsNotSet_AssertExportAsyncWasCalledOnDashboardModelExporterWithBuildDashboardFromDashboardFactory()
        {
            IDashboard dashboard = BuildDashboard();
            Web.Controllers.HomeController sut = CreateSut(dashboard);

            string redditAccessToken = BuildRedditAccessTokenAsBase64(_random);
            await sut.Export(redditAccessToken: redditAccessToken);

            _dashboardModelExporterMock.Verify(m => m.ExportAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCanBeDeserializedAndOnlyNsfwContentIsNotSet_ReturnsOkObjectResult()
        {
            DashboardExportModel dashboardExportModel = BuildDashboardExportModel();
            Web.Controllers.HomeController sut = CreateSut(dashboardExportModel: dashboardExportModel);

            string redditAccessToken = BuildRedditAccessTokenAsBase64(_random);
            IActionResult result = await sut.Export(redditAccessToken: redditAccessToken);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            OkObjectResult okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.IsNotNull(okObjectResult.Value);
            Assert.AreEqual(dashboardExportModel, okObjectResult.Value);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCanBeDeserializedAndOnlyNsfwContentIsSet_AssertBuildAsyncWasCalledOnDashboardFactoryWithDashboardSettingsWhereOnlyNsfwContentIsTrue()
        {
            Web.Controllers.HomeController sut = CreateSut();

            string redditAccessToken = BuildRedditAccessTokenAsBase64(_random);
            await sut.Export(redditAccessToken: redditAccessToken, onlyNsfwContent: true);

            _dashboardFactoryMock.Verify(m => m.BuildAsync(It.Is<IDashboardSettings>(value => value != null && value.OnlyNsfwContent)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCanBeDeserializedAndOnlyNsfwContentIsSet_AssertExportAsyncWasCalledOnDashboardModelExporterWithBuildDashboardFromDashboardFactory()
        {
            IDashboard dashboard = BuildDashboard();
            Web.Controllers.HomeController sut = CreateSut(dashboard);

            string redditAccessToken = BuildRedditAccessTokenAsBase64(_random);
            await sut.Export(redditAccessToken: redditAccessToken, onlyNsfwContent: true);

            _dashboardModelExporterMock.Verify(m => m.ExportAsync(It.Is<IDashboard>(value => value == dashboard)), Times.Once);
        }

        [TestMethod]
        public async Task Export_WhenRedditAccessTokenCanBeDeserializedAndOnlyNsfwContentIsSet_ReturnsOkObjectResult()
        {
            DashboardExportModel dashboardExportModel = BuildDashboardExportModel();
            Web.Controllers.HomeController sut = CreateSut(dashboardExportModel: dashboardExportModel);

            string redditAccessToken = BuildRedditAccessTokenAsBase64(_random);
            IActionResult result = await sut.Export(redditAccessToken: redditAccessToken, onlyNsfwContent: true);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            OkObjectResult okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            Assert.IsNotNull(okObjectResult.Value);
            Assert.AreEqual(dashboardExportModel, okObjectResult.Value);
        }

        private Web.Controllers.HomeController CreateSut(IDashboard dashboard = null, DashboardExportModel dashboardExportModel = null)
        {
            _dashboardFactoryMock.Setup(m => m.BuildAsync(It.IsAny<IDashboardSettings>()))
                .Returns(Task.FromResult(dashboard ?? BuildDashboard()));

            _dashboardModelExporterMock.Setup(m => m.ExportAsync(It.IsAny<IDashboard>()))
                .Returns(Task.FromResult(dashboardExportModel ?? BuildDashboardExportModel()));

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