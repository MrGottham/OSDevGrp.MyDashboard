using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.SystemErrorViewModelBuilder
{
    [TestClass]
    public class BuildAsyncTests
    {
        #region private variables

        private Mock<IHtmlHelper> _htmlHelperMock;

        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _htmlHelperMock = new Mock<IHtmlHelper>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        public async Task BuildAsync_WhenSystemErrorIsNull_ThrowsArgumentNullException()
        {
            IViewModelBuilder<SystemErrorViewModel, ISystemError> sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.BuildAsync(null));

            Assert.AreEqual("input", result.ParamName);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertIdentifierWasCalledOnSystemError()
        {
            Mock<ISystemError> systemErrorMock = CreateSystemErrorMock();

            IViewModelBuilder<SystemErrorViewModel, ISystemError> sut = CreateSut();

            await sut.BuildAsync(systemErrorMock.Object);

            systemErrorMock.Verify(m => m.Identifier, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertTimestampWasCalledOnSystemError()
        {
            Mock<ISystemError> systemErrorMock = CreateSystemErrorMock();

            IViewModelBuilder<SystemErrorViewModel, ISystemError> sut = CreateSut();

            await sut.BuildAsync(systemErrorMock.Object);

            systemErrorMock.Verify(m => m.Timestamp, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertInformationWasCalledOnSystemError()
        {
            Mock<ISystemError> systemErrorMock = CreateSystemErrorMock();

            IViewModelBuilder<SystemErrorViewModel, ISystemError> sut = CreateSut();

            await sut.BuildAsync(systemErrorMock.Object);

            systemErrorMock.Verify(m => m.Information, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertConvertNewLinesWasCalledOnHtmlHelperWithInformation()
        {
            string information = Guid.NewGuid().ToString("D");
            ISystemError systemError = CreateSystemError(information: information);

            IViewModelBuilder<SystemErrorViewModel, ISystemError> sut = CreateSut();

            await sut.BuildAsync(systemError);

            _htmlHelperMock.Verify(m => m.ConvertNewLines(It.Is<string>(value => string.Compare(value, information, false) == 0)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertDetailsWasCalledOnSystemError()
        {
            Mock<ISystemError> systemErrorMock = CreateSystemErrorMock();

            IViewModelBuilder<SystemErrorViewModel, ISystemError> sut = CreateSut();

            await sut.BuildAsync(systemErrorMock.Object);

            systemErrorMock.Verify(m => m.Details, Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_AssertConvertNewLinesWasCalledOnHtmlHelperWithDetails()
        {
            string details = Guid.NewGuid().ToString("D");
            ISystemError systemError = CreateSystemError(details: details);

            IViewModelBuilder<SystemErrorViewModel, ISystemError> sut = CreateSut();

            await sut.BuildAsync(systemError);

            _htmlHelperMock.Verify(m => m.ConvertNewLines(It.Is<string>(value => string.Compare(value, details, false) == 0)), Times.Once);
        }

        [TestMethod]
        public async Task BuildAsync_WhenCalled_ReturnsInitializedSystemErrorViewModel()
        {
            string identifier = Guid.NewGuid().ToString("D");
            DateTime timestamp = DateTime.Now.AddTicks(_random.Next(-5000, 5000));
            string information = Guid.NewGuid().ToString("D");
            string details = Guid.NewGuid().ToString("D");
            ISystemError systemErrorMock = CreateSystemError(identifier, timestamp, information, details);

            IViewModelBuilder<SystemErrorViewModel, ISystemError> sut = CreateSut();

            SystemErrorViewModel result = await sut.BuildAsync(systemErrorMock);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.SystemErrorIdentifier);
            Assert.AreEqual(identifier, result.SystemErrorIdentifier);
            Assert.AreEqual(timestamp, result.Timestamp);
            Assert.IsNotNull(result.Message);
            Assert.AreEqual($"HtmlHelper.ConvertNewLines:{information}", result.Message);
            Assert.IsNotNull(result.Details);
            Assert.AreEqual($"HtmlHelper.ConvertNewLines:{details}", result.Details);
        }

        private IViewModelBuilder<SystemErrorViewModel, ISystemError> CreateSut()
        {
            _htmlHelperMock.Setup(m => m.ConvertNewLines(It.IsAny<string>()))
                .Returns<string>(value => $"HtmlHelper.ConvertNewLines:{value}");

            return new OSDevGrp.MyDashboard.Web.Factories.SystemErrorViewModelBuilder(_htmlHelperMock.Object);
        }

        private ISystemError CreateSystemError(string identifier = null, DateTime? timestamp = null, string information = null, string details = null)
        {
            return CreateSystemErrorMock(identifier, timestamp, information, details).Object;
        }

        private Mock<ISystemError> CreateSystemErrorMock(string identifier = null, DateTime? timestamp = null, string information = null, string details = null)
        {
            Mock<ISystemError> systemErrorMock = new Mock<ISystemError>();
            systemErrorMock.Setup(m => m.Identifier)
                .Returns(identifier);
            systemErrorMock.Setup(m => m.Timestamp)
                .Returns(timestamp ?? DateTime.Now);
            systemErrorMock.Setup(m => m.Information)
                .Returns(information);
            systemErrorMock.Setup(m => m.Details)
                .Returns(details);
            return systemErrorMock;
        }
    } 
}