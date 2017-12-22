using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Web.Tests.Models.DashboardSettingsViewModel
{
    [TestClass]
    public class CreateTests
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
        [ExpectedArgumentNullException("base64")]
        public void Create_WhenBase64IsNull_ThrowsArgumentNullException()
        {
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(null);
        }
        
        [TestMethod]
        [ExpectedArgumentNullException("base64")]
        public void Create_WhenBase64IsEmpty_ThrowsArgumentNullException()
        {
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(string.Empty);
        }
        
        [TestMethod]
        [ExpectedArgumentNullException("base64")]
        public void Create_WhenBase64IsWhitespace_ThrowsArgumentNullException()
        {
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(" ");
        }
        
        [TestMethod]
        [ExpectedArgumentNullException("base64")]
        public void Create_WhenBase64IsWhitespaces_ThrowsArgumentNullException()
        {
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create("  ");
        }
        
        [TestMethod]
        public void Create_WhenCalled_ReturnsDashboardSettingsViewModel()
        {
            int numberOfNews = _random.Next(25, 50);
            bool useReddit = _random.Next(100) > 50;
            string redditAccessToken = _random.Next(100) > 50 ? Guid.NewGuid().ToString("D") : null;
            string base64 = CreateSut(numberOfNews, useReddit, redditAccessToken).ToBase64();

            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel result = OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel.Create(base64);
            Assert.IsNotNull(result);
            Assert.AreEqual(numberOfNews, result.NumberOfNews);
            Assert.AreEqual(useReddit, result.UseReddit);
            Assert.AreEqual(redditAccessToken, result.RedditAccessToken);
        }
        
        private OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel CreateSut(int? numberOfNews = null, bool? useReddit = null, string redditAccessToken = null)
        {
            return new OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel
            {
                NumberOfNews = numberOfNews ?? _random.Next(25, 50),
                UseReddit = useReddit ?? _random.Next(100) > 50,
                RedditAccessToken = redditAccessToken
            };
        }
    }
}