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
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToNullAndUsedLowerThanAlreadyUsed_ExpectUsedUnthouched()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed - _random.Next(1, 10);

            int remaining = _random.Next(30, 60);
            DateTime? resetTime = null;
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(alreadyUsed, sut.Used);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToNullAndUsedEqualToAlreadyUsed_ExpectUsedUnthouched()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed;

            int remaining = _random.Next(30, 60);
            DateTime? resetTime = null;
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(alreadyUsed, sut.Used);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToNullAndUsedGreaterThanAlreadyUsed_ExpectUsedEqualToInputValue()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed + _random.Next(1, 10);

            int remaining = _random.Next(30, 60);
            DateTime? resetTime = null;
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(used, sut.Used);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToNullAndRemainingLowerThanExistingRemaining_ExpectRemainingEqualToInputValue()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining - _random.Next(1, 10);

            int used = _random.Next(30, 60);
            DateTime? resetTime = null;
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(remaining, sut.Remaining);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToNullAndRemainingEqualToExistingRemaining_ExpectRemainingUnthouched()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining;

            int used = _random.Next(30, 60);
            DateTime? resetTime = null;
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(existingRemaining, sut.Remaining);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToNullAndRemainingGreaterThanExistingRemaining_ExpectRemainingUnthouched()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining + _random.Next(1, 10);

            int used = _random.Next(30, 60);
            DateTime? resetTime = null;
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(existingRemaining, sut.Remaining);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToNull_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime? resetTime = null;
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut();

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeBeforeThanExistingResetTimeAndUsedLowerThanAlreadyUsed_ExpectUsedUnthouched()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed - _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 25) * -1);
                        
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed, resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(alreadyUsed, sut.Used);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeBeforeThanExistingResetTimeAndUsedEqualToAlreadyUsed_ExpectUsedUnthouched()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed;

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 25) * -1);
                        
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed, resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(alreadyUsed, sut.Used);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeBeforeThanExistingResetTimeAndUsedGreaterThanAlreadyUsed_ExpectUsedEqualToInputValue()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed + _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 25) * -1);
                        
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed, resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(used, sut.Used);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeBeforeThanExistingResetTimeAndRemainingLowerThanExistingRemaining_ExpectRemainingEqualToInputValue()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining - _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 25) * -1);
                        
            int used = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining, resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(remaining, sut.Remaining);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeBeforeThanExistingResetTimeAndRemainingEqualToExistingRemaining_ExpectRemainingUnthouched()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining;

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 25) * -1);
                        
            int used = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining, resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(existingRemaining, sut.Remaining);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeBeforeThanExistingResetTimeAndRemainingGreaterThanExistingRemaining_ExpectRemainingUnthouched()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining + _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 25) * -1);
                        
            int used = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining, resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(existingRemaining, sut.Remaining);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeBeforeThanExistingResetTime_ExpectResetTimeUnthouched()
        {
            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 25) * -1);
                        
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(existingResetTime, sut.ResetTime);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeBeforeThanExistingResetTime_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 25) * -1);
                        
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToExistingResetTimeAndUsedLowerThanAlreadyUsed_ExpectUsedUnthouched()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed - _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime;
                        
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed, resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(alreadyUsed, sut.Used);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToExistingResetTimeAndUsedEqualToAlreadyUsed_ExpectUsedUnthouched()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed;

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime;
                        
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed, resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(alreadyUsed, sut.Used);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToExistingResetTimeAndUsedGreaterThanAlreadyUsed_ExpectUsedEqualToInputValue()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed + _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime;
                        
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed, resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(used, sut.Used);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToExistingResetTimeAndRemainingLowerThanExistingRemaining_ExpectRemainingEqualToInputValue()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining - _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime;
                        
            int used = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining, resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(remaining, sut.Remaining);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToExistingResetTimeAndRemainingEqualToExistingRemaining_ExpectRemainingUnthouched()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining;

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime;
                        
            int used = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining, resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(existingRemaining, sut.Remaining);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToExistingResetTimeAndRemainingGreaterThanExistingRemaining_ExpectRemainingUnthouched()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining + _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime;
                        
            int used = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining, resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(existingRemaining, sut.Remaining);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToExistingResetTime_ExpectResetTimeUnthouched()
        {
            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime;
                        
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(existingResetTime, sut.ResetTime);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeEqualToExistingResetTime_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime;
                        
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeLaterThanExistingResetTime_ExpectUsedEqualToInputValue()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed + _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 30));
                        
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed, resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(used, sut.Used);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeLaterThanExistingResetTime_ExpectRemainingEqualToInputValue()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining - _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 30));
                        
            int used = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining, resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(remaining, sut.Remaining);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeLaterThanExistingResetTime_ExpectResetTimeEqualToInputValue()
        {
            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 30));
                                                
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(resetTime, sut.ResetTime);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithResetTimeLaterThanExistingResetTime_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(30, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(1, 30));
                                                                        
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            IRedditRateLimitLogic sut = CreateSut(resetTime: existingResetTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithReceivedTimeLowerThanLatestReceivedTime_ExpectUsedUnthouched()
        {
            int alreadyUsed = _random.Next(30, 60);
            int used = alreadyUsed + _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(10, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(10, 60));
            
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            DateTime latestReceivedTime = receivedTime.AddSeconds(_random.Next(10, 30));
            IRedditRateLimitLogic sut = CreateSut(used: alreadyUsed, resetTime: existingResetTime, latestReceivedTime: latestReceivedTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(alreadyUsed, sut.Used);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithReceivedTimeLowerThanLatestReceivedTime_ExpectRemainingUnthouched()
        {
            int existingRemaining = _random.Next(30, 60);
            int remaining = existingRemaining - _random.Next(1, 10);

            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(10, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(10, 60));
            
            int used = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            DateTime latestReceivedTime = receivedTime.AddSeconds(_random.Next(10, 30));
            IRedditRateLimitLogic sut = CreateSut(remaining: existingRemaining, resetTime: existingResetTime, latestReceivedTime: latestReceivedTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(existingRemaining, sut.Remaining);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithReceivedTimeLowerThanLatestReceivedTime_ExpectResetTimeUnthouched()
        {
            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(10, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(10, 60));
            
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            DateTime latestReceivedTime = receivedTime.AddSeconds(_random.Next(10, 30));
            IRedditRateLimitLogic sut = CreateSut(resetTime: existingResetTime, latestReceivedTime: latestReceivedTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

            Assert.AreEqual(existingResetTime, sut.ResetTime);
        }

        [TestMethod]
        public void EnforceRateLimitAsync_WhenCalledWithReceivedTimeLowerThanLatestReceivedTime_AssertHandleAsyncWasNotCalledOnExceptionHandler()
        {
            DateTime existingResetTime = DateTime.Now.AddSeconds(_random.Next(10, 60));
            DateTime? resetTime = existingResetTime.AddSeconds(_random.Next(10, 60));
            
            int used = _random.Next(30, 60);
            int remaining = _random.Next(30, 60);
            DateTime receivedTime = DateTime.Now;

            DateTime latestReceivedTime = receivedTime.AddSeconds(_random.Next(10, 30));
            IRedditRateLimitLogic sut = CreateSut(resetTime: existingResetTime, latestReceivedTime: latestReceivedTime);

            Task enforceRateLimitTask = sut.EnforceRateLimitAsync(used, remaining, resetTime, receivedTime);
            enforceRateLimitTask.Wait();

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