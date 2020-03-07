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
        public async Task GetSystemErrorsAsyncTests_WhenCalled_ReturnsSystemErrors()
        {
            Exception firstException = new Exception(Guid.NewGuid().ToString("D"));
            Exception secondException = new Exception(Guid.NewGuid().ToString("D"));
            Exception thirdException = new Exception(Guid.NewGuid().ToString("D"));

            IExceptionRepository sut = CreateSut();
            
            await sut.AddAsync(firstException);
            await sut.AddAsync(secondException);
            await sut.AddAsync(thirdException);

            IEnumerable<ISystemError> result = await sut.GetSystemErrorsAsync();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
            Assert.IsNotNull(result.FirstOrDefault(systemError => string.Compare(systemError.Information, firstException.Message, StringComparison.InvariantCulture) == 0));
            Assert.IsNotNull(result.FirstOrDefault(systemError => string.Compare(systemError.Information, secondException.Message, StringComparison.InvariantCulture) == 0));
            Assert.IsNotNull(result.FirstOrDefault(systemError => string.Compare(systemError.Information, thirdException.Message, StringComparison.InvariantCulture) == 0));
        }

        [TestMethod]
        public async Task GetSystemErrorsAsyncTests_WhenCalled_RemoveSystemErrors()
        {
            Exception firstException = new Exception();
            Exception secondException = new Exception();
            Exception thirdException = new Exception();

            IExceptionRepository sut = CreateSut();
            
            await sut.AddAsync(firstException);
            await sut.AddAsync(secondException);
            await sut.AddAsync(thirdException);

            IEnumerable<ISystemError> firstGetSystemErrors = await sut.GetSystemErrorsAsync();

            Assert.IsNotNull(firstGetSystemErrors);
            Assert.IsTrue(firstGetSystemErrors.Any());

            IEnumerable<ISystemError> secondGetSystemErrors = await sut.GetSystemErrorsAsync();

            Assert.IsNotNull(secondGetSystemErrors);
            Assert.IsFalse(secondGetSystemErrors.Any());
        }

        private IExceptionRepository CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Repositories.ExceptionRepository();
        }
    }
}
