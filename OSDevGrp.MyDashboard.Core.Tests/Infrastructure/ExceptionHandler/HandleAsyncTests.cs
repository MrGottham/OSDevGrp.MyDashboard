using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Infrastructure.ExceptionHandler
{
    [TestClass]
    public class HandleAsyncTests
    {
        #region Private variables

        private Mock<IExceptionRepository> _exceptionRepositoryMock;
        private Mock<ILoggerFactory> _loggerFactoryMock;
        private Mock<ILogger<Core.Infrastructure.ExceptionHandler>> _loggerMock;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _exceptionRepositoryMock = new Mock<IExceptionRepository>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _loggerMock = new Mock<ILogger<Core.Infrastructure.ExceptionHandler>>();
        }

        [TestMethod]
        [ExpectedArgumentNullException("exception")]
        public async Task HandleAsync_WhenExceptionIsNull_ThrowsArgumentNullException()
        {
            Exception exception = null;

            IExceptionHandler sut = CreateSut();

            await sut.HandleAsync(exception);
        }

        [TestMethod]
        public async Task HandleAsync_WhenCalledWithException_AssertAddAsyncWasCalledOnExceptionRepository()
        {
            Exception exception = new Exception();

            IExceptionHandler sut = CreateSut();

            await sut.HandleAsync(exception);

            _exceptionRepositoryMock.Verify(m => m.AddAsync(It.Is<Exception>(ex => ex == exception)), Times.Once);
        }

        [TestMethod]
        public async Task HandleAsync_WhenCalledWithException_AssertCreateLoggerWasCalledOnLoggerFactory()
        {
            Exception exception = new Exception();

            IExceptionHandler sut = CreateSut();

            await sut.HandleAsync(exception);

            string methodNamespace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;

            _loggerFactoryMock.Verify(m => m.CreateLogger(It.Is<string>(value => string.CompareOrdinal(value, methodNamespace) == 0)), Times.Once);
        }

        [TestMethod]
        public async Task HandleAsync_WhenCalledWithException_AssertLogWasCalledOnCreatedLogger()
        {
            string exceptionMessage = Guid.NewGuid().ToString("D");
            Exception exception = new Exception(exceptionMessage);

            IExceptionHandler sut = CreateSut();

            await sut.HandleAsync(exception);

            string methodName = MethodBase.GetCurrentMethod().Name;

            _loggerMock.Verify(m => m.Log(
                    It.Is<LogLevel>(value => value == LogLevel.Error),
                    It.Is<EventId>(value => value.Id == 0 && string.IsNullOrWhiteSpace(value.Name)),
                    It.Is<It.IsAnyType>((value, type) => value.ToString() == $"{methodName}: {exceptionMessage}"),
                    It.Is<Exception>(value => value == exception),
                    It.Is<Func<It.IsAnyType, Exception, string>>((value, type) => true)),
                Times.Once);
        }

        [TestMethod]
        [ExpectedArgumentNullException("exception")]
        public async Task HandleAsync_WhenAggregateExceptionIsNull_ThrowsArgumentNullException()
        {
            AggregateException aggregateException = null;

            IExceptionHandler sut = CreateSut();

            await sut.HandleAsync(aggregateException);
        }

        [TestMethod]
        public async Task HandleAsync_WhenCalledWithAggregateException_AssertAddAsyncWasCalledOnExceptionRepository()
        {
            Exception exception = new Exception();
            AggregateException aggregateException = new AggregateException(new [] {exception});

            IExceptionHandler sut = CreateSut();

            await sut.HandleAsync(aggregateException);

            _exceptionRepositoryMock.Verify(m => m.AddAsync(It.Is<Exception>(ex => ex == exception)), Times.Once);
        }

        [TestMethod]
        public async Task HandleAsync_WhenCalledWithAggregateException_AssertCreateLoggerWasCalledOnLoggerFactory()
        {
            Exception exception = new Exception();
            AggregateException aggregateException = new AggregateException(new [] {exception});

            IExceptionHandler sut = CreateSut();

            await sut.HandleAsync(aggregateException);

            string methodNamespace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;

            _loggerFactoryMock.Verify(m => m.CreateLogger(It.Is<string>(value => string.CompareOrdinal(value, methodNamespace) == 0)), Times.Once);
        }

        [TestMethod]
        public async Task HandleAsync_WhenCalledWithAggregateException_AssertLogWasCalledOnCreatedLogger()
        {
            string exceptionMessage = Guid.NewGuid().ToString("D");
            Exception exception = new Exception(exceptionMessage);
            AggregateException aggregateException = new AggregateException(new [] {exception});

            IExceptionHandler sut = CreateSut();

            await sut.HandleAsync(aggregateException);

            string methodName = MethodBase.GetCurrentMethod().Name;

            _loggerMock.Verify(m => m.Log(
                    It.Is<LogLevel>(value => value == LogLevel.Error),
                    It.Is<EventId>(value => value.Id == 0 && string.IsNullOrWhiteSpace(value.Name)),
                    It.Is<It.IsAnyType>((value, type) => value.ToString() == $"{methodName}: {exceptionMessage}"),
                    It.Is<Exception>(value => value == exception),
                    It.Is<Func<It.IsAnyType, Exception, string>>((value, type) => true)),
                Times.Once);
        }

        private IExceptionHandler CreateSut()
        {
            _loggerFactoryMock.Setup(m => m.CreateLogger(It.IsAny<string>()))
                .Returns(_loggerMock.Object);

            return new Core.Infrastructure.ExceptionHandler(_exceptionRepositoryMock.Object, _loggerFactoryMock.Object);
        }
    }
}