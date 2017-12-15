using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExpectedAggregateExceptionAttribute : ExpectedExceptionBaseAttribute
    {
        #region Constructor

        public ExpectedAggregateExceptionAttribute(Type expectedHandledExceptionType, string expectedMessage = null)
        {
            if (expectedHandledExceptionType == null)
            {
                throw new ArgumentNullException(nameof(expectedHandledExceptionType));
            }

            ExpectedHandledExceptionType = expectedHandledExceptionType;
            ExpectedMessage = expectedMessage;
        }

        #endregion

        #region Properties

        public Type ExpectedHandledExceptionType
        {
            get;
            private set;
        }

        public string ExpectedMessage
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        protected override void Verify(Exception exception)
        {
            Assert.IsNotNull(exception);

            base.RethrowIfAssertException(exception);

            AggregateException aggregateException = exception as AggregateException;
            Assert.IsNotNull(aggregateException, "An AggregateException was not thrown.");

            Exception handledException = null;
            aggregateException.Handle(ex =>
            {
                handledException = ex;
                return true;
            });
            
            Assert.IsNotNull(handledException, $"An {ExpectedHandledExceptionType.Name} was not thrown.");
            Assert.IsInstanceOfType(handledException, ExpectedHandledExceptionType, $"An {ExpectedHandledExceptionType.Name} was not thrown.");
            if (string.IsNullOrWhiteSpace(ExpectedMessage))
            {
                return;
            }
            Assert.AreEqual(ExpectedMessage, handledException.Message, $"The Message in the thrown {handledException.GetType().Name} was not '{ExpectedMessage}' but '{handledException.Message}'.");
        }

        #endregion
    }
}