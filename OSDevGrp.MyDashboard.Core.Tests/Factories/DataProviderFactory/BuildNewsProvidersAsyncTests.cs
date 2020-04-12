using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;

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
        public async Task BuildNewsProvidersAsync_WhenCalled_ReturnsNewsProviders()
        {
            IDataProviderFactory sut = CreateSut();

            IEnumerable<INewsProvider> newsProviders = await sut.BuildNewsProvidersAsync();

            Assert.IsNotNull(newsProviders);
            Assert.AreEqual(5, newsProviders.Count());
            
            AssertNewsProvider(newsProviders.ElementAt(0), "DR", "https://www.dr.dk/nyheder/service/feeds/allenyheder");
            AssertNewsProvider(newsProviders.ElementAt(1), "TV 2 Lorry", "https://www.tv2lorry.dk/rss");
            AssertNewsProvider(newsProviders.ElementAt(2), "Børsen", "https://borsen.dk/rss");
            AssertNewsProvider(newsProviders.ElementAt(3), "Computerworld", "https://www.computerworld.dk/rss/all");
            AssertNewsProvider(newsProviders.ElementAt(4), "Version2", "https://www.version2.dk/it-nyheder/rss");
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