using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Factories;

namespace OSDevGrp.MyDashboard.Core.Tests.Factories.DataProviderFactory
{
    [TestClass]
    public class GetNewsProvidersAsyncTests
    {
        [TestMethod]
        public void GetNewsProvidersAsync_WhenCalled_ReturnsNewsProviders()
        {
            IDataProviderFactory sut = CreateSut();

            Task<IEnumerable<INewsProvider>> newsProvidersTask = sut.GetNewsProvidersAsync();
            newsProvidersTask.Wait();

            Assert.IsNotNull(newsProvidersTask.Result);
        }

        private IDataProviderFactory CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Factories.DataProviderFactory();
        }
    }
}
