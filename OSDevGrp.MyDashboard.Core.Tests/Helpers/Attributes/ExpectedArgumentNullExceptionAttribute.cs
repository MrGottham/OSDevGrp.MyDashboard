using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExpectedArgumentNullExceptionAttribute : ExpectedExceptionBaseAttribute
    {
        #region Constructor

        public ExpectedArgumentNullExceptionAttribute(string expectedParamName) : base("No ArgumentNullException was thrown.")
        {
            if (string.IsNullOrWhiteSpace(expectedParamName))
            {
                throw new ArgumentNullException(nameof(expectedParamName));
            }
            ExpectedParamName = expectedParamName;
        }

        #endregion

        #region Properties

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

            ArgumentNullException argumentNullException = exception as ArgumentNullException;
            Assert.IsNotNull(argumentNullException, "An ArgumentNullException was not thrown.");
            Assert.IsNotNull(argumentNullException.ParamName, "The ParamName in the thrown ArgumentNullException is null.");
            Assert.AreEqual(ExpectedParamName, argumentNullException.ParamName, $"The ParamName in the thrown ArgumentNullException was not '{ExpectedParamName}' but '{argumentNullException.ParamName}'.");
        }

        #endregion
    }
}