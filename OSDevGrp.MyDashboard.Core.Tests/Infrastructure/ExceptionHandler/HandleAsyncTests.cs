using System;
using System.Threading.Tasks;
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

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _exceptionRepositoryMock = new Mock<IExceptionRepository>();
        }

        [TestMethod]
        [ExpectedArgumentNullException("exception")]
        public void HandleAsync_WhenExceptionIsNull_ThrowsArgumentNullException()
        {
            Exception exception = null;

            IExceptionHandler sut = CreateSut();
            
            sut.HandleAsync(exception);
        }

        [TestMethod]
        public void HandleAsync_WhenCalledWithException_AssertAddAsyncWasCalledOnExceptionRepository()
        {
            Exception exception = new Exception();

            IExceptionHandler sut = CreateSut();
            
            Task handleTask = sut.HandleAsync(exception);
            handleTask.Wait();

            _exceptionRepositoryMock.Verify(m => m.AddAsync(It.Is<Exception>(ex => ex == exception)));
        }

        [TestMethod]
        [ExpectedArgumentNullException("exception")]
        public void HandleAsync_WhenAggregateExceptionIsNull_ThrowsArgumentNullException()
        {
            AggregateException aggregateException = null;

            IExceptionHandler sut = CreateSut();
            
            sut.HandleAsync(aggregateException);
        }

        [TestMethod]
        public void HandleAsync_WhenCalledWithAggregateException_AssertAddAsyncWasCalledOnExceptionRepository()
        {
            Exception exception = new Exception();
            AggregateException aggregateException = new AggregateException(new [] {exception});
            
            IExceptionHandler sut = CreateSut();
            
            Task handleTask = sut.HandleAsync(aggregateException);
            handleTask.Wait();

            _exceptionRepositoryMock.Verify(m => m.AddAsync(It.Is<Exception>(ex => ex == exception)));
        }

        private IExceptionHandler CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Infrastructure.ExceptionHandler(_exceptionRepositoryMock.Object);
        }
    }
}
