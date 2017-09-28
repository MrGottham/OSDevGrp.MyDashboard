using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;

namespace OSDevGrp.MyDashboard.Core.Tests.Repositories.ExceptionRepository
{
    [TestClass]
    public class GetSystemErrorsAsyncTests
    {
        [TestMethod]
        public void GetSystemErrorsAsyncTests_WhenCalled_ReturnsSystemErrors()
        {
            Exception firstException = new Exception(Guid.NewGuid().ToString("D"));
            Exception secondException = new Exception(Guid.NewGuid().ToString("D"));
            Exception thirdException = new Exception(Guid.NewGuid().ToString("D"));

            IExceptionRepository sut = CreateSut();
            
            sut.AddAsync(firstException).Wait();
            sut.AddAsync(secondException).Wait();
            sut.AddAsync(thirdException).Wait();

            Task<IEnumerable<ISystemError>> getSystemErrorsTask = sut.GetSystemErrorsAsync();
            getSystemErrorsTask.Wait();

            Assert.IsNotNull(getSystemErrorsTask.Result);
            Assert.IsTrue(getSystemErrorsTask.Result.Any());
            Assert.IsNotNull(getSystemErrorsTask.Result.FirstOrDefault(systemError => string.Compare(systemError.Information, firstException.Message, StringComparison.InvariantCulture) == 0));
            Assert.IsNotNull(getSystemErrorsTask.Result.FirstOrDefault(systemError => string.Compare(systemError.Information, secondException.Message, StringComparison.InvariantCulture) == 0));
            Assert.IsNotNull(getSystemErrorsTask.Result.FirstOrDefault(systemError => string.Compare(systemError.Information, thirdException.Message, StringComparison.InvariantCulture) == 0));
        }

        [TestMethod]
        public void GetSystemErrorsAsyncTests_WhenCalled_RemoveSystemErrors()
        {
            Exception firstException = new Exception();
            Exception secondException = new Exception();
            Exception thirdException = new Exception();

            IExceptionRepository sut = CreateSut();
            
            sut.AddAsync(firstException).Wait();
            sut.AddAsync(secondException).Wait();
            sut.AddAsync(thirdException).Wait();

            Task<IEnumerable<ISystemError>> firstGetSystemErrorsTask = sut.GetSystemErrorsAsync();
            firstGetSystemErrorsTask.Wait();

            Assert.IsNotNull(firstGetSystemErrorsTask.Result);
            Assert.IsTrue(firstGetSystemErrorsTask.Result.Any());

            Task<IEnumerable<ISystemError>> secondGetSystemErrorsTask = sut.GetSystemErrorsAsync();
            secondGetSystemErrorsTask.Wait();

            Assert.IsNotNull(secondGetSystemErrorsTask.Result);
            Assert.IsFalse(secondGetSystemErrorsTask.Result.Any());
        }

        private IExceptionRepository CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Repositories.ExceptionRepository();
        }
    }
}
