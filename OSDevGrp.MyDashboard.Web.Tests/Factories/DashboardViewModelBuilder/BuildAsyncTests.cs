using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.DashboardViewModelBuilder
{
    [TestClass]
    public class BuildAsyncTests
    {
        [TestMethod]
        [ExpectedArgumentNullExceptionAttribute("input")]
        public void BuildAsync_WhenDashboardIsNull_ThrowsArgumentNullException()
        {
            IViewModelBuilder<DashboardViewModel, IDashboard> sut = CreateSut();

            sut.BuildAsync(null);
        }

        private IViewModelBuilder<DashboardViewModel, IDashboard> CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Factories.DashboardViewModelBuilder();
        }
    } 
}