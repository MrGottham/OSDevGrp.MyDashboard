using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Models.DashboardSettingsViewModel
{
    [TestClass]
    public class ToDashboardSettingsTests
    {
        #region Private variables

        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _random = new Random(DateTime.Now.Millisecond);
        }
        
        [TestMethod]
        public void ToDashboardSettings_WhenCalled_ReturnsInitializedDashboardSettings()
        {
            int numberOfNews = _random.Next(25, 50);
            bool useReddit = _random.Next(100) > 50;
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut(numberOfNews, useReddit);

            IDashboardSettings result = sut.ToDashboardSettings();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DashboardSettings));
            Assert.AreEqual(numberOfNews, result.NumberOfNews);
            Assert.AreEqual(useReddit, result.UseReddit);
        }

        private OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel CreateSut(int? numberOfNews = null, bool? useReddit = null)
        {
            return new OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel
            {
                NumberOfNews = numberOfNews ?? _random.Next(25, 50),
                UseReddit = useReddit ?? _random.Next(100) > 50
            };
        }
    }
}