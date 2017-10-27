using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.NewsViewModelBuilder
{
    [TestClass]
    public class BuildAsyncTests
    {
        [TestMethod]
        [ExpectedArgumentNullExceptionAttribute("news")]
        public void BuildAsync_WhenNewsIsNull_Throws()
        {
            IViewModelBuilder<NewsViewModel, INews> sut = CreateSut();

            sut.BuildAsync(null);
        }

        private IViewModelBuilder<NewsViewModel, INews> CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Factories.NewsViewModelBuilder();
        }
    } 
}