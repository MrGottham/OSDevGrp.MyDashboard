using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Factories;

namespace OSDevGrp.MyDashboard.Core.Tests.Factories.DataProviderFactory
{
    [TestClass]
    public class BuildNewsProvidersAsyncTests
    {
        #region Private variables

        private Mock<IRandomizer> _randomizerMock;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _randomizerMock = new Mock<IRandomizer>();
        }

        [TestMethod]
        public void BuildNewsProvidersAsync_WhenCalled_ReturnsNewsProviders()
        {
            IDataProviderFactory sut = CreateSut();

            Task<IEnumerable<INewsProvider>> newsProvidersTask = sut.BuildNewsProvidersAsync();
            newsProvidersTask.Wait();

            Assert.IsNotNull(newsProvidersTask.Result);
            Assert.AreEqual(5, newsProvidersTask.Result.Count());
            
            AssertNewsProvider(newsProvidersTask.Result.ElementAt(0), "DR", "http://www.dr.dk/nyheder/service/feeds/allenyheder");
            AssertNewsProvider(newsProvidersTask.Result.ElementAt(1), "TV 2", "http://feeds.tv2.dk/nyheder/rss");
            AssertNewsProvider(newsProvidersTask.Result.ElementAt(2), "Børsen", "http://borsen.dk/rss");
            AssertNewsProvider(newsProvidersTask.Result.ElementAt(3), "Computerworld", "https://www.computerworld.dk/rss/all");
            AssertNewsProvider(newsProvidersTask.Result.ElementAt(4), "Version2", "https://www.version2.dk/it-nyheder/rss");
        }

        private IDataProviderFactory CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Factories.DataProviderFactory(_randomizerMock.Object);
        }

        private void AssertNewsProvider(INewsProvider newsProvider, string name, string url)
        {
            Assert.IsNotNull(newsProvider);
            Assert.IsNotNull(newsProvider.Name);
            Assert.AreEqual(name, newsProvider.Name);
            Assert.IsNotNull(newsProvider.Uri);
            Assert.AreEqual(url, newsProvider.Uri.ToString());
        }
    }
}