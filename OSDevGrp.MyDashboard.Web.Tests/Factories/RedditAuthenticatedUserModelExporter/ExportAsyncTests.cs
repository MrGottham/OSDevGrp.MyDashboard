using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.RedditAuthenticatedUserModelExporter
{
    [TestClass]
    public class ExportAsyncTests
    {
        #region Private variables

        private Mock<IExceptionHandler> _exceptionHandlerMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        public async Task ExportAsync_WhenInputIsNull_ThrowsArgumentNullException()
        {
            IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser> sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.ExportAsync(null));

            Assert.AreEqual("input", result.ParamName);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertFullNameWasCalledOnRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser> sut = CreateSut();

            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = BuildRedditAuthenticatedUserMock();
            await sut.ExportAsync(redditAuthenticatedUserMock.Object);

            redditAuthenticatedUserMock.Verify(m => m.FullName, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertCreatedTimeWasCalledOnRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser> sut = CreateSut();

            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = BuildRedditAuthenticatedUserMock();
            await sut.ExportAsync(redditAuthenticatedUserMock.Object);

            redditAuthenticatedUserMock.Verify(m => m.CreatedTime, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertUserNameWasCalledOnRedditLink()
        {
            IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser> sut = CreateSut();

            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = BuildRedditAuthenticatedUserMock();
            await sut.ExportAsync(redditAuthenticatedUserMock.Object);

            redditAuthenticatedUserMock.Verify(m => m.UserName, Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithIdentifierFromRedditAuthenticatedUser()
        {
            IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser> sut = CreateSut();

            string fullName = Guid.NewGuid().ToString("D");
            IRedditAuthenticatedUser redditAuthenticatedUser = BuildRedditAuthenticatedUser(fullName);
            DashboardItemExportModel result = await sut.ExportAsync(redditAuthenticatedUser);

            Assert.IsNotNull(result);
            Assert.AreEqual(fullName, result.Identifier);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithTimestampFromRedditAuthenticatedUser()
        {
            IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser> sut = CreateSut();

            DateTime createdTime = DateTime.Now.AddSeconds(_random.Next(300, 3600) * -1);
            IRedditAuthenticatedUser redditAuthenticatedUser = BuildRedditAuthenticatedUser(createdTime: createdTime);
            DashboardItemExportModel result = await sut.ExportAsync(redditAuthenticatedUser);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdTime, result.Timestamp);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithInformationFromRedditAuthenticatedUser()
        {
            IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser> sut = CreateSut();

            string userName = Guid.NewGuid().ToString("D");
            IRedditAuthenticatedUser redditAuthenticatedUser = BuildRedditAuthenticatedUser(userName: userName);
            DashboardItemExportModel result = await sut.ExportAsync(redditAuthenticatedUser);

            Assert.IsNotNull(result);
            Assert.AreEqual(userName, result.Information);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithoutDetails()
        {
            IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser> sut = CreateSut();

            IRedditAuthenticatedUser redditAuthenticatedUser = BuildRedditAuthenticatedUser();
            DashboardItemExportModel result = await sut.ExportAsync(redditAuthenticatedUser);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Details);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithProvider()
        {
            IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser> sut = CreateSut();

            IRedditAuthenticatedUser redditAuthenticatedUser = BuildRedditAuthenticatedUser();
            DashboardItemExportModel result = await sut.ExportAsync(redditAuthenticatedUser);

            Assert.IsNotNull(result);
            Assert.AreEqual("Reddit", result.Provider);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithoutAuthor()
        {
            IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser> sut = CreateSut();

            IRedditAuthenticatedUser redditAuthenticatedUser = BuildRedditAuthenticatedUser();
            DashboardItemExportModel result = await sut.ExportAsync(redditAuthenticatedUser);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Author);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithoutSourceUrl()
        {
            IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser> sut = CreateSut();

            IRedditAuthenticatedUser redditAuthenticatedUser = BuildRedditAuthenticatedUser();
            DashboardItemExportModel result = await sut.ExportAsync(redditAuthenticatedUser);

            Assert.IsNotNull(result);
            Assert.IsNull(result.SourceUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsDashboardItemExportModelWithoutImageUrl()
        {
            IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser> sut = CreateSut();

            IRedditAuthenticatedUser redditAuthenticatedUser = BuildRedditAuthenticatedUser();
            DashboardItemExportModel result = await sut.ExportAsync(redditAuthenticatedUser);

            Assert.IsNotNull(result);
            Assert.IsNull(result.ImageUrl);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser> sut = CreateSut();

            IRedditAuthenticatedUser redditAuthenticatedUser = BuildRedditAuthenticatedUser();
            DashboardItemExportModel result = await sut.ExportAsync(redditAuthenticatedUser);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        private IModelExporter<DashboardItemExportModel, IRedditAuthenticatedUser> CreateSut()
        {
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<AggregateException>()))
                .Returns(Task.CompletedTask);
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.CompletedTask);

            return new Web.Factories.RedditAuthenticatedUserModelExporter(_exceptionHandlerMock.Object);
        }

        private IRedditAuthenticatedUser BuildRedditAuthenticatedUser(string fullName = null, DateTime? createdTime = null, string userName = null)
        {
            return BuildRedditAuthenticatedUserMock(fullName, createdTime, userName).Object;
        }

        private Mock<IRedditAuthenticatedUser> BuildRedditAuthenticatedUserMock(string fullName = null, DateTime? createdTime = null, string userName = null)
        {
            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = new Mock<IRedditAuthenticatedUser>();
            redditAuthenticatedUserMock.Setup(m => m.FullName)
                .Returns(fullName ?? Guid.NewGuid().ToString("D"));
            redditAuthenticatedUserMock.Setup(m => m.CreatedTime)
                .Returns(createdTime ?? DateTime.Now.AddSeconds(_random.Next(300, 3600) * -1));
            redditAuthenticatedUserMock.Setup(m => m.UserName)
                .Returns(userName ?? Guid.NewGuid().ToString("D"));
            return redditAuthenticatedUserMock;
        }
    }
}