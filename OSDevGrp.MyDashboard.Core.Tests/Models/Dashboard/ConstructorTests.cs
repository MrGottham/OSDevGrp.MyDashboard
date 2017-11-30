using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Tests.Models.Dashboard
{
    [TestClass]
    public class ConstructorTests
    {
        [TestMethod]
        public void Constructor_WhenCalled_ExpectNewsEqualToEmptyCollection()
        {
            IDashboard sut = CreateSut();

            Assert.IsNotNull(sut.News);
            Assert.IsFalse(sut.News.Any());
        }
        
        [TestMethod]
        public void Constructor_WhenCalled_ExpectSystemErrorsEqualToEmptyCollection()
        {
            IDashboard sut = CreateSut();

            Assert.IsNotNull(sut.SystemErrors);
            Assert.IsFalse(sut.SystemErrors.Any());
        }
        
        [TestMethod]
        public void Constructor_WhenCalled_ExpectSettingsEqualToNull()
        {
            IDashboard sut = CreateSut();

            Assert.IsNull(sut.Settings);
        }
        
        private IDashboard CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Models.Dashboard();
        }
   }
}
