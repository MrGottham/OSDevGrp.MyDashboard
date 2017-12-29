using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OSDevGrp.MyDashboard.Core.Tests.Repositories.RedditRepository
{
    [TestClass]
    public class UserAgentTests
    {
        [TestMethod]
        public void UserAgent_WhenCalled_ReturnsUserAgent()
        {
            string result = OSDevGrp.MyDashboard.Core.Repositories.RedditRepository.UserAgent;
            Assert.AreEqual("windows:osdevgrp.mydashboard:v0.1.0 (by /u/mrgottham)", result);
        }
    }
}