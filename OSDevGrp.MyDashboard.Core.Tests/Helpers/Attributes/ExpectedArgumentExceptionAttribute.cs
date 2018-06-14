using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExpectedArgumentExceptionAttribute : ExpectedExceptionBaseAttribute
    {
        #region Constructor

        public ExpectedArgumentExceptionAttribute(string expectedMessage, string expectedParamName) : base("No ArgumentException was thrown.")
        {
            if (string.IsNullOrWhiteSpace(expectedMessage))
            {
                throw new ArgumentNullException(nameof(expectedMessage));
            }
            if (string.IsNullOrWhiteSpace(expectedParamName))
            {
                throw new ArgumentNullException(nameof(expectedParamName));
            }
            ExpectedMessage = expectedMessage;
            ExpectedParamName = expectedParamName;
        }

        #endregion

        #region Properties

        public string ExpectedMessage
        {
            get;
            private set;
        }

        public string ExpectedParamName
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

            ArgumentException argumentException = exception as ArgumentException;
            Assert.IsNotNull(argumentException, "An ArgumentException was not thrown.");

            string expectedMessage = $"{ExpectedMessage + Environment.NewLine}Parameter name: {ExpectedParamName}"; 
            Assert.IsNotNull(argumentException.Message, "The Message in the thrown ArgumentException is null.");
            Assert.AreEqual(expectedMessage, argumentException.Message, $"The Message in the thrown ArgumentException was not '{expectedMessage}' but '{argumentException.Message}'.");
 
            Assert.IsNotNull(argumentException.ParamName, "The ParamName in the thrown ArgumentException is null.");
            Assert.AreEqual(ExpectedParamName, argumentException.ParamName, $"The ParamName in the thrown ArgumentException was not '{ExpectedParamName}' but '{argumentException.ParamName}'.");
        }

        #endregion
    }
}