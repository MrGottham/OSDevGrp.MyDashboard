using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.NewsToInformationViewModelBuilder
{
    [TestClass]
    public class BuildAsyncTests
    {
        [TestMethod]
        [ExpectedArgumentNullExceptionAttribute("input")]
        public void BuildAsync_WhenSystemErrorIsNull_ThrowsArgumentNullException()
        {
            IViewModelBuilder<InformationViewModel, INews> sut = CreateSut();

            sut.BuildAsync(null);
        }

        private IViewModelBuilder<InformationViewModel, INews> CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Factories.NewsToInformationViewModelBuilder();
        }
    } 
}