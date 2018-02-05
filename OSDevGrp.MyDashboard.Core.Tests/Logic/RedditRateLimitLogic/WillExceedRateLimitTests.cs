using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;

namespace OSDevGrp.MyDashboard.Core.Tests.Logic.RedditRateLimitLogic
{
    [TestClass]
    public class WillExceedRateLimitTests
    {
        #region Private variables

        private Mock<IExceptionHandler> _exceptionHandlerMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        public void WillExceedRateLimit_WhenExpectedCallsDoesExceedReamingAndResetTimeHasPassed_ExpectTrue()
        {
            int remaining = _random.Next(10, 60);
            DateTime resetTime = DateTime.Now.AddSeconds(_random.Next(1, 10) * -1);

            IRedditRateLimitLogic sut = CreateSut(remaining: remaining, resetTime: resetTime);

            bool result = sut.WillExceedRateLimit(remaining + _random.Next(1, 10));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WillExceedRateLimit_WhenExpectedCallsEqaulToReamingAndResetTimeHasPassed_ExpectTrue()
        {
            int remaining = _random.Next(10, 60);
            DateTime resetTime = DateTime.Now.AddSeconds(_random.Next(1, 10) * -1);

            IRedditRateLimitLogic sut = CreateSut(remaining: remaining, resetTime: resetTime);

            bool result = sut.WillExceedRateLimit(remaining);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WillExceedRateLimit_WhenExpectedCallsDoesNotExceedReamingAndResetTimeHasPassed_ExpectTrue()
        {
            int remaining = _random.Next(10, 60);
            DateTime resetTime = DateTime.Now.AddSeconds(_random.Next(1, 10) * -1);

            IRedditRateLimitLogic sut = CreateSut(remaining: remaining, resetTime: resetTime);

            bool result = sut.WillExceedRateLimit(remaining - _random.Next(1, 10));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WillExceedRateLimit_WhenExpectedCallsDoesExceedReamingAndResetTimeHasNotPassed_ExpectTrue()
        {
            int remaining = _random.Next(10, 60);
            DateTime resetTime = DateTime.Now.AddSeconds(_random.Next(5, 30));

            IRedditRateLimitLogic sut = CreateSut(remaining: remaining, resetTime: resetTime);

            bool result = sut.WillExceedRateLimit(remaining + _random.Next(1, 10));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WillExceedRateLimit_WhenExpectedCallsEqaulToReamingAndResetTimeHasNotPassed_ExpectFalse()
        {
            int remaining = _random.Next(10, 60);
            DateTime resetTime = DateTime.Now.AddSeconds(_random.Next(5, 30));

            IRedditRateLimitLogic sut = CreateSut(remaining: remaining, resetTime: resetTime);

            bool result = sut.WillExceedRateLimit(remaining);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void WillExceedRateLimit_WhenExpectedCallsDoesNotExceedReamingAndResetTimeHasNotPassed_ExpectFalse()
        {
            int remaining = _random.Next(10, 60);
            DateTime resetTime = DateTime.Now.AddSeconds(_random.Next(5, 30));

            IRedditRateLimitLogic sut = CreateSut(remaining: remaining, resetTime: resetTime);

            bool result = sut.WillExceedRateLimit(remaining - _random.Next(1, 10));

            Assert.IsFalse(result);
        }

        private IRedditRateLimitLogic CreateSut(int? used = null, int? remaining = null, DateTime? resetTime = null)
        {
            return new MyRedditRateLimitLogic(
                _exceptionHandlerMock.Object,
                used,
                remaining,
                resetTime);
        }

        private class MyRedditRateLimitLogic : OSDevGrp.MyDashboard.Core.Logic.RedditRateLimitLogic
        {
            #region Constructor

            public MyRedditRateLimitLogic(IExceptionHandler exceptionHandler, int? used = null, int? remaining = null, DateTime? resetTime = null) 
                : base(exceptionHandler)
            {
                Used = used ?? Used;
                Remaining = remaining ?? Remaining;
                ResetTime = resetTime ?? ResetTime;
            }

            #endregion
        }
    }
}