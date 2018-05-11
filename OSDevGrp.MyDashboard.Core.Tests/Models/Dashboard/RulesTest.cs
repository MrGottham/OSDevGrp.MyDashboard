using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Tests.Models.Dashboard
{
    [TestClass]
    public class RulesTests
    {
        [TestMethod]
        public void Rules_WhenCalled_ExpectNotNull()
        {
            IDashboard sut = CreateSut();

            Assert.IsNotNull(sut.Rules);
        }

        [TestMethod]
        public void Rules_WhenCalled_ExpectTypeOfDashboardRules()
        {
            IDashboard sut = CreateSut();

            Assert.IsInstanceOfType(sut.Rules, typeof(OSDevGrp.MyDashboard.Core.Models.DashboardRules));
        }

        [TestMethod]
        public void Rules_WhenCalledMoreThanOnce_ExpectSameInstance()
        {
            IDashboard sut = CreateSut();

            IDashboardRules instance1 = sut.Rules;
            IDashboardRules instance2 = sut.Rules;

            Assert.AreSame(instance1, instance2);
        }

        private IDashboard CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Models.Dashboard();
        }
   }
}