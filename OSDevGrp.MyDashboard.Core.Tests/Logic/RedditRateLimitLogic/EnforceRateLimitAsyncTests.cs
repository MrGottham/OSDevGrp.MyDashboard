using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;

namespace OSDevGrp.MyDashboard.Core.Tests.Logic.RedditRateLimitLogic
{
    [TestClass]
    public class EnforceRateLimitAsyncTests
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
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToNullAndUsedLowerThanAlreadyUsed_ExpectUsedUntouched()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed - _random.Next(1, 10);

            int remaining = _random.Next(30, 60);
            DateTime? resetTime = null;
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(alreadyUsed, sut.Used);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToNullAndUsedEqualToAlreadyUsed_ExpectUsedUntouched()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed;

            int remaining = _random.Next(30, 60);
            DateTime? resetTime = null;
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(alreadyUsed, sut.Used);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToNullAndUsedGreaterThanAlreadyUsed_ExpectUsedEqualToInputValue()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed + _random.Next(1, 10);

            int remaining = _random.Next(30, 60);
            DateTime? resetTime = null;
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(used, sut.Used);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToNullAndRemainingLowerThanExistingRemaining_ExpectRemainingEqualToInputValue()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining - _random.Next(1, 10);

            int used = _random.Next(30, 60);
            DateTime? resetTime = null;
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(remaining, sut.Remaining);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToNullAndRemainingEqualToExistingRemaining_ExpectRemainingUntouched()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining;

            int used = _random.Next(30, 60);
            DateTime? resetTime = null;
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(existingRemaining, sut.Remaining);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToNullAndRemainingGreaterThanExistingRemaining_ExpectRemainingUntouched()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining + _random.Next(1, 10);

