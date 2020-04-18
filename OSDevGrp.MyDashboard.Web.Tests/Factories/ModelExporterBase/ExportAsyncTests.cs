using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.ModelExporterBase
{
    [TestClass]
    public class ExportAsyncTests
    {
        #region Private variables

        private Mock<IExceptionHandler> _exceptionHandlerMock;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
        }

        [TestMethod]
        [ExpectedArgumentNullException("input")]
        public async Task ExportAsync_WhenInputIsNull_ThrowsArgumentNullException()
        {
            IModelExporter<IExportModel, object> sut = CreateSut();

            await sut.ExportAsync(null);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertDoExportAsyncWasCalledOnModelExporter()
        {
            IModelExporter<IExportModel, object> sut = CreateSut();

            object input = new object();
            await sut.ExportAsync(input);

            MyModelExporter myModelExporter = sut as MyModelExporter;
            Assert.IsNotNull(myModelExporter);
            Assert.IsTrue(myModelExporter.DoExportAsyncHasBeenCalled);
            Assert.AreEqual(input, myModelExporter.DoExportAsyncArgument);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            IModelExporter<IExportModel, object> sut = CreateSut();

            object input = new object();
            await sut.ExportAsync(input);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public async Task ExportAsync_WhenCalled_ReturnsResultFromDoExportAsyncOnModelExporter()
        {
            IExportModel exportModel = new Mock<IExportModel>().Object;
            IModelExporter<IExportModel, object> sut = CreateSut(exportModel);

            IExportModel result = await sut.ExportAsync(new object());

            Assert.AreEqual(exportModel, result);
        }

        [TestMethod]
        public async Task ExportAsync_WhenAggregateExceptionWasThrown_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            AggregateException exception = new AggregateException();
            IModelExporter<IExportModel, object> sut = CreateSut(exception: exception);

            object input = new object();
            await sut.ExportAsync(input);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<AggregateException>(value => value == exception)), Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenAggregateExceptionWasThrown_AssertEmptyWasCalledOnModelExporter()
        {
            AggregateException exception = new AggregateException();
            IModelExporter<IExportModel, object> sut = CreateSut(exception: exception);

            object input = new object();
            await sut.ExportAsync(input);

            MyModelExporter myModelExporter = sut as MyModelExporter;
            Assert.IsNotNull(myModelExporter);
            Assert.IsTrue(myModelExporter.EmptyHasBeenCalled);
        }

        [TestMethod]
        public async Task ExportAsync_WhenAggregateExceptionWasThrown_ReturnsResultFromEmptyOnModelExporter()
        {
            IExportModel emptyExportModel = new Mock<IExportModel>().Object;
            AggregateException exception = new AggregateException();
            IModelExporter<IExportModel, object> sut = CreateSut(emptyExportModel: emptyExportModel, exception: exception);

            IExportModel result = await sut.ExportAsync(new object());

            Assert.AreEqual(emptyExportModel, result);
        }

        [TestMethod]
        public async Task ExportAsync_WhenExceptionWasThrown_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            Exception exception = new Exception();
            IModelExporter<IExportModel, object> sut = CreateSut(exception: exception);

            object input = new object();
            await sut.ExportAsync(input);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(value => value == exception)), Times.Once);
        }

        [TestMethod]
        public async Task ExportAsync_WhenExceptionWasThrown_AssertEmptyWasCalledOnModelExporter()
        {
            Exception exception = new Exception();
            IModelExporter<IExportModel, object> sut = CreateSut(exception: exception);

            object input = new object();
            await sut.ExportAsync(input);

            MyModelExporter myModelExporter = sut as MyModelExporter;
            Assert.IsNotNull(myModelExporter);
            Assert.IsTrue(myModelExporter.EmptyHasBeenCalled);
        }

        [TestMethod]
        public async Task ExportAsync_WhenExceptionWasThrown_ReturnsResultFromEmptyOnModelExporter()
        {
            IExportModel emptyExportModel = new Mock<IExportModel>().Object;
            Exception exception = new Exception();
            IModelExporter<IExportModel, object> sut = CreateSut(emptyExportModel: emptyExportModel, exception: exception);

            IExportModel result = await sut.ExportAsync(new object());

            Assert.AreEqual(emptyExportModel, result);
        }

        private IModelExporter<IExportModel, object> CreateSut(IExportModel exportModel = null, IExportModel emptyExportModel = null, Exception exception = null)
        {
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<AggregateException>()))
                .Returns(Task.CompletedTask);
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.CompletedTask);

            return new MyModelExporter(_exceptionHandlerMock.Object, exportModel ?? new Mock<IExportModel>().Object, emptyExportModel ?? new Mock<IExportModel>().Object, exception);
        }

        private class MyModelExporter : Web.Factories.ModelExporterBase<IExportModel, object>
        {
            #region Private variables

            private readonly IExportModel _exportModel;
            private readonly IExportModel _emptyExportModel;
            private readonly Exception _exception;

            #endregion

            #region Constructor

            public MyModelExporter(IExceptionHandler exceptionHandler, IExportModel exportModel, IExportModel emptyExportModel, Exception exception)
                : base(exceptionHandler)
            {
                if (exportModel == null)
                {
                    throw new ArgumentNullException(nameof(exportModel));
                }
                if (emptyExportModel == null)
                {
                    throw new ArgumentNullException(nameof(emptyExportModel));
                }

                _exportModel = exportModel;
                _emptyExportModel = emptyExportModel;
                _exception = exception;
            }

            #endregion

            #region Properties

            public bool DoExportAsyncHasBeenCalled { get; private set; }

            public object DoExportAsyncArgument { get; private set;}

            public bool EmptyHasBeenCalled { get; private set; }

            #endregion

            #region Methods

            protected override Task<IExportModel> DoExportAsync(object input)
            {
                if (input == null)
                {
                    throw new ArgumentNullException(nameof(input));
                }

                if (_exception != null)
                {
                    throw _exception;
                }

                DoExportAsyncHasBeenCalled = true;
                DoExportAsyncArgument = input;

                return Task.FromResult(_exportModel);
            }

            protected override IExportModel Empty()
            {
                EmptyHasBeenCalled = true;

                return _emptyExportModel;
            }

            #endregion
        }
    }
}