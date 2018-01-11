using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;

namespace OSDevGrp.MyDashboard.Core.Tests.Logic.RedditRateLimitLogic
{
    [TestClass]
    public class ConstructorTests
    {
        #region Private variables

        private Mock<IExceptionHandler> _exceptionHandlerMock;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
        }

        [TestMethod]
        public void Constructor_WhenCalled_AssertUsedEqualToZero()
        {
            IRedditRateLimitLogic sut = CreateSut();
            
            Assert.AreEqual(0, sut.Used);
        }

        [TestMethod]
        public void Constructor_WhenCalled_AssertRemainingEqualTo60()
        {
            IRedditRateLimitLogic sut = CreateSut();
            
            Assert.AreEqual(60, sut.Remaining);
        }

        [TestMethod]
        public void Constructor_WhenCalled_AssertResetTimeEqualToNowWithOneMinute()
        {
            IRedditRateLimitLogic sut = CreateSut();
            
            Assert.IsTrue(sut.ResetTime >= DateTime.Now && sut.ResetTime <= DateTime.Now.AddMinutes(1));
        }

        [TestMethod]
        public void Constructor_WhenCalled_AssertResetUtcTimeEqualToNowWithOneMinute()
        {
            IRedditRateLimitLogic sut = CreateSut();
            
            Assert.IsTrue(sut.ResetUtcTime >= DateTime.UtcNow && sut.ResetUtcTime <= DateTime.UtcNow.AddMinutes(1));
        }

        private IRedditRateLimitLogic CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Logic.RedditRateLimitLogic(_exceptionHandlerMock.Object);
        }
    }
}
