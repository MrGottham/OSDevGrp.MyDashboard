using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Repositories.ExceptionRepository
{
    [TestClass]
    public class AddAsyncTests
    {
        [TestMethod]
        [ExpectedArgumentNullException("exception")]
        public void AddAsync_WhenExceptionIsNull_ThrowsArgumentNullException()
        {
            Exception exception = null;

            IExceptionRepository sut = CreateSut();
            
            sut.AddAsync(exception);
        }

        [TestMethod]
        public void AddAsync_WhenCalled_ExpectNoError()
        {
            Exception exception = new Exception();

            IExceptionRepository sut = CreateSut();
            
            Task addTask = sut.AddAsync(exception);
            addTask.Wait();
        }

        private IExceptionRepository CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Repositories.ExceptionRepository();
        }
    }
}
