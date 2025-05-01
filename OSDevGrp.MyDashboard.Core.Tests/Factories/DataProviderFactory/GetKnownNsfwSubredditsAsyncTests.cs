using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Core.Tests.Factories.DataProviderFactory
{
    [TestClass]
    public class GetKnownNsfwSubredditsAsyncTests
    {
        #region Private constants

        private const int NumberOfKnownNsfwSubreddits = 25;

        #endregion

        #region Private variables

        private Mock<IRandomizer> _randomizerMock;
        private Random _random;
        private int _minRank;
        private int _maxRank;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _randomizerMock = new Mock<IRandomizer>();

            _random = new Random(DateTime.Now.Millisecond);
            _minRank = _random.Next(0, 100);
            _maxRank = _minRank + _random.Next(0, 100);
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_AssertNextWasCalledOnRandomizerForEachKnownNsfwSubreddit()
        {
            IDataProviderFactory sut = CreateSut();

            await sut.GetKnownNsfwSubredditsAsync();

            _randomizerMock.Verify(m => m.Next(
                    It.Is<int>(value => value == 0),
                    It.Is<int>(value => value == 1000)),
                Times.Exactly(NumberOfKnownNsfwSubreddits));
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsGoneWildCurvy()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("gonewildcurvy");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsGoneWildPlus()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("gonewildplus");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsBigBoobsGW()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("bigboobsgw");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsHomeGrownTits()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("homegrowntits");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsMilf()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("milf");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsGoneWild30Plus()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("gonewild30plus");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContains40PlusGoneWild()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("40plusgonewild");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContains50PlusGW()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("50plusgw");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsOhNoMomWentWild()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("onmww");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsWouldYouFuckMyWife()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("wouldyoufuckmywife");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsWifeSharing()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("wifesharing");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsGifsGoneWild()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("gifsgonewild");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsGwNerdy()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("gwnerdy");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsChubby()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("chubby");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsSwingersGW()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("swingersgw");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsSaggy()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("saggy");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsOBSF()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("obsf");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsFupaWives()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("fupawives");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsBbw()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("bbw");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsRateMyBooks()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("ratemyboobs");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsSlutWife()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("slutwife");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsChurchWife()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("churchwife");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsWorkGoneWild()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("workgonewild");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsGWPublic()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("gwpublic");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsNoFans()
        {
            await GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("nofans");  
        }

        [TestMethod]
        public async Task GetKnownNsfwSubredditsAsync_WhenCalled_ReturnsSortedKnownNsfwSubreddits()
        {
            IDataProviderFactory sut = CreateSut();

            IEnumerable<IRedditKnownSubreddit> result = await sut.GetKnownNsfwSubredditsAsync();
            for (int i = 1; i < result.Count(); i++)
            {
                Assert.IsTrue(result.ElementAt(i - 1).Rank <= result.ElementAt(i).Rank);
            }
       }

        private async Task GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
 
            IDataProviderFactory sut = CreateSut();

            IEnumerable<IRedditKnownSubreddit> knownNsfwSubreddits = await sut.GetKnownNsfwSubredditsAsync();
            Assert.IsNotNull(knownNsfwSubreddits);
            Assert.AreEqual(NumberOfKnownNsfwSubreddits, knownNsfwSubreddits.Count());

            IRedditKnownSubreddit knownNsfwSubreddit = knownNsfwSubreddits.SingleOrDefault(m => string.Compare(m.Name, name, false) == 0);
            Assert.IsNotNull(knownNsfwSubreddit);
            Assert.IsTrue(knownNsfwSubreddit.Rank >= _minRank && knownNsfwSubreddit.Rank <= _maxRank);
        }

        private IDataProviderFactory CreateSut()
        {
            _randomizerMock.Setup(m => m.Next(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(_random.Next(_minRank, _maxRank));

            return new Core.Factories.DataProviderFactory(_randomizerMock.Object);
        }
    }
}