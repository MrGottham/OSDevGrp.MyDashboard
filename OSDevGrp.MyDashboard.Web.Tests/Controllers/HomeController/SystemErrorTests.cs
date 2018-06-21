using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Controllers.HomeController
{
    [TestClass]
    public class SystemErrorTests
    {
        #region Private variables

        private Mock<IDashboardFactory> _dashboardFactoryMock;
        private Mock<IViewModelBuilder<DashboardViewModel, IDashboard>> _dashboardViewModelBuilderMock;
        private Mock<IRedditAccessTokenProviderFactory> _redditAccessTokenProviderFactoryMock;
        private Mock<IExceptionHandler> _exceptionHandlerMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _dashboardFactoryMock = new Mock<IDashboardFactory>();
            _dashboardViewModelBuilderMock = new Mock<IViewModelBuilder<DashboardViewModel, IDashboard>>();
            _redditAccessTokenProviderFactoryMock = new Mock<IRedditAccessTokenProviderFactory>();
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        }

        [TestMethod]
        [ExpectedArgumentNullException("systemErrorViewModel")]
        public void SystemError_WhenSystemErrorViewModelIsNull_ThrowsArgumentNullExcpetion()
        {
            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            sut.SystemError(null);
        }

        [TestMethod]
        public void SystemError_WhenCalled_ReturnsViewWithSystemErrorViewModel()
        {
            SystemErrorViewModel systemErrorViewModel = new SystemErrorViewModel();

            OSDevGrp.MyDashboard.Web.Controllers.HomeController sut = CreateSut();

            IActionResult result = sut.SystemError(systemErrorViewModel);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult) result;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewName);
            Assert.AreEqual("SystemError", viewResult.ViewName);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreEqual(systemErrorViewModel, viewResult.Model);
        }

        private OSDevGrp.MyDashboard.Web.Controllers.HomeController CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Controllers.HomeController(
                _dashboardFactoryMock.Object,
                _dashboardViewModelBuilderMock.Object,
                _redditAccessTokenProviderFactoryMock.Object,
                _exceptionHandlerMock.Object,
                _httpContextAccessorMock.Object);
        }
    }
}