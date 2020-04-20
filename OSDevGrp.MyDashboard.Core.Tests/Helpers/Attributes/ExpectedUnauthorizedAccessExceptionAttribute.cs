using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExpectedUnauthorizedAccessExceptionAttribute : ExpectedExceptionBaseAttribute
    {
        #region Constructor

        public ExpectedUnauthorizedAccessExceptionAttribute(string expectedMessage = null)
        {
            ExpectedMessage = expectedMessage;
        }

        #endregion

        #region Properties

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

            UnauthorizedAccessException unauthorizedAccessException = exception as UnauthorizedAccessException;
            Assert.IsNotNull(unauthorizedAccessException, "An UnauthorizedAccessException was not thrown.");

            if (string.IsNullOrWhiteSpace(ExpectedMessage))
            {
                return;
            }

            Assert.AreEqual(ExpectedMessage, unauthorizedAccessException.Message, $"The Message in the thrown {exception.GetType().Name} was not '{ExpectedMessage}' but '{exception.Message}'.");
        }

        #endregion
    }
}