            int used = _random.Next(30, 60);
            DateTime? resetTime = null;
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(existingRemaining, sut.Remaining);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToNull_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime? resetTime = null;
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut();

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeBeforeThanExistingResetTimeAndUsedLowerThanAlreadyUsed_ExpectUsedUntouched()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed - _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 25) * -1);
                        
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed, resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(alreadyUsed, sut.Used);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeBeforeThanExistingResetTimeAndUsedEqualToAlreadyUsed_ExpectUsedUntouched()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed;

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 25) * -1);
                        
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed, resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(alreadyUsed, sut.Used);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeBeforeThanExistingResetTimeAndUsedGreaterThanAlreadyUsed_ExpectUsedEqualToInputValue()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed + _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 25) * -1);
                        
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed, resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(used, sut.Used);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeBeforeThanExistingResetTimeAndRemainingLowerThanExistingRemaining_ExpectRemainingEqualToInputValue()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining - _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 25) * -1);
                        
            int used = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining, resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(remaining, sut.Remaining);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeBeforeThanExistingResetTimeAndRemainingEqualToExistingRemaining_ExpectRemainingUntouched()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining;

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 25) * -1);
                        
            int used = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining, resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(existingRemaining, sut.Remaining);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeBeforeThanExistingResetTimeAndRemainingGreaterThanExistingRemaining_ExpectRemainingUntouched()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining + _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 25) * -1);
                        
            int used = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining, resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(existingRemaining, sut.Remaining);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeBeforeThanExistingResetTime_ExpectResetTimeUntouched()
        {
            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 25) * -1);
                        
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(existingResetTime, sut.ResetTime);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeBeforeThanExistingResetTime_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 25) * -1);
                        
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToExistingResetTimeAndUsedLowerThanAlreadyUsed_ExpectUsedUntouched()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed - _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime;
                        
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed, resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(alreadyUsed, sut.Used);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToExistingResetTimeAndUsedEqualToAlreadyUsed_ExpectUsedUntouched()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed;

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime;
                        
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed, resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(alreadyUsed, sut.Used);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToExistingResetTimeAndUsedGreaterThanAlreadyUsed_ExpectUsedEqualToInputValue()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed + _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime;
                        
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed, resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(used, sut.Used);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToExistingResetTimeAndRemainingLowerThanExistingRemaining_ExpectRemainingEqualToInputValue()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining - _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime;
                        
            int used = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining, resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(remaining, sut.Remaining);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToExistingResetTimeAndRemainingEqualToExistingRemaining_ExpectRemainingUntouched()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining;

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime;
                        
            int used = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining, resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(existingRemaining, sut.Remaining);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToExistingResetTimeAndRemainingGreaterThanExistingRemaining_ExpectRemainingUntouched()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining + _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime;
                        
            int used = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining, resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(existingRemaining, sut.Remaining);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToExistingResetTime_ExpectResetTimeUntouched()
        {
            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime;
                        
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(existingResetTime, sut.ResetTime);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToExistingResetTime_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime;
                        
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeLaterThanExistingResetTime_ExpectUsedEqualToInputValue()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed + _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 30));
                        
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed, resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(used, sut.Used);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeLaterThanExistingResetTime_ExpectRemainingEqualToInputValue()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining - _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 30));
                        
            int used = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining, resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(remaining, sut.Remaining);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeLaterThanExistingResetTime_ExpectResetTimeEqualToInputValue()
        {
            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 30));
                                                
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(resetTime, sut.ResetTime);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithResetTimeLaterThanExistingResetTime_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 30));
                                                                        
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(resetTime: existingResetTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithReceivedTimeLowerThanLatestReceivedTime_ExpectUsedUntouched()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed + _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(10, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(10, 60));
            
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            DateTime latestReceivedTime = receivedTime.AddSeconds(_random.Next(10, 30));
            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed, resetTime: existingResetTime, latestReceivedTime: latestReceivedTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(alreadyUsed, sut.Used);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithReceivedTimeLowerThanLatestReceivedTime_ExpectRemainingUntouched()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining - _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(10, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(10, 60));
            
            int used = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            DateTime latestReceivedTime = receivedTime.AddSeconds(_random.Next(10, 30));
            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining, resetTime: existingResetTime, latestReceivedTime: latestReceivedTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(existingRemaining, sut.Remaining);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithReceivedTimeLowerThanLatestReceivedTime_ExpectResetTimeUntouched()
        {
            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(10, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(10, 60));
            
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            DateTime latestReceivedTime = receivedTime.AddSeconds(_random.Next(10, 30));
            IRedditRateLimitLogic sut = CreateSut(resetTime: existingResetTime, latestReceivedTime: latestReceivedTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            Assert.AreEqual(existingResetTime, sut.ResetTime);
        }

        [TestMethod]
        public async Task EnforceRateLimitAsync_WhenCalledWithReceivedTimeLowerThanLatestReceivedTime_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(10, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(10, 60));
            
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            DateTime latestReceivedTime = receivedTime.AddSeconds(_random.Next(10, 30));
            IRedditRateLimitLogic sut = CreateSut(resetTime: existingResetTime, latestReceivedTime: latestReceivedTime);

            await sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        private IRedditRateLimitLogic CreateSut(int? used = null, int? remaining = null, DateTime? resetTime = null, DateTime? latestReceivedTime = null)
        {
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<AggregateException>()))
                .Returns(Task.Run(() => { }));
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.Run(() => { }));

            return new MyRedditRateLimitLogic(
                _exceptionHandlerMock.Object,
                used,
                remaining,
                resetTime,
                latestReceivedTime);
        }

        private class MyRedditRateLimitLogic : OSDevGrp.MyDashboard.Core.Logic.RedditRateLimitLogic
        {
            #region Constructor

            public MyRedditRateLimitLogic(IExceptionHandler exceptionHandler, int? used = null, int? remaining = null, DateTime? resetTime = null, DateTime? latestReceivedTime = null) 
                : base(exceptionHandler)
            {
                Used = used ?? Used;
                Remaining = remaining ?? Remaining;
                ResetTime = resetTime ?? ResetTime;
                LatestReceivedTime = latestReceivedTime ?? LatestReceivedTime;
            }

            #endregion
        }
    }
}