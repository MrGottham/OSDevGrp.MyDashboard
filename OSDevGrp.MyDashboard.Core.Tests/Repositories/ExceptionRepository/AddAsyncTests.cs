using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;

namespace OSDevGrp.MyDashboard.Core.Tests.Repositories.ExceptionRepository
{
    [TestClass]
    public class AddAsyncTests
    {
        [TestMethod]
        public async Task AddAsync_WhenExceptionIsNull_ThrowsArgumentNullException()
        {
            Exception exception = null;

            IExceptionRepository sut = CreateSut();

            ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.AddAsync(exception));
            
            Assert.AreEqual("exception", result.ParamName);
        }

        [TestMethod]
        public async Task AddAsync_WhenCalled_ExpectNoError()
        {
            Exception exception = new Exception();

            IExceptionRepository sut = CreateSut();

            await sut.AddAsync(exception);
        }

        private IExceptionRepository CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Repositories.ExceptionRepository();
        }
    }
}