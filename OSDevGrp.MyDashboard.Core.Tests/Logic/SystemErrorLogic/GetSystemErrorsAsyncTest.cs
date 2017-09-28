using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;

namespace OSDevGrp.MyDashboard.Core.Tests.Logic.SystemErrorLogic
{
    [TestClass]
    public class GetSystemErrorsAsyncTests
    {
        #region Private variables

        private Mock<IExceptionRepository> _exceptionRepositoryMock;
        private Mock<IExceptionHandler> _exceptionHandlerMock;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _exceptionRepositoryMock = new Mock<IExceptionRepository>();
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
        }

        [TestMethod]
        public void GetSystemErrorsAsync_WhenCalled_ExpectNoError()
        {
            ISystemErrorLogic sut = CreateSut();

            Task<IEnumerable<ISystemError>> getNewsTask = sut.GetSystemErrorsAsync();
            getNewsTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void GetSystemErrorsAsync_WhenCalled_AssertGetSystemErrorsAsyncWasCalledOnExceptionRepository()
        {
            ISystemErrorLogic sut = CreateSut();

            Task<IEnumerable<ISystemError>> getNewsTask = sut.GetSystemErrorsAsync();
            getNewsTask.Wait();

            _exceptionRepositoryMock.Verify(m => m.GetSystemErrorsAsync(), Times.Once);
        }

        [TestMethod]
        public void GetSystemErrorsAsync_WhenCalledAndGetSystemErrorsAsyncOnExceptionRepositoryReturnsNull_ReturnsEmptyCollection()
        {
            ISystemErrorLogic sut = CreateSut(getSystemErrorsAsyncReturnsNull: true);

            Task<IEnumerable<ISystemError>> getNewsTask = sut.GetSystemErrorsAsync();
            getNewsTask.Wait();

            IEnumerable<ISystemError> result = getNewsTask.Result;
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetSystemErrorsAsync_WhenCalledAndGetSystemErrorsAsyncOnExceptionRepositoryReturnsEmptyCollection_ReturnsEmptyCollection()
        {
            IEnumerable<ISystemError> systemErrors = BuildSystemErrors(numberOfSystemErrors: 0);
            ISystemErrorLogic sut = CreateSut(systemErrors: systemErrors);

            Task<IEnumerable<ISystemError>> getNewsTask = sut.GetSystemErrorsAsync();
            getNewsTask.Wait();

            IEnumerable<ISystemError> result = getNewsTask.Result;
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetSystemErrorsAsync_WhenCalledAndGetSystemErrorsAsyncOnExceptionRepositoryReturnsCollection_ReturnsEmptyCollection()
        {
            IEnumerable<ISystemError> systemErrors = BuildSystemErrors(numberOfSystemErrors: 100);
            ISystemErrorLogic sut = CreateSut(systemErrors: systemErrors);

            Task<IEnumerable<ISystemError>> getNewsTask = sut.GetSystemErrorsAsync();
            getNewsTask.Wait();

            IEnumerable<ISystemError> result = getNewsTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(100, result.Count());
            Assert.IsTrue(result.All(systemError => systemErrors.Contains(systemError)));
        }

        [TestMethod]
        public void GetSystemErrorsAsync_WhenCalledAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            Exception exception = new Exception();
            ISystemErrorLogic sut =  CreateSut(provokedException: exception);

            Task<IEnumerable<ISystemError>> getNewsTask = sut.GetSystemErrorsAsync();
            getNewsTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(ex => ex == exception)), Times.Once);
        }

        [TestMethod]
        public void GetSystemErrorsAsync_WhenCalledAndExceptionOccurs_ReturnsEmptyCollection()
        {
            Exception exception = new Exception();
            ISystemErrorLogic sut =  CreateSut(provokedException: exception);

            Task<IEnumerable<ISystemError>> getNewsTask = sut.GetSystemErrorsAsync();
            getNewsTask.Wait();

            IEnumerable<ISystemError> result = getNewsTask.Result;
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        private ISystemErrorLogic CreateSut(IEnumerable<ISystemError> systemErrors = null, bool getSystemErrorsAsyncReturnsNull = false, Exception provokedException = null)
        {
            if (provokedException == null)
            {
                _exceptionRepositoryMock.Setup(m => m.GetSystemErrorsAsync())
                    .Returns(Task.Run<IEnumerable<ISystemError>>(() => systemErrors ?? BuildSystemErrors(getSystemErrorsAsyncReturnsNull)));
            }
            else
            {
                _exceptionRepositoryMock.Setup(m => m.GetSystemErrorsAsync())
                    .Throws(provokedException);
            }
            
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.Run(() => { }));

            return new OSDevGrp.MyDashboard.Core.Logic.SystemErrorLogic(
                _exceptionRepositoryMock.Object,
                _exceptionHandlerMock.Object);
        }

        private IEnumerable<ISystemError> BuildSystemErrors(bool shouldReturnNull = false, int numberOfSystemErrors = 7)
        {
            if (shouldReturnNull)
            {
                return null;
            }

            IList<ISystemError> systemErrors = new List<ISystemError>(numberOfSystemErrors);
            while (systemErrors.Count < numberOfSystemErrors)
            {
                Mock<ISystemError> systemErrorMock = new Mock<ISystemError>();
                systemErrorMock.Setup(m => m.Timestamp)
                    .Returns(DateTime.Now);
                systemErrors.Add(systemErrorMock.Object);
            }
            return systemErrors;
        }
    }
}