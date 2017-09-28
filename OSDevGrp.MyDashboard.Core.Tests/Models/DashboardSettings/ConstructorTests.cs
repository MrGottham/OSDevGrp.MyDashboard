using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Tests.Models.DashboardSettings
{
    [TestClass]
    public class ConstructorTests
    {
        [TestMethod]
        public void Constructor_WhenCalled_ExpectNumberOfNewsEqualTo50()
        {
            IDashboardSettings sut = CreateSut();

            Assert.AreEqual(50, sut.NumberOfNews);
        }
        
        private IDashboardSettings CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Models.DashboardSettings();
        }
   }
}
