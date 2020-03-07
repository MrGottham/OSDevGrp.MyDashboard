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

            _exceptionRepositoryMock.Verify(m => m.AddAsync(It.Is<Exception>(ex => ex == exception)));
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

            _exceptionRepositoryMock.Verify(m => m.AddAsync(It.Is<Exception>(ex => ex == exception)));
        }

        private IExceptionHandler CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Infrastructure.ExceptionHandler(_exceptionRepositoryMock.Object);
        }
    }
}
