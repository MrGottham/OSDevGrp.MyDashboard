using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Infrastructure.Randomizer
{
    [TestClass]
    public class NextTests
    {
        #region Private variables

        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentException("The maximum value cannot be lower than the minimum value.", "maxValue")]
        public void Next_WhenMaxValueLowerThanMinValue_ThrowsArgumentException()
        {
            int minValue = _random.Next(1, 100);
            int maxValue = minValue - _random.Next(1, 100);

            IRandomizer sut = CreateSut();

            sut.Next(minValue, maxValue);
        }

        [TestMethod]
        public void Next_WhenCalled_ReturnsValueInInterval()
        {
            int minValue = _random.Next(1, 100);
            int maxValue = minValue + _random.Next(1, 100);

            IRandomizer sut = CreateSut();

            int result = sut.Next(minValue, maxValue);

            Assert.IsTrue(result >= minValue && result <= maxValue);
        }

        private IRandomizer CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Infrastructure.Randomizer();
        }
    }
}