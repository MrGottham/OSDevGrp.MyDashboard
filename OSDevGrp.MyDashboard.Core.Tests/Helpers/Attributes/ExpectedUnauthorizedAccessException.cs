using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExpectedUnauthorizedAccessException : ExpectedExceptionBaseAttribute
    {
        #region Constructor

        public ExpectedUnauthorizedAccessException(string expectedMessage = null) : base("No UnauthorizedAccessException was thrown.")
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
            Assert.IsNotNull(unauthorizedAccessException.Message, "The Message in the thrown UnauthorizedAccessException is null.");
            if (ExpectedMessage == null)
            {
                return;
            }
            Assert.AreEqual(ExpectedMessage, unauthorizedAccessException.Message, $"The Message in the thrown UnauthorizedAccessException was not '{ExpectedMessage}' but '{unauthorizedAccessException.Message}'.");
        }

        #endregion
    }
}