using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.SystemErrorViewModelBuilder
{
    [TestClass]
    public class BuildAsyncTests
    {
        [TestMethod]
        [ExpectedArgumentNullExceptionAttribute("systemError")]
        public void BuildAsync_WhenSystemErrorIsNull_Throws()
        {
            IViewModelBuilder<SystemErrorViewModel, ISystemError> sut = CreateSut();

            sut.BuildAsync(null);
        }

        private IViewModelBuilder<SystemErrorViewModel, ISystemError> CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Factories.SystemErrorViewModelBuilder();
        }
    } 
}