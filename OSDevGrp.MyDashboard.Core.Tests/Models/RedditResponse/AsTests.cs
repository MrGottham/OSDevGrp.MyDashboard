using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Tests.Models.RedditResponse
{
    [TestClass]
    public class AsTests
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
        public void As_WhenCalled_ExpectRedditResponseWithSameValues()
        {
            int rateLimitUsed = _random.Next(100);
            int rateLimitRemaining = _random.Next(100);
            DateTime? rateLimitResetTime = _random.Next(100) > 50 ? DateTime.Now.AddSeconds(_random.Next(300)) : (DateTime?) null;
            MyRedditObject data = new MyRedditObject();

            IRedditResponse<MyRedditObject> sut = CreateSut(rateLimitUsed: rateLimitUsed, rateLimitRemaining: rateLimitRemaining, rateLimitResetTime: rateLimitResetTime, data: data);

            IRedditResponse<IRedditObject> result = sut.As<IRedditObject>();
            Assert.AreEqual(rateLimitUsed, result.RateLimitUsed);
            Assert.AreEqual(rateLimitRemaining, result.RateLimitRemaining);
            Assert.AreEqual(rateLimitResetTime, result.RateLimitResetTime);
            Assert.AreEqual(data, result.Data);
        }

        [TestMethod]
        public void As_WhenCalled_ExpectRedditResponseNotEqualToSut()
        {
            IRedditResponse<MyRedditObject> sut = CreateSut();

            IRedditResponse<IRedditObject> result = sut.As<IRedditObject>();
            Assert.AreNotSame(sut, result);
        }

        [TestMethod]
        public void As_WhenCalled_ExpectDataInRedditResponseToBeInterface()
        {
            IRedditResponse<MyRedditObject> sut = CreateSut();

            IRedditResponse<IRedditObject> result = sut.As<IRedditObject>();
            Assert.AreEqual(typeof(IRedditObject), result.GetType().GetProperty("Data").PropertyType);
        }

        private IRedditResponse<MyRedditObject> CreateSut(int? rateLimitUsed = null, int? rateLimitRemaining = null, DateTime? rateLimitResetTime = null, MyRedditObject data = null)
        {
            return new OSDevGrp.MyDashboard.Core.Models.RedditResponse<MyRedditObject>(
                rateLimitUsed ?? _random.Next(100),
                rateLimitRemaining ?? _random.Next(100),
                rateLimitResetTime,
                data ?? new MyRedditObject());
        }

        private class MyRedditObject : OSDevGrp.MyDashboard.Core.Models.RedditObjectBase
        {
        }
    }
}