using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Models.RedditResponse
{
    [TestClass]
    public class ConstructorTests
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
        [ExpectedArgumentNullException("data")]
        public void Constructor_WhenCalledWhereDataIsNull_ThrowsArgumentNullException()
        {
            const MyRedditObject data = null;
            const bool hasData = false;

            IRedditResponse<MyRedditObject> sut = CreateSut(data: data, hasData: hasData);
        }

        [TestMethod]
        public void Constructor_WhenCalled_ExpectRateLimitUsedEqualToInputValue()
        {
            int rateLimitUsed = _random.Next(100);

            IRedditResponse<MyRedditObject> sut = CreateSut(rateLimitUsed: rateLimitUsed);

            Assert.AreEqual(rateLimitUsed, sut.RateLimitUsed);
        }

        [TestMethod]
        public void Constructor_WhenCalled_ExpectRateLimitRemainingEqualToInputValue()
        {
            int rateLimitRemaining = _random.Next(100);

            IRedditResponse<MyRedditObject> sut = CreateSut(rateLimitRemaining: rateLimitRemaining);

            Assert.AreEqual(rateLimitRemaining, sut.RateLimitRemaining);
        }

        [TestMethod]
        public void Constructor_WhenCalledWithRateLimitResetTimeEqualToNull_ExpectRateLimitResetTimeEqualToNull()
        {
            DateTime? rateLimitResetTime = null;

            IRedditResponse<MyRedditObject> sut = CreateSut(rateLimitResetTime: rateLimitResetTime);

            Assert.IsNull(sut.RateLimitResetTime);
        }

        [TestMethod]
        public void Constructor_WhenCalledWithRateLimitResetTimeEqualToNull_ExpectRateLimitResetUtcTimeEqualToNull()
        {
            DateTime? rateLimitResetTime = null;

            IRedditResponse<MyRedditObject> sut = CreateSut(rateLimitResetTime: rateLimitResetTime);

            Assert.IsNull(sut.RateLimitResetUtcTime);
        }

        [TestMethod]
        public void Constructor_WhenCalledWithRateLimitResetTimeNotEqualToNull_ExpectRateLimitResetTimeEqualToInputValue()
        {
            DateTime rateLimitResetTime = DateTime.Now.AddSeconds(_random.Next(300, 900));

            IRedditResponse<MyRedditObject> sut = CreateSut(rateLimitResetTime: rateLimitResetTime);

            Assert.AreEqual(rateLimitResetTime, sut.RateLimitResetTime);
        }

        [TestMethod]
        public void Constructor_WhenCalledWithRateLimitResetTimeNotEqualToNull_ExpectRateLimitResetUtcTimeEqualToInputValueAsUtcTime()
        {
            DateTime rateLimitResetTime = DateTime.Now.AddSeconds(_random.Next(300, 900));

            IRedditResponse<MyRedditObject> sut = CreateSut(rateLimitResetTime: rateLimitResetTime);

            Assert.AreEqual(rateLimitResetTime.ToUniversalTime(), sut.RateLimitResetUtcTime);
        }

        [TestMethod]
        public void Constructor_WhenCalled_ExpectReceivedTimeEqualToInputValue()
        {
            DateTime receivedTime = DateTime.Now.AddSeconds(_random.Next(30, 60) * -1);

            IRedditResponse<MyRedditObject> sut = CreateSut(receivedTime: receivedTime);

            Assert.AreEqual(receivedTime, sut.ReceivedTime);
        }

        [TestMethod]
        public void Constructor_WhenCalled_ExpectReceivedUtcTimeEqualToInputValueAsUtcTime()
        {
            DateTime receivedTime = DateTime.Now.AddSeconds(_random.Next(30, 60) * -1);

            IRedditResponse<MyRedditObject> sut = CreateSut(receivedTime: receivedTime);

            Assert.AreEqual(receivedTime.ToUniversalTime(), sut.ReceivedUtcTime);
        }

        [TestMethod]
        public void Constructor_WhenCalled_ExpectDataEqualToInputValue()
        {
            MyRedditObject data = new MyRedditObject();

            IRedditResponse<MyRedditObject> sut = CreateSut(data: data);

            Assert.AreEqual(data, sut.Data);
        }

        private IRedditResponse<MyRedditObject> CreateSut(int? rateLimitUsed = null, int? rateLimitRemaining = null, DateTime? rateLimitResetTime = null, DateTime? receivedTime = null, MyRedditObject data = null, bool hasData = true)
        {
            return new OSDevGrp.MyDashboard.Core.Models.RedditResponse<MyRedditObject>(
                rateLimitUsed ?? _random.Next(100),
                rateLimitRemaining ?? _random.Next(100),
                rateLimitResetTime,
                receivedTime ?? DateTime.Now,
                data ?? (hasData ? new MyRedditObject() : null));
        }

        private class MyRedditObject : OSDevGrp.MyDashboard.Core.Models.RedditObjectBase
        {
        }
    }
}
