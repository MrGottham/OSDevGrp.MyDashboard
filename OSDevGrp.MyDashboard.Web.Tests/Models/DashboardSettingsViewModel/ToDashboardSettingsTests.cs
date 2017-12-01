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
        public void ToDashboardSettings_WhenCalled_ReturnsInitializedX()
        {
            int numberOfNews = _random.Next(25, 50);
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut(numberOfNews);

            IDashboardSettings result = sut.ToDashboardSettings();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DashboardSettings));
            Assert.AreEqual(numberOfNews, result.NumberOfNews);
        }

        private OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel CreateSut(int? numberOfNews = null)
        {
            return new OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel
            {
                NumberOfNews = numberOfNews ?? _random.Next(25, 50)
            };
        }
    }
}