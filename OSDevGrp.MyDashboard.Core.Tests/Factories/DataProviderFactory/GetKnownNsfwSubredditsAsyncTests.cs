using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Tests.Factories.DataProviderFactory
{
    [TestClass]
    public class GetKnownNsfwSubredditsAsyncTests
    {
        [TestMethod]
        public void GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsGoneWildCurvy()
        {
            GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("gonewildcurvy");  
        }

        [TestMethod]
        public void GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsGoneWildPlus()
        {
            GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("gonewildplus");  
        }

        [TestMethod]
        public void GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsBigBoobsGW()
        {
            GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("bigboobsgw");  
        }

        [TestMethod]
        public void GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsHomeGrownTits()
        {
            GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("homegrowntits");  
        }

        [TestMethod]
        public void GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsMilf()
        {
            GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("milf");  
        }

        [TestMethod]
        public void GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsGoneWild30Plus()
        {
            GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("gonewild30plus");  
        }

        [TestMethod]
        public void GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsOhNoMomWentWild()
        {
            GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("onmww");  
        }

        [TestMethod]
        public void GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsWouldYouFuckMyWife()
        {
            GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("wouldyoufuckmywife");  
        }

        [TestMethod]
        public void GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsWifeSharing()
        {
            GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("wifesharing");  
        }

        [TestMethod]
        public void GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsGifsGoneWild()
        {
            GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("gifsgonewild");  
        }

        [TestMethod]
        public void GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsGwNerdy()
        {
            GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("gwnerdy");  
        }

        [TestMethod]
        public void GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsChubby()
        {
            GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName("chubby");  
        }

        private void GetKnownNsfwSubredditsAsync_WhenCalled_ExpectKnownNsfwSubredditsContainsName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            IDataProviderFactory sut = CreateSut();

            Task<IEnumerable<IRedditKnownSubreddit>> getKnownNsfwSubredditsTask = sut.GetKnownNsfwSubredditsAsync();
            getKnownNsfwSubredditsTask.Wait();

            IEnumerable<IRedditKnownSubreddit> knownNsfwSubreddits = getKnownNsfwSubredditsTask.Result;
            Assert.IsNotNull(knownNsfwSubreddits);
            Assert.AreEqual(12, knownNsfwSubreddits.Count());

            IRedditKnownSubreddit knownNsfwSubreddit = knownNsfwSubreddits.SingleOrDefault(m => string.Compare(m.Name, name, false) == 0);
            Assert.IsNotNull(knownNsfwSubreddit);
            Assert.IsTrue(knownNsfwSubreddit.Rank >= 0 && knownNsfwSubreddit.Rank <= 1000);
        }

        private IDataProviderFactory CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Factories.DataProviderFactory();
        }
    }
}