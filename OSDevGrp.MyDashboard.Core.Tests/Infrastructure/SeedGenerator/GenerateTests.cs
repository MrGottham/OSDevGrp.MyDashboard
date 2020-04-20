using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;

namespace OSDevGrp.MyDashboard.Core.Tests.Infrastructure.SeedGenerator
{
    [TestClass]
    public class GenerateTests
    {
        [TestMethod]
        public void Generate_WhenCalled_GeneratesUniqueSeeds()
        {
            ISeedGenerator sut = CreateSut();

            int[] seedArray = new int[]
            {
                sut.Generate(),
                sut.Generate(),
                sut.Generate(),
                sut.Generate(),
                sut.Generate(),
                sut.Generate(),
                sut.Generate(),
                sut.Generate(),
                sut.Generate(),
                sut.Generate()
            };

            Assert.AreEqual(seedArray.Distinct().Count(), seedArray.Length);
        }

        private ISeedGenerator CreateSut()
        {
            return new Core.Infrastructure.SeedGenerator();
        }
    }
}