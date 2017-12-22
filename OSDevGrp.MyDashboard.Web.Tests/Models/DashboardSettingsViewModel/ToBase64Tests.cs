using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Models.DashboardSettingsViewModel
{
    [TestClass]
    public class ToBase64Tests
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
        public void ToBase64_WhenCalled_ReturnsBase64()
        {
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut();

            string result = sut.ToBase64();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Trim().Length % 4 == 0 && Regex.IsMatch(result.Trim(), @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None));
        }
 
        private OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel
            {
                NumberOfNews = _random.Next(25, 50),
                UseReddit = _random.Next(100) > 50,
                RedditAccessToken = _random.Next(100) > 50 ? Guid.NewGuid().ToString("D") : null
            };
        }
    }
}