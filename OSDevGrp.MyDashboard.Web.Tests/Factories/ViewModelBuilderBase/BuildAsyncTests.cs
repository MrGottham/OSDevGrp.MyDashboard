using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Factories.ViewModelBuilderBase
{
    [TestClass]
    public class BuildAsyncTests
    {
        [TestMethod]
        [ExpectedArgumentNullExceptionAttribute("input")]
        public void BuildAsync_WhenInputIsNull_ThrowsArgumentNullException()
        {
            MyViewModelBuilder sut = CreateSut();

            sut.BuildAsync(null);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_AssertBuildWasCalledOnBaseClass()
        {
            IDashboard dashboard = CreateDashboard();

            MyViewModelBuilder sut = CreateSut();
            Assert.IsFalse(sut.BuildWasCalled);

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            Assert.IsTrue(sut.BuildWasCalled);
        }

        [TestMethod]
        public void BuildAsync_WhenCalled_ReturnsViewModelFromBaseClass()
        {
            IDashboard dashboard = CreateDashboard();

            DashboardViewModel dashboardViewModelToBuild = new DashboardViewModel();
            MyViewModelBuilder sut = CreateSut(dashboardViewModelToBuild);

            Task<DashboardViewModel> buildTask = sut.BuildAsync(dashboard);
            buildTask.Wait();

            Assert.AreEqual(dashboardViewModelToBuild, buildTask.Result);
        }

        private MyViewModelBuilder CreateSut(DashboardViewModel dashboardViewModelToBuild = null)
        {
            return new MyViewModelBuilder(dashboardViewModelToBuild ?? new DashboardViewModel());
        }

        private IDashboard CreateDashboard()
        {
            Mock<IDashboard> dashboardMock = new Mock<IDashboard>();
            return dashboardMock.Object;
        }

        private class MyViewModelBuilder : OSDevGrp.MyDashboard.Web.Factories.ViewModelBuilderBase<DashboardViewModel, IDashboard>
        {
            #region Private variables

            private readonly DashboardViewModel _dashboardViewModelToBuild;
            
            #endregion

            #region Constructor

            public MyViewModelBuilder(DashboardViewModel dashboardViewModelToBuild)
            {
                if (dashboardViewModelToBuild == null)
                {
                    throw new ArgumentNullException(nameof(dashboardViewModelToBuild));
                }
                
                _dashboardViewModelToBuild = dashboardViewModelToBuild;
            }

            #endregion

            #region Properties

            public bool BuildWasCalled
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            protected override DashboardViewModel Build(IDashboard input)
            {
                BuildWasCalled = true;
                return _dashboardViewModelToBuild;
            }

            #endregion
        }
    } 
